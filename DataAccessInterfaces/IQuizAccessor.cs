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
        int InsertNewQuizTopic(string quizTopic, string description);
        int InsertNewQuiz(string quizTopicID, string name, int userID, string description); // Returns new QuizID.
        int InsertNewQuizQuestion(string questionTypeID, int quizID, string prompt, string answer1,
                string answer2, string answer3, string answer4, string correctAnswer);
    }
}