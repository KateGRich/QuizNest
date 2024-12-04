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
    }
}