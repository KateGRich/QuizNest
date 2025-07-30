using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataDomain
{
    public class Message
    {
        public int? MessageID { get; set; }
        public int ChatID { get; set; }
        public int SenderID { get; set; }
        public string Content { get; set; }
        public DateTime? DateSent { get; set; }
    }
    public class MessageVM : Message
    {
        public string SenderGivenName { get; set; }
        public string SenderFamilyName { get; set; }
        public string SenderName
        {
            get
            {
                return SenderGivenName + " " + SenderFamilyName;
            }
        }
        public string? FormattedDateSent
        {
            get
            {
                return DateSent?.ToString("MM/dd/yyyy HH:mm");
            }
        }
    }
}