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
    }
}