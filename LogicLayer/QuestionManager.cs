using DataAccessInterfaces;
using DataAccessLayer;
using DataDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayer
{
    public class QuestionManager : IQuestionManager
    {
        private IQuestionAccessor _questionAccessor;

        // Constructor for Tests
        public QuestionManager(IQuestionAccessor questionAccessor)
        {
            _questionAccessor = questionAccessor;
        }

        // Constructor for DB
        public QuestionManager()
        {
            _questionAccessor = new QuestionAccessor();
        }

        public List<Question> GetAllQuestionsByQuizID(int quizID)
        {
            List<Question> questions = null;

            try
            {
                questions = _questionAccessor.SelectAllQuestionsByQuizID(quizID);
            }
            catch(Exception ex)
            {
                throw new ApplicationException("No questions found...", ex);
            }

            return questions;
        }

        public bool AddNewQuizQuestion(string questionTypeID, int quizID, string prompt, string answer1,
                    string answer2, string answer3, string answer4, string correctAnswer)
        {
            bool added = false;

            int rowsAffected = 0;

            try
            {
                rowsAffected = _questionAccessor.InsertNewQuizQuestion(questionTypeID, quizID, prompt,
                                    answer1, answer2, answer3, answer4, correctAnswer);
                if(rowsAffected == 1)
                {
                    added = true;
                }
            }
            catch(Exception ex)
            {
                throw new ApplicationException("New Quiz Question Creation Failed...", ex);
            }

            return added;
        }

        public bool EditQuestionInformation(int questionID, string newQuestionTypeID, int quizID, string newPrompt,
                        string newAnswer1, string newAnswer2, string newAnswer3, string newAnswer4,
                        string newCorrectAnswer, bool newActive)
        {
            bool updated = false;

            int rowsAffected = 0;

            try
            {
                rowsAffected = _questionAccessor.UpdateQuestionInformation(questionID, newQuestionTypeID, quizID, newPrompt, newAnswer1,
                                        newAnswer2, newAnswer3, newAnswer4, newCorrectAnswer, newActive);
                if(rowsAffected == 1)
                {
                    updated = true;
                }
            }
            catch(Exception ex)
            {
                throw new ApplicationException("Question Update Failed...", ex);
            }

            return updated;
        }
    }
}