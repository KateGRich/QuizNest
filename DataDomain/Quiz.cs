using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataDomain
{
    public class Quiz
    {
        public int QuizID { get; set; }
        public string QuizTopicID { get; set; }
        public string Name { get; set; }
        public int CreatedBy { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool Active { get; set; }
    }
    public class QuizVM : Quiz
    {
        public int NumberOfQuestions { get; set; }
        public string FormattedDate
        {
            get
            {
                return CreatedOn.ToString("MM/dd/yyyy");
            }
        }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string CreatorName
        { 
            get
            {
                return GivenName + " " + FamilyName;
            }
        }
    }
}