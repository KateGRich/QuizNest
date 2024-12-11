using DataDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessInterfaces
{
    public interface IQuizRecordAccessor
    {
        List<QuizRecordVM> SelectQuizLeaderboard(int quizID);
        List<QuizRecordVM> SelectQuizzesByTaker(int userID);
        int InsertQuizRecord(string attemptType, int userID, int quizID, decimal score);
        int InsertMissedQuestion(int quizRecordID, int questionID);
        int UpdateQuizRecordIsPublicStatus(int quizRecordID, bool isPublic);

    }
}