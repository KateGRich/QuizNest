using DataDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessInterfaces
{
    public interface IQuestionAccessor
    {
        List<QuestionVM> SelectAllQuestionsByQuizID(int quizID);
        int InsertNewQuizQuestion(Question question);
        int UpdateQuestionInformation(Question question);
        List<QuestionVM> SelectActiveQuestionsByQuizID(int quizID);
        QuestionVM SelectQuestionByID(int questionID);
    }
}