using DataDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayer
{
    public interface IQuizRecordManager
    {
        List<QuizRecordVM> GetQuizLeaderboard(int quizID);
        List<QuizRecordVM> GetTakenQuizzes(int userID);
        int AddQuizRecord(QuizRecord quizRecord);
        bool AddMissedQuestion(int quizRecordID, int questionID);
        bool EditQuizRecordIsPublicStatus(int quizRecordID, bool isPublic);
        List<MissedQuestion> GetActiveMissedQuestionsByQuizRecordID(int quizRecordID);
        QuizRecordVM GetQuizRecordByID(int quizRecordID);
    }
}