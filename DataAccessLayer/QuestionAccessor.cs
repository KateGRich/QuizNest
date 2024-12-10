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
    public class QuestionAccessor : IQuestionAccessor
    {
        public List<Question> SelectAllQuestionsByQuizID(int quizID)
        {
            List<Question> questions = new List<Question>();

            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_select_all_questions_by_quizID", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@QuizID", SqlDbType.Int);
            cmd.Parameters["@QuizID"].Value = quizID;
            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();
                while(reader.Read())
                {
                    questions.Add(new Question()
                    {
                        QuestionID = reader.GetInt32(0),
                        QuestionTypeID = reader.GetString(1),
                        QuizID = reader.GetInt32(2),
                        Prompt = reader.GetString(3),
                        Answer1 = reader.IsDBNull(4) ? null : reader.GetString(4),
                        Answer2 = reader.IsDBNull(5) ? null : reader.GetString(5),
                        Answer3 = reader.IsDBNull(6) ? null : reader.GetString(6),
                        Answer4 = reader.IsDBNull(7) ? null : reader.GetString(7),
                        CorrectAnswer = reader.GetString(8),
                        CreatedOn = reader.GetDateTime(9),
                        Active = reader.GetBoolean(10)
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
            return questions;
        }

        public int InsertNewQuizQuestion(string questionTypeID, int quizID, string prompt, string answer1,
                string answer2, string answer3, string answer4, string correctAnswer)
        {
            int result = 0;

            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_insert_new_question", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@QuestionTypeID", SqlDbType.NVarChar, 50);
            cmd.Parameters.Add("@QuizID", SqlDbType.Int);
            cmd.Parameters.Add("@Prompt", SqlDbType.NVarChar, 250);
            cmd.Parameters.Add("@Answer1", SqlDbType.NVarChar, 250);
            cmd.Parameters.Add("@Answer2", SqlDbType.NVarChar, 250);
            cmd.Parameters.Add("@Answer3", SqlDbType.NVarChar, 250);
            cmd.Parameters.Add("@Answer4", SqlDbType.NVarChar, 250);
            cmd.Parameters.Add("@CorrectAnswer", SqlDbType.NVarChar, 250);

            cmd.Parameters["@QuestionTypeID"].Value = questionTypeID;
            cmd.Parameters["@QuizID"].Value = quizID;
            cmd.Parameters["@Prompt"].Value = prompt;
            cmd.Parameters["@Answer1"].Value = answer1;
            cmd.Parameters["@Answer2"].Value = answer2;
            cmd.Parameters["@Answer3"].Value = answer3;
            cmd.Parameters["@Answer4"].Value = answer4;
            cmd.Parameters["@CorrectAnswer"].Value = correctAnswer;

            try
            {
                conn.Open();
                result = cmd.ExecuteNonQuery();
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }

            return result;
        }

        public int UpdateQuestionInformation(int questionID, string newQuestionTypeID, int quizID, string newPrompt,
                        string newAnswer1, string newAnswer2, string newAnswer3, string newAnswer4,
                        string newCorrectAnswer, bool newActive)
        {
            int result = 0;

            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_update_question", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@QuestionID", SqlDbType.Int);
            cmd.Parameters.Add("@newQuestionTypeID", SqlDbType.NVarChar, 50);
            cmd.Parameters.Add("@QuizID", SqlDbType.Int);
            cmd.Parameters.Add("@newPrompt", SqlDbType.NVarChar, 250);
            cmd.Parameters.Add("@newAnswer1", SqlDbType.NVarChar, 250);
            cmd.Parameters.Add("@newAnswer2", SqlDbType.NVarChar, 250);
            cmd.Parameters.Add("@newAnswer3", SqlDbType.NVarChar, 250);
            cmd.Parameters.Add("@newAnswer4", SqlDbType.NVarChar, 250);
            cmd.Parameters.Add("@newCorrectAnswer", SqlDbType.NVarChar, 250);
            cmd.Parameters.Add("@newActive", SqlDbType.Bit);

            cmd.Parameters["@QuestionID"].Value = questionID;
            cmd.Parameters["@newQuestionTypeID"].Value = newQuestionTypeID;
            cmd.Parameters["@QuizID"].Value = quizID;
            cmd.Parameters["@newPrompt"].Value = newPrompt;
            cmd.Parameters["@newAnswer1"].Value = (object)newAnswer1 ?? DBNull.Value;
            cmd.Parameters["@newAnswer2"].Value = (object)newAnswer2 ?? DBNull.Value;
            cmd.Parameters["@newAnswer3"].Value = (object)newAnswer3 ?? DBNull.Value;
            cmd.Parameters["@newAnswer4"].Value = (object)newAnswer4 ?? DBNull.Value;
            cmd.Parameters["@newCorrectAnswer"].Value = newCorrectAnswer;
            cmd.Parameters["@newActive"].Value = newActive;

            try
            {
                conn.Open();
                result = cmd.ExecuteNonQuery();
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }

            return result;
        }
    }
}