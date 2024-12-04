using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace DataDomain
{
    public class Chat
    {
        public int ChatID { get; set; }
        public string ChatTypeID { get; set; }
        public int Originator { get; set; }
        public int Recipient { get; set; }
        public DateTime CreatedOn { get; set; }
    }
    public class ChatVM : Chat
    {
        public string OGivenName { get; set; }
        public string OFamilyName { get; set; }
        public string OriginatorName
        {
            get
            {
                return OGivenName + " " + OFamilyName;
            }
        }
        public string RGivenName { get; set; }
        public string RFamilyName { get; set; }
        public string RecipientName
        {
            get
            {
                return RGivenName + " " + RFamilyName;
            }
        }
        public DateTime LastMessageDate { get; set; }
    }
}