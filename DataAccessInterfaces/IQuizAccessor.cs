using DataDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessInterfaces
{
    public interface IQuizAccessor
    {
        List<QuizVM> SelectQuizzesByCreator(int userID);
        int SelectTotalCountOfQuestionsByQuiz(int quizID);
        List<QuizVM> SelectAllActiveQuizzes();
        List<QuizTopic> SelectAllQuizTopics();
        List<string> SelectAllQuestionTypes();
        int InsertNewQuizTopic(QuizTopic quizTopic);
        int InsertNewQuiz(Quiz quiz); // Returns new QuizID.
        int UpdateQuizInformation(Quiz quiz, Quiz newQuiz);
        QuizVM SelectQuizByID(int quizID);
        int SelectCountOfActiveQuestionsByQuiz(int quizID);
    }
}