using DataAccessInterfaces;
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
    public class ChatAccessor : IChatAccessor
    {
        public List<ChatVM> SelectChatsByOriginator(int userID)
        {
            List<ChatVM> chats = new List<ChatVM>();

            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_select_chats_by_originator", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@UserID", SqlDbType.Int);
            cmd.Parameters["@UserID"].Value = userID;
            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();
                while(reader.Read())
                {
                    chats.Add(new ChatVM()
                    {
                        ChatID = reader.GetInt32(0),
                        ChatTypeID = reader.GetString(1),
                        Recipient = reader.GetInt32(2),
                        RGivenName = reader.GetString(3),
                        RFamilyName = reader.GetString(4),
                        CreatedOn = reader.GetDateTime(5),
                        LastMessageDate = reader.GetDateTime(6)
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
            return chats;
        }

        public List<ChatVM> SelectChatsByRecipient(int chatID)
        {
            List<ChatVM> chats = new List<ChatVM>();

            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_select_chats_by_recipient", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@UserID", SqlDbType.Int);
            cmd.Parameters["@UserID"].Value = chatID;
            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();
                while(reader.Read())
                {
                    chats.Add(new ChatVM()
                    {
                        ChatID = reader.GetInt32(0),
                        ChatTypeID = reader.GetString(1),
                        Originator = reader.GetInt32(2),
                        OGivenName = reader.GetString(3),
                        OFamilyName = reader.GetString(4),
                        CreatedOn = reader.GetDateTime(5),
                        LastMessageDate = reader.GetDateTime(6)
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
            return chats;
        }

        public List<ChatType> SelectAllChatTypes()
        {
            List<ChatType> chatTypes = new List<ChatType>();

            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_select_all_chat_types", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();
                while(reader.Read())
                {
                    chatTypes.Add(new ChatType
                    {
                        ChatTypeID = reader.GetString(0),
                        Description = reader.GetString(1),
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
            return chatTypes;
        }

        public List<User> SelectAllAdmins()
        {
            List<User> adminUsers = new List<User>();

            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_select_all_admins", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();
                while(reader.Read())
                {
                    adminUsers.Add(new User
                    {
                        UserID = reader.GetInt32(0),
                        GivenName = reader.GetString(1),
                        FamilyName = reader.GetString(2)
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
            return adminUsers;
        }

        public int InsertNewChat(ChatVM chat)
        {
            int result = 0;

            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_insert_new_chat_and_message", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@ChatTypeID", SqlDbType.NVarChar, 50);
            cmd.Parameters.Add("@Originator", SqlDbType.Int);
            cmd.Parameters.Add("@Recipient", SqlDbType.Int);
            cmd.Parameters.Add("@Content", SqlDbType.NVarChar, 250);


            cmd.Parameters["@ChatTypeID"].Value = chat.ChatTypeID;
            cmd.Parameters["@Originator"].Value = chat.Originator;
            cmd.Parameters["@Recipient"].Value = chat.Recipient;
            cmd.Parameters["@Content"].Value = chat.Content;

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

        public List<MessageVM> SelectMessagesByChatID(int chatID)
        {
            List<MessageVM> messages = new List<MessageVM>();

            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_select_messages_by_chatID", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@ChatID", SqlDbType.Int);
            cmd.Parameters["@ChatID"].Value = chatID;
            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();
                while(reader.Read())
                {
                    messages.Add(new MessageVM()
                    {
                        MessageID = reader.GetInt32(0),
                        ChatID = reader.GetInt32(1),
                        SenderID = reader.GetInt32(2),
                        SenderGivenName = reader.GetString(3),
                        SenderFamilyName = reader.GetString(4),
                        Content = reader.GetString(5),
                        DateSent = reader.GetDateTime(6)
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
            return messages;
        }

        public int InsertNewMessage(Message message)
        {
            int result = 0;

            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_insert_new_message", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@ChatID", SqlDbType.Int);
            cmd.Parameters.Add("@SenderID", SqlDbType.Int);
            cmd.Parameters.Add("@Content", SqlDbType.NVarChar, 250);


            cmd.Parameters["@ChatID"].Value = message.ChatID;
            cmd.Parameters["@SenderID"].Value = message.SenderID;
            cmd.Parameters["@Content"].Value = message.Content;

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