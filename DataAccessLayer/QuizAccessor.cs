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

        public List<QuizTopic> SelectAllQuizTopics()
        {
            List<QuizTopic> quizTopics = new List<QuizTopic>();

            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_select_all_quiz_topics", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();
                while(reader.Read())
                {
                    quizTopics.Add( new QuizTopic
                        {
                            QuizTopicID = reader.GetString(0),
                            Description = reader.IsDBNull(1) ? null : reader.GetString(1),
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
            return quizTopics;
        }

        public List<string> SelectAllQuestionTypes()
        {
            List<string> questionTypes = new List<string>();

            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_select_all_question_types", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();
                while(reader.Read())
                {
                    questionTypes.Add(reader.GetString(0)); // Only returning 1 value, which is the QuestionTypeID.
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
            return questionTypes;
        }

        public int InsertNewQuizTopic(string quizTopic, string description)
        {
            int result = 0;

            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_insert_new_quiz_topic", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@QuizTopicID", SqlDbType.NVarChar, 50);
            cmd.Parameters.Add("@Description", SqlDbType.NVarChar, 250);
            cmd.Parameters["@QuizTopicID"].Value = quizTopic;
            cmd.Parameters["@Description"].Value = description;
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

        public int InsertNewQuiz(string quizTopicID, string name, int userID, string description)
        {
            int newQuizID = 0;

            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_insert_new_quiz", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@QuizTopicID", SqlDbType.NVarChar, 50);
            cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 50);
            cmd.Parameters.Add("@CreatedBy", SqlDbType.Int);
            cmd.Parameters.Add("@Description", SqlDbType.NVarChar, 250);
            cmd.Parameters["@QuizTopicID"].Value = quizTopicID;
            cmd.Parameters["@Name"].Value = name;
            cmd.Parameters["@CreatedBy"].Value = userID;
            cmd.Parameters["@Description"].Value = description;
            try
            {
                conn.Open();
                var result = cmd.ExecuteScalar();
                newQuizID = Convert.ToInt32(result);
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }

            return newQuizID;
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
    }
}