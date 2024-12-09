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
        bool AddNewQuizTopic(string quizTopic, string description);
        int AddNewQuiz(string quizTopicID, string name, int userID, string description); // Returns new QuizID.
        bool AddNewQuizQuestion(string questionTypeID, int quizID, string prompt, string answer1,
                string answer2, string answer3, string answer4, string correctAnswer);
        bool EditQuizInformation(int quizID, string newQuizTopicID, string newName, string newDescription, bool newActive);
    }
}