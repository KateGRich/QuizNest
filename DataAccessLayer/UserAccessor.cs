﻿using DataAccessInterfaces;
using DataDomain;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class UserAccessor : IUserAccessor
    {
        public int SelectUserCountByEmailAndPasswordHash(string email, string passwordHash)
        {
            int count = 0;
            // Get DB Connection.
            var conn = DBConnection.GetConnection();
            // Get a Command Object.
            var cmd = new SqlCommand("sp_verify_user", conn);
            // Set CommandType to StoredProcedure.
            cmd.CommandType = CommandType.StoredProcedure;
            // Set Parameters & Values.
            cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 250);
            cmd.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 100);
            cmd.Parameters["@Email"].Value = email;
            cmd.Parameters["@PasswordHash"].Value = passwordHash;
            // try-catch for code that works with DB.
            try
            {
                conn.Open();
                var result = cmd.ExecuteScalar();
                count = Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Incorrect Email or Password...", ex);
            }
            return count;
        }
        public UserVM SelectUserByEmail(string email)
        {
            UserVM? user = null;

            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_select_user_by_email", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 250);
            cmd.Parameters["@Email"].Value = email;
            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();
                if(reader.HasRows)
                {
                    reader.Read();
                    user = new UserVM()
                    {
                        UserID = reader.GetInt32(0),
                        GivenName = reader.GetString(1),
                        FamilyName = reader.GetString(2),
                        Email = reader.GetString(3),
                        PhoneNumber = reader.IsDBNull(4) ? null : reader.GetString(4),
                        ReactivationDate = reader.IsDBNull(5) ? null : reader.GetDateTime(5),
                        Active = reader.GetBoolean(6)
                    };
                }
                else
                {
                    throw new ArgumentException("User Record Not Found...");
                }
            }
            catch(Exception ex)
            {
                throw new ApplicationException("No Record Found...", ex);
            }
            return user;
        }
        public List<string> SelectRolesByUserID(int userID)
        {
            List<string> roles = new List<string>();

            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_select_roles_by_UserID", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@UserID", SqlDbType.Int);
            cmd.Parameters["@UserID"].Value = userID;
            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();
                while(reader.Read())
                {
                    roles.Add(reader.GetString(1));
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }
            return roles;
        }

        public List<UserVM> SelectAllUsers()
        {
            List<UserVM> users = new List<UserVM>();

            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_select_all_users", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();
                while(reader.Read())
                {
                    users.Add(new UserVM()
                    {
                        UserID = reader.GetInt32(0),
                        GivenName = reader.GetString(1),
                        FamilyName = reader.GetString(2),
                        Email = reader.GetString(3),
                        PhoneNumber = reader.IsDBNull(4) ? null : reader.GetString(4),
                        ReactivationDate = reader.IsDBNull(5) ? null : reader.GetDateTime(5),
                        Active = reader.GetBoolean(6)
                    });
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }
            return users;
        }

        public int InsertNewUser(User user)
        {
            int result = 0;

            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_insert_new_user", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@GivenName", SqlDbType.NVarChar, 50);
            cmd.Parameters.Add("@FamilyName", SqlDbType.NVarChar, 50);
            cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 250);
            cmd.Parameters.Add("@PhoneNumber", SqlDbType.NVarChar, 15);
            cmd.Parameters["@GivenName"].Value = user.GivenName;
            cmd.Parameters["@FamilyName"].Value = user.FamilyName;
            cmd.Parameters["@Email"].Value = user.Email;
            cmd.Parameters["@PhoneNumber"].Value = user.PhoneNumber;
            try
            {
                conn.Open();
                result = cmd.ExecuteNonQuery();
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }

            return result;
        }

        public int InserNewUserRole(UserRole userRole)
        {
            int rowsAffected = 0;

            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_insert_user_role", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@UserID", SqlDbType.Int);
            cmd.Parameters.Add("@RoleID", SqlDbType.NVarChar, 50);
            cmd.Parameters["@UserID"].Value = userRole.UserID;
            cmd.Parameters["@RoleID"].Value = userRole.RoleID;
            try
            {
                conn.Open();
                rowsAffected = cmd.ExecuteNonQuery();
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }

            return rowsAffected;
        }

        public int UpdateUserInformation(User user, User updatedUser)
        {
            // Not checking old values because the sp would not work with adding them.
            // Too many checks in the WHERE clause or something, but this is the only way I could get it to work.

            int result = 0;

            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_update_user_information", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@UserID", SqlDbType.Int);
            cmd.Parameters.Add("@newGivenName", SqlDbType.NVarChar, 50);
            cmd.Parameters.Add("@newFamilyName", SqlDbType.NVarChar, 50);
            cmd.Parameters.Add("@newEmail", SqlDbType.NVarChar, 250);
            cmd.Parameters.Add("@newPhoneNumber", SqlDbType.NVarChar, 15);
            cmd.Parameters.Add("@newActive", SqlDbType.Bit);
            cmd.Parameters.Add("@newReactivationDate", SqlDbType.DateTime);

            cmd.Parameters["@UserID"].Value = user.UserID;
            cmd.Parameters["@newGivenName"].Value = updatedUser.GivenName;
            cmd.Parameters["@newFamilyName"].Value = updatedUser.FamilyName;
            cmd.Parameters["@newEmail"].Value = updatedUser.Email;
            cmd.Parameters["@newPhoneNumber"].Value = (object)updatedUser.PhoneNumber ?? DBNull.Value;
            cmd.Parameters["@newActive"].Value = updatedUser.Active;
            cmd.Parameters["@newReactivationDate"].Value = (object)updatedUser.ReactivationDate ?? DBNull.Value;

            try
            {
                conn.Open();
                result = cmd.ExecuteNonQuery();
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }

            return result;
        }

        public int DeleteUserRole(UserRole userRole)
        {
            int rowsAffected = 0;

            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_delete_user_role", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@UserID", SqlDbType.Int);
            cmd.Parameters.Add("@RoleID", SqlDbType.NVarChar, 50);
            cmd.Parameters["@UserID"].Value = userRole.UserID;
            cmd.Parameters["@RoleID"].Value = userRole.RoleID;
            try
            {
                conn.Open();
                rowsAffected = cmd.ExecuteNonQuery();
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }

            return rowsAffected;
        }

        public UserVM SelectUserByUserID(int userID)
        {
            UserVM? user = null;

            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_select_user_by_userID", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@UserID", SqlDbType.Int);
            cmd.Parameters["@UserID"].Value = userID;
            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();
                if(reader.HasRows)
                {
                    reader.Read();
                    user = new UserVM()
                    {
                        UserID = reader.GetInt32(0),
                        GivenName = reader.GetString(1),
                        FamilyName = reader.GetString(2),
                        Email = reader.GetString(3),
                        PhoneNumber = reader.IsDBNull(4) ? null : reader.GetString(4),
                        ReactivationDate = reader.IsDBNull(5) ? null : reader.GetDateTime(5),
                        Active = reader.GetBoolean(6)
                    };
                }
                else
                {
                    throw new ArgumentException("User Record Not Found...");
                }
            }
            catch(Exception ex)
            {
                throw new ApplicationException("No Record Found...", ex);
            }
            return user;
        }

        public int UpdateUserPasswordHash(string email, string currentPasswordHash, string newPasswordHash)
        {
            int result = 0;

            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_update_user_passwordHash", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 250);
            cmd.Parameters.Add("@currentPasswordHash", SqlDbType.NVarChar, 100);
            cmd.Parameters.Add("@newPasswordHash", SqlDbType.NVarChar, 100);

            cmd.Parameters["@Email"].Value = email;
            cmd.Parameters["@currentPasswordHash"].Value = currentPasswordHash;
            cmd.Parameters["@newPasswordHash"].Value = newPasswordHash;

            try
            {
                conn.Open();
                result = cmd.ExecuteNonQuery();
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }

            return result;
        }
    }
}