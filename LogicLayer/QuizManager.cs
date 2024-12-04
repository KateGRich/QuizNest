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
    public class QuizManager : IQuizManager
    {
        private IQuizAccessor _quizAccessor;

        // Constructor for Tests
        public QuizManager(IQuizAccessor quizAccessor)
        {
            _quizAccessor = quizAccessor;
        }

        // Constructor for DB
        public QuizManager()
        {
            _quizAccessor = new QuizAccessor();
        }

        public List<QuizVM> GetQuizzesByCreator(int userID)
        {
            List<QuizVM> quizzes = null;

            try
            {
                quizzes = _quizAccessor.SelectQuizzesByCreator(userID);

                foreach(QuizVM quiz in quizzes)
                {
                    quiz.NumberOfQuestions = _quizAccessor.SelectTotalCountOfQuestionsByQuiz(quiz.QuizID);
                }
            }
            catch(Exception ex)
            {
                throw new ApplicationException("Something is very wrong...", ex);
            }

            return quizzes;
        }

        public List<QuizVM> GetAllActiveQuizes()
        {
            List<QuizVM> quizzes = null;

            try
            {
                quizzes = _quizAccessor.SelectAllActiveQuizzes();

                foreach(QuizVM quiz in quizzes)
                {
                    quiz.NumberOfQuestions = _quizAccessor.SelectTotalCountOfQuestionsByQuiz(quiz.QuizID);
                }
            }
            catch(Exception ex)
            {
                throw new ApplicationException("Something is very wrong...", ex);
            }

            return quizzes;
        }
    }
}