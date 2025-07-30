using DataDomain;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayer
{
    public interface IQuestionManager
    {
        List<QuestionVM> GetAllQuestionsByQuizID(int quizID);
        bool AddNewQuizQuestion(Question question);
        bool EditQuestionInformation(Question question);
        List<QuestionVM> GetActiveQuestionsByQuizID(int quizID);
        QuestionVM GetQuestionByID(int questionID);
    }
}