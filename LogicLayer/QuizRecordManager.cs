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
    public class QuizRecordManager : IQuizRecordManager
    {
        private IQuizRecordAccessor _quizRecordAccessor;

        // Constructor for Tests
        public QuizRecordManager(IQuizRecordAccessor quizRecordAccessor)
        {
            _quizRecordAccessor = quizRecordAccessor;
        }

        // Constructor for DB
        public QuizRecordManager()
        {
            _quizRecordAccessor = new QuizRecordAccessor();
        }

        public List<QuizRecordVM> GetQuizLeaderboard(int quizID)
        {
            List<QuizRecordVM> leaderboard = null;
            try
            {
                leaderboard = _quizRecordAccessor.SelectQuizLeaderboard(quizID);

                for(int i = 0; i < leaderboard.Count; i++)
                {
                    leaderboard[i].Place = i + 1;
                }
            }
            catch(Exception ex)
            {
                throw new ApplicationException("No Records Found...", ex);
            }

            return leaderboard;
        }

        public List<QuizRecordVM> GetTakenQuizzes(int userID)
        {
            List<QuizRecordVM> takenQuizzes = null;

            try
            {
                takenQuizzes = _quizRecordAccessor.SelectQuizzesByTaker(userID);
            }
            catch(Exception ex)
            {
                throw new ApplicationException("No Records Found...", ex);
            }

            return takenQuizzes;
        }
    }
}