using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace DataDomain
{
    public class Chat
    {
        public int ChatID { get; set; }

        [Display(Name = "Topic")]
        public string ChatTypeID { get; set; }

        public int Originator { get; set; }
        public int Recipient { get; set; }

        public DateTime? CreatedOn { get; set; }
    }
    public class ChatVM : Chat
    {
        public string? OGivenName { get; set; }
        public string? OFamilyName { get; set; }

        [Display(Name = "From")]
        public string OriginatorName
        {
            get
            {
                return OGivenName + " " + OFamilyName;
            }
        }
        public string? RGivenName { get; set; }
        public string? RFamilyName { get; set; }

        [Display(Name = "Sent To")]
        public string RecipientName
        {
            get
            {
                return RGivenName + " " + RFamilyName;
            }
        }

        public DateTime LastMessageDate { get; set; }

        [Display(Name = "Started On")]
        public string? FormattedCreatedOn
        {
            get
            {
                return CreatedOn?.ToString("MM/dd/yyyy");
            }
        }
        [Display(Name = "Last Message")]
        public string FormattedLastMessageDate
        {
            get
            {
                return LastMessageDate.ToShortDateString();
            }
        }

        public string? Content { get; set; }
    }
}