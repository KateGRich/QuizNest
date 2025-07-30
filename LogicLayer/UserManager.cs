using DataAccessInterfaces;
using DataAccessLayer;
using DataDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayer
{
    public class UserManager : IUserManager
    {
        private IUserAccessor _userAccessor;

        // Constructor for Tests
        public UserManager(IUserAccessor userAccessor)
        {
            _userAccessor = userAccessor;
        }

        // Constructor for DB
        public UserManager()
        {
            _userAccessor = new UserAccessor();
        }

        public string HashSHA256(string password)
        {
            if (password == "" || password == null)
            {
                throw new ArgumentException("Missing Input");
            }

            string hashValue = null;
            byte[] data;

            // Create a hash provider object.
            using(SHA256 sha256Provider = SHA256.Create())
            {
                data = sha256Provider.ComputeHash(Encoding.UTF8.GetBytes(password));
            }

            // Build a string from the result.
            var s = new StringBuilder();
            for(int i = 0; i < data.Length; i++)
            {
                s.Append(data[i].ToString("x2"));
            }
            hashValue = s.ToString().ToLower();

            return hashValue;
        }

        public bool VerifyUser(string email, string password)
        {
            bool result = false;

            password = HashSHA256(password);
            result = (1 == _userAccessor.SelectUserCountByEmailAndPasswordHash(email, password));

            return result;
        }

        public User GetUserByEmail(string email)
        {
            UserVM? user = null;

            try
            {
                user = _userAccessor.SelectUserByEmail(email);
            }
            catch(Exception ex)
            {
                throw new ApplicationException("Log In Failed", ex);
            }

            return user;
        }

        public List<string> GetUserRoles(int userID)
        {
            List<string>? roles = null;
            try
            {
                roles = _userAccessor.SelectRolesByUserID(userID);
                if(roles.Count == 0)
                {
                    throw new Exception("No roles were retrieved from the DB...");
                }
            }
            catch(Exception ex)
            {
                throw new ApplicationException("No Roles Found", ex);
            }

            return roles;
        }

        public UserVM LogInUser(string email, string password)
        {
            UserVM? userVM = null;
            try
            {
                if(VerifyUser(email, password))
                {
                    // Account exists & is Active.
                    userVM = (UserVM)GetUserByEmail(email);
                    if(userVM != null)
                    {
                        userVM.Roles = GetUserRoles(userVM.UserID);
                    }
                }
                else if((userVM = (UserVM)GetUserByEmail(email)).Active == false)
                {
                    // Account exists, but is not Active.
                    string message = "Your account is no longer active..." + "\n\n";
                    if(userVM.ReactivationDate == null)
                    {
                        message += "Please speak directly with a system admin if you'd like to reactivate it.";
                    }
                    else
                    {
                        message += "Your account will become active again on " + userVM.FormattedReactivationDate + ".";
                    }
                    throw new ApplicationException(message);
                }
                else
                {
                    throw new ArgumentException("Incorrect Email or Password...");
                }
            }
            catch(Exception)
            {
                // Exception would already be wrapped by logic functions.
                throw;
            }
            return userVM;
        }

        public List<UserVM> GetAllUsers()
        {
            List<UserVM> users = null;
            try
            {
                users = _userAccessor.SelectAllUsers();

                // Get Roles to Display.
                foreach(var user in users)
                {
                    user.Roles = GetUserRoles(user.UserID);
                }

                if(users.Count == 0)
                {
                    // There must be at least 1 User record in the DB.
                    throw new Exception("No users were retrieved from the DB...");
                }
            }
            catch(Exception ex)
            {
                // There must be at least 1 User record in the DB.
                throw new ApplicationException("No Users Found", ex);
            }

            return users;
        }

        public bool AddNewUser(User user, List<string> roles)
        {
            bool added = false;
            
            int rowsAffected = 0;

            User newUser = null;
            int newUserID = 0;


            try
            {
                rowsAffected = _userAccessor.InsertNewUser(user);
                if(rowsAffected == 1)
                {
                    newUser = _userAccessor.SelectUserByEmail(user.Email);
                    newUserID = newUser.UserID;

                    foreach(string role in roles)
                    {
                        UserRole userRole = new UserRole()
                        {
                            UserID = newUserID,
                            RoleID = role
                        };
                        rowsAffected = _userAccessor.InserNewUserRole(userRole);
                        if(rowsAffected == 0)
                        {
                            throw new Exception("Role not added...");
                        }
                    }
                    added = true;
                }
            }
            catch(Exception ex)
            {
                // If the user & roles are not successfully added.
                throw new ApplicationException("New User Creation Failed...", ex);
            }

            return added;
        }

        public bool EditUserInformation(UserVM userEdit, User updatedUser, List<string> newRoles)
        {
            bool updated = false;

            int rowsAffected = 0;

            User editUser = userEdit;
            List<string> userRoles = newRoles;

            User checkUser = null; // Use this to check that we are not updating the editUser's email to an existing User's email.

            try
            {
                rowsAffected = _userAccessor.UpdateUserInformation(editUser, updatedUser);
                if(rowsAffected == 1)
                {
                    foreach(string role in newRoles)
                    {
                        if(userEdit.Roles.Contains(role))
                        {
                            continue;
                        }
                        else
                        {
                            UserRole newRole = new UserRole()
                            {
                                UserID = userEdit.UserID,
                                RoleID = role
                            };
                            rowsAffected = _userAccessor.InserNewUserRole(newRole);
                            if(rowsAffected == 0)
                            {
                                throw new Exception("Role not added...");
                            }
                        }
                    }

                    foreach(string role in userEdit.Roles)
                    {
                        if(newRoles.Contains(role))
                        {
                            continue;
                        }
                        else
                        {
                            UserRole userRole = new UserRole()
                            {
                                UserID = editUser.UserID,
                                RoleID = role
                            };
                            rowsAffected = _userAccessor.DeleteUserRole(userRole);
                        }
                    }
                }

                updated = true;
            }
            catch(Exception ex)
            {
                // If the user & roles are not successfully added/updated.
                throw new ApplicationException("Update Failed...", ex);
            }

            return updated;
        }

        public UserVM GetUserByUserID(int userID)
        {
            UserVM? user = null;

            try
            {
                user = _userAccessor.SelectUserByUserID(userID);

                user.Roles = GetUserRoles(user.UserID);
            }
            catch(Exception ex)
            {
                throw new ApplicationException("Log In Failed", ex);
            }

            return user;
        }

        public bool UpdatePassword(string email, string currentPassword, string newPassword)
        {
            bool result = false;

            // Hash the passwords.
            currentPassword = HashSHA256(currentPassword);
            newPassword = HashSHA256(newPassword);

            try
            {
                result = (1 == _userAccessor.UpdateUserPasswordHash(email, currentPassword, newPassword));
                if(!result)
                {
                    throw new ApplicationException("You must enter your correct current password.");
                }
            }
            catch(Exception ex)
            {
                throw new ApplicationException("Your Password Update was Unsuccessful...", ex);
            }
            return result;
        }
    }
}