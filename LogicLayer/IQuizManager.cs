using DataDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayer
{
    public interface IQuizManager
    {
        List<QuizVM> GetQuizzesByCreator(int userID);
        List<QuizVM> GetAllActiveQuizes();
        List<QuizTopic> GetAllQuizTopics();
        List<string> GetAllQuestionTypes();
        bool AddNewQuizTopic(QuizTopic quizTopic);
        int AddNewQuiz(Quiz quiz); // Returns new QuizID.
        bool EditQuizInformation(Quiz quiz, Quiz newQuiz);
        QuizVM GetQuizByID(int quizID);
    }
}