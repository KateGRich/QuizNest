using DataDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessInterfaces
{
    public interface IChatAccessor
    {
        List<ChatVM> SelectChatsByOriginator(int userID);
        List<ChatVM> SelectChatsByRecipient(int userID);
    }
}