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
            List<QuizRecordVM> leaderboard;
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

        public int AddQuizRecord(QuizRecord quizRecord)
        {
            int newRecordID = 0;

            try
            {
                newRecordID = _quizRecordAccessor.InsertQuizRecord(quizRecord);
            }
            catch(Exception ex)
            {
                throw new ApplicationException("New Quiz Creation Failed...", ex);
            }

            return newRecordID;
        }

        public bool AddMissedQuestion(int quizRecordID, int questionID)
        {
            bool added = false;

            int rowsAffected = 0;

            try
            {
                rowsAffected = _quizRecordAccessor.InsertMissedQuestion(quizRecordID, questionID);
                if(rowsAffected == 1)
                {
                    added = true;
                }
            }
            catch(Exception ex)
            {
                throw new ApplicationException("Missed Question Not Added...", ex);
            }

            return added;
        }

        public bool EditQuizRecordIsPublicStatus(int quizRecordID, bool isPublic)
        {
            bool updated = false;

            int rowsAffected = 0;

            try
            {
                rowsAffected = _quizRecordAccessor.UpdateQuizRecordIsPublicStatus(quizRecordID, isPublic);
                if(rowsAffected == 1)
                {
                    updated = true;
                }
            }
            catch(Exception ex)
            {
                throw new ApplicationException("Public Status Not Updated...", ex);
            }

            return updated;
        }

        public List<MissedQuestion> GetActiveMissedQuestionsByQuizRecordID(int quizRecordID)
        {
            List<MissedQuestion> missedQuestions = null;

            try
            {
                missedQuestions = _quizRecordAccessor.SelectActiveMissedQuestionsByQuizRecordID(quizRecordID);
            }
            catch(Exception ex)
            {
                throw new ApplicationException("No questions found...", ex);
            }

            return missedQuestions;
        }

        public QuizRecordVM GetQuizRecordByID(int quizRecordID)
        {
            QuizRecordVM quizRecord = null;

            try
            {
                quizRecord = _quizRecordAccessor.SelectQuizRecordByID(quizRecordID);
            }
            catch(Exception ex)
            {
                throw new ApplicationException("Quiz Record Not Found...", ex);
            }

            return quizRecord;
        }
    }
}