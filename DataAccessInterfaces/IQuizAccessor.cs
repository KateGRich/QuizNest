using DataDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessInterfaces
{
    public interface IQuizAccessor
    {
        List<QuizVM> SelectQuizzesByCreator(int userID);
        int SelectTotalCountOfQuestionsByQuiz(int quizID);
        List<QuizVM> SelectAllActiveQuizzes();
    }
}