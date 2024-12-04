using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataDomain
{
    public class Message
    {
        public int MessageID { get; set; }
        public int ChatID { get; set; }
        public int SenderID { get; set; }
        public string Content { get; set; }
        public DateTime DateSent { get; set; }
    }
}