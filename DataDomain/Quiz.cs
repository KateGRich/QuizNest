using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataDomain
{
    public class Quiz
    {
        public int QuizID { get; set; }

        [Display(Name = "Topic")]
        public string? QuizTopicID { get; set; }
        public string? Name { get; set; }
        public int CreatedBy { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool Active { get; set; }
    }
    public class QuizVM : Quiz
    {
        [Display(Name = "Topic Description")]
        public string? QuizTopicDescription { get; set; }

        [Display(Name = "Questions")]
        public int NumberOfQuestions { get; set; }

        [Display(Name = "Date Created")]
        public string FormattedDate
        {
            get
            {
                return CreatedOn.ToString("MM/dd/yyyy");
            }
        }
        public string? GivenName { get; set; }
        public string? FamilyName { get; set; }

        [Display(Name = "Creator")]
        public string CreatorName
        { 
            get
            {
                return GivenName + " " + FamilyName;
            }
        }
    }
}