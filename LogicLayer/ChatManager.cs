using DataAccessInterfaces;
using DataAccessLayer;
using DataDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayer
{
    public class ChatManager : IChatManager
    {
        private IChatAccessor _chatAccessor;

        // Constructor for Tests
        public ChatManager(IChatAccessor chatAccessor)
        {
            _chatAccessor = chatAccessor;
        }

        // Constructor for DB
        public ChatManager()
        {
            _chatAccessor = new ChatAccessor();
        }

        public List<ChatVM> GetStartedChats(int userID)
        {
            List<ChatVM> startedChats = null;

            try
            {
                startedChats = _chatAccessor.SelectChatsByOriginator(userID);
            }
            catch(Exception ex)
            {
                throw new ApplicationException("No Chats Found...", ex);
            }

            return startedChats;
        }

        public List<ChatVM> GetReceivedChats(int userID)
        {
            List<ChatVM> receivedChats = null;

            try
            {
                receivedChats = _chatAccessor.SelectChatsByRecipient(userID);
            }
            catch(Exception ex)
            {
                throw new ApplicationException("No Chats Found...", ex);
            }

            return receivedChats;
        }
    }
}