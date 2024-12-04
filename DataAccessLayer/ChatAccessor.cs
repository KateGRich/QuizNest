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

        public List<ChatVM> SelectChatsByRecipient(int userID)
        {
            List<ChatVM> chats = new List<ChatVM>();

            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_select_chats_by_recipient", conn);
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
    }
}