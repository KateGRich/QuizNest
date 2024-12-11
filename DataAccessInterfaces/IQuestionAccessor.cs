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
        List<Question> SelectAllQuestionsByQuizID(int quizID);
        int InsertNewQuizQuestion(string questionTypeID, int quizID, string prompt, string answer1,
                string answer2, string answer3, string answer4, string correctAnswer);
        int UpdateQuestionInformation(int questionID, string newQuestionTypeID, int quizID, string newPrompt,
                        string newAnswer1, string newAnswer2, string newAnswer3, string newAnswer4,
                        string newCorrectAnswer, bool newActive);
        List<Question> SelectActiveQuestionsByQuizID(int quizID);
    }
}