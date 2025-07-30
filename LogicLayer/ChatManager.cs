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

        public List<ChatType> GetAllChatTypes()
        {
            List<ChatType> chatTypes = null;

            try
            {
                chatTypes = _chatAccessor.SelectAllChatTypes();
            }
            catch(Exception ex)
            {
                throw new ApplicationException("Something is very wrong...", ex);
            }

            return chatTypes;
        }

        public List<User> GetAllAdmins()
        {
            List<User> adminUsers = null;

            try
            {
                adminUsers = _chatAccessor.SelectAllAdmins();
            }
            catch(Exception ex)
            {
                throw new ApplicationException("Something is very wrong...", ex);
            }

            return adminUsers;
        }

        public bool AddNewChat(ChatVM chat)
        {
            bool added = false;

            try
            {
                int result = _chatAccessor.InsertNewChat(chat);
                if(result > 0)
                {
                    added = true;
                }
            }
            catch(Exception ex)
            {
                throw new ApplicationException("Chat Failed to Send...", ex);
            }

            return added;
        }

        public List<MessageVM> GetMessagesByChatID(int chatID)
        {
            List<MessageVM> messages = null;

            try
            {
                messages = _chatAccessor.SelectMessagesByChatID(chatID);
            }
            catch(Exception ex)
            {
                throw new ApplicationException("No Messages Found...", ex);
            }

            return messages;
        }

        public bool AddNewMessage(Message message)
        {
            bool added = false;

            try
            {
                int result = _chatAccessor.InsertNewMessage(message);
                if(result == 1)
                {
                    added = true;
                }
            }
            catch(Exception ex)
            {
                throw new ApplicationException("Message Failed to Send...", ex);
            }

            return added;
        }
    }
}