using DataAccessInterfaces;
using DataDomain;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class QuizAccessor : IQuizAccessor
    {
        public List<QuizVM> SelectQuizzesByCreator(int userID)
        {
            List<QuizVM> quizzes = new List<QuizVM>();

            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_select_quizzes_by_creator", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@UserID", SqlDbType.Int);
            cmd.Parameters["@UserID"].Value = userID;
            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();
                while(reader.Read())
                {
                    quizzes.Add(new QuizVM()
                    {
                        QuizID = reader.GetInt32(0),
                        QuizTopicID = reader.GetString(1),
                        Name = reader.GetString(2),
                        Description = reader.IsDBNull(3) ? null : reader.GetString(3),
                        CreatedOn = reader.GetDateTime(4),
                        Active = reader.GetBoolean(5)
                    });
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }
            return quizzes;
        }

        public int SelectTotalCountOfQuestionsByQuiz(int quizID)
        {
            int count = 0;

            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_select_total_count_of_questions_by_quiz", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@QuizID", SqlDbType.Int);
            cmd.Parameters["@QuizID"].Value = quizID;
            try
            {
                conn.Open();
                var result = cmd.ExecuteScalar();
                count = Convert.ToInt32(result);
            }
            catch(Exception ex)
            {
                throw new ApplicationException("No Questions Found...", ex);
            }
            return count;
        }
        public List<QuizVM> SelectAllActiveQuizzes()
        {
            List<QuizVM> quizzes = new List<QuizVM>();

            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_select_all_active_quizzes", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();
                while(reader.Read())
                {
                    quizzes.Add(new QuizVM()
                    {
                        QuizID = reader.GetInt32(0),
                        QuizTopicID = reader.GetString(1),
                        Name = reader.GetString(2),
                        GivenName = reader.GetString(3),
                        FamilyName = reader.GetString(4),
                        CreatedBy = reader.GetInt32(5),
                        Description = reader.IsDBNull(6) ? null : reader.GetString(6),
                        CreatedOn = reader.GetDateTime(7),
                        Active = reader.GetBoolean(8)
                    });
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }
            return quizzes;
        }
    }
}