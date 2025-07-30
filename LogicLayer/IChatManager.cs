using DataDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayer
{
    public interface IChatManager
    {
        List<ChatVM> GetStartedChats(int userID);
        List<ChatVM> GetReceivedChats(int userID);
        List<ChatType> GetAllChatTypes();
        List<User> GetAllAdmins();
        bool AddNewChat(ChatVM chat);
        List<MessageVM> GetMessagesByChatID(int chatID);
        bool AddNewMessage(Message message);
    }
}