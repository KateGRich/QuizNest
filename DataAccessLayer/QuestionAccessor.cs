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
        public List<QuestionVM> SelectAllQuestionsByQuizID(int quizID)
        {
            List<QuestionVM> questions = new List<QuestionVM>();

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
                    questions.Add(new QuestionVM()
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
                        Active = reader.GetBoolean(10),
                        QuizName = reader.GetString(11)
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

        public int InsertNewQuizQuestion(Question question)
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

            cmd.Parameters["@QuestionTypeID"].Value = question.QuestionTypeID;
            cmd.Parameters["@QuizID"].Value = question.QuizID;
            cmd.Parameters["@Prompt"].Value = question.Prompt;
            cmd.Parameters["@Answer1"].Value = question.Answer1 == null ? DBNull.Value : question.Answer1;
            cmd.Parameters["@Answer2"].Value = question.Answer2 == null ? DBNull.Value : question.Answer2;
            cmd.Parameters["@Answer3"].Value = question.Answer3 == null ? DBNull.Value : question.Answer3;
            cmd.Parameters["@Answer4"].Value = question.Answer4 == null ? DBNull.Value : question.Answer4;
            cmd.Parameters["@CorrectAnswer"].Value = question.CorrectAnswer;

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

        public int UpdateQuestionInformation(Question question)
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

            cmd.Parameters["@QuestionID"].Value = question.QuestionID;
            cmd.Parameters["@newQuestionTypeID"].Value = question.QuestionTypeID;
            cmd.Parameters["@QuizID"].Value = question.QuizID;
            cmd.Parameters["@newPrompt"].Value = question.Prompt;
            cmd.Parameters["@newAnswer1"].Value = (object)question.Answer1 ?? DBNull.Value;
            cmd.Parameters["@newAnswer2"].Value = (object)question.Answer2 ?? DBNull.Value;
            cmd.Parameters["@newAnswer3"].Value = (object)question.Answer3 ?? DBNull.Value;
            cmd.Parameters["@newAnswer4"].Value = (object)question.Answer4 ?? DBNull.Value;
            cmd.Parameters["@newCorrectAnswer"].Value = question.CorrectAnswer;
            cmd.Parameters["@newActive"].Value = question.Active;

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

        public List<QuestionVM> SelectActiveQuestionsByQuizID(int quizID)
        {
            List<QuestionVM> questions = new List<QuestionVM>();

            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_select_active_questions_by_quizID", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@QuizID", SqlDbType.Int);
            cmd.Parameters["@QuizID"].Value = quizID;
            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();
                while(reader.Read())
                {
                    questions.Add(new QuestionVM()
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
                        Active = reader.GetBoolean(10),
                        QuizName = reader.GetString(11)
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

        public QuestionVM SelectQuestionByID(int questionID)
        {
            QuestionVM question = new QuestionVM();

            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_select_question_by_questionID", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@QuestionID", SqlDbType.Int);
            cmd.Parameters["@QuestionID"].Value = questionID;
            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();
                if(reader.HasRows)
                {
                    reader.Read();
                    question = new QuestionVM()
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
                        Active = reader.GetBoolean(10),
                        QuizName = reader.GetString(11)
                    };
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
            return question;
        }
    }
}