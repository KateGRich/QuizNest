using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataDomain
{
    public class QuizRecord
    {
        public int QuizRecordID { get; set; }
        public string AttemptTypeID { get; set; }
        public int UserID { get; set; }
        public int QuizID { get; set; }
        public decimal Score { get; set; }
        public DateTime DateTaken { get; set; }
        public bool IsPublic { get; set; }
    }
    public class QuizRecordVM : QuizRecord
    {
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string UserName
        {
            get
            {
                return GivenName + " " + FamilyName;
            }
        }
        public string QuizName { get; set; }
        public int Place {  get; set; }
        public string FormattedDate
        {
            get
            {
                return DateTaken.ToString("MM/dd/yyyy");
            }
        }
        public string FormattedScore
        {
            get
            {
                return Score.ToString() + "%";
            }
        }
        public string QuizTopicID { get; set; }
    }
}