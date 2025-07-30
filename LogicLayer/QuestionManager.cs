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

        public List<QuestionVM> GetAllQuestionsByQuizID(int quizID)
        {
            List<QuestionVM> questions = null;

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

        public bool AddNewQuizQuestion(Question question)
        {
            bool added = false;

            int rowsAffected = 0;

            try
            {
                rowsAffected = _questionAccessor.InsertNewQuizQuestion(question);
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

        public bool EditQuestionInformation(Question question)
        {
            bool updated = false;

            int rowsAffected = 0;

            try
            {
                rowsAffected = _questionAccessor.UpdateQuestionInformation(question);
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

        public List<QuestionVM> GetActiveQuestionsByQuizID(int quizID)
        {
            List<QuestionVM> questions = null;

            try
            {
                questions = _questionAccessor.SelectActiveQuestionsByQuizID(quizID);
            }
            catch(Exception ex)
            {
                throw new ApplicationException("No questions found...", ex);
            }

            return questions;
        }

        public QuestionVM GetQuestionByID(int questionID)
        {
            QuestionVM questions = null;

            try
            {
                questions = _questionAccessor.SelectQuestionByID(questionID);
            }
            catch(Exception ex)
            {
                throw new ApplicationException("No questions found...", ex);
            }

            return questions;
        }
    }
}