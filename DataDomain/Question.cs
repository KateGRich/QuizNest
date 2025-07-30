using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DataDomain
{
    public class Question
    {
        public int QuestionID { get; set; }

        [Display(Name = "Question Type")]
        public string QuestionTypeID { get; set; }
        public int QuizID { get; set; }
        public string? Prompt { get; set; }

        [Display(Name = "Option 1")]
        public string? Answer1 { get; set; }
        [Display(Name = "Option 2")]
        public string? Answer2 { get; set; }
        [Display(Name = "Option 3")]
        public string? Answer3 { get; set; }
        [Display(Name = "Option 4")]
        public string? Answer4 { get; set; }
        [Display(Name = "Correct Answer")]
        public string? CorrectAnswer { get; set; }

        public DateTime CreatedOn { get; set; }
        public bool Active { get; set; }
    }
    public class QuestionVM : Question
    {
        [Display(Name = "Quiz")]
        public string? QuizName { get; set; }

        [Display(Name = "Question Number")]
        public int QuestionNumber { get; set; }
        public bool EnableEdit { get; set; }

        [Display(Name = "Attempt")]
        public string? AttemptTypeID { get; set; }

        [Display(Name = "My Answer")]
        public string? MyAnswer { get; set; }
    }
    public class MissedQuestion : Question
    {
        [Display(Name = "Quiz")]
        public string? QuizName { get; set; }

        [Display(Name = "Question Number")]
        public int QuestionNumber { get; set; }

        [Display(Name = "Attempt")]
        public string? AttemptTypeID { get; set; }
        public int QuizRecordID { get; set; }

        [Display(Name = "My Answer")]
        public string? MyAnswer { get; set; }
    }
}