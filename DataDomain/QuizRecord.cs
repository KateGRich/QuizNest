using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DataDomain
{
    public class QuizRecord
    {
        public int QuizRecordID { get; set; }

        [Display(Name = "Attempt Type")]
        public string AttemptTypeID { get; set; }
        public int UserID { get; set; }
        public int QuizID { get; set; }
        public decimal Score { get; set; }
        public DateTime DateTaken { get; set; }

        [Display(Name = "Public")]
        public bool IsPublic { get; set; }
    }
    public class QuizRecordVM : QuizRecord
    {
        public string? GivenName { get; set; }
        public string? FamilyName { get; set; }

        [Display(Name = "Name")]
        public string? UserName
        {
            get
            {
                return GivenName + " " + FamilyName;
            }
        }

        [Display(Name = "Quiz")]
        public string? QuizName { get; set; }
        public int Place {  get; set; }

        [Display(Name = "Date Taken")]
        public string FormattedDate
        {
            get
            {
                return DateTaken.ToString("MM/dd/yyyy");
            }
        }

        [Display(Name = "Score")]
        public string FormattedScore
        {
            get
            {
                return Score.ToString() + "%";
            }
        }

        [Display(Name = "Quiz Topic")]
        public string QuizTopicID { get; set; }
    }
}