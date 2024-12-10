using DataDomain;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayer
{
    public interface IQuestionManager
    {
        List<Question> GetAllQuestionsByQuizID(int quizID);
        bool EditQuestionInformation(int questionID, string newQuestionTypeID, int quizID, string newPrompt,
                        string newAnswer1, string newAnswer2, string newAnswer3, string newAnswer4,
                        string newCorrectAnswer, bool newActive);
    }
}