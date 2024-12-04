using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataDomain
{
    public class Question
    {
        public int QuestionID { get; set; }
        public string QuestionTypeID { get; set; }
        public int QuizID { get; set; }
        public string Prompt { get; set; }
        public string? Answer1 { get; set; }
        public string? Answer2 { get; set; }
        public string? Answer3 { get; set; }
        public string? Answer4 { get; set; }
        public string CorrectAnswer { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool Active { get; set; }
    }
}