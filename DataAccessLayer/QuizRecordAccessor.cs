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
    public class QuizRecordAccessor : IQuizRecordAccessor
    {
        public List<QuizRecordVM> SelectQuizLeaderboard(int quizID)
        {
            List<QuizRecordVM> leaderboard = new List<QuizRecordVM>();

            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_select_quiz_leaderboard", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@QuizID", SqlDbType.Int);
            cmd.Parameters["@QuizID"].Value = quizID;
            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();
                while(reader.Read())
                {
                    leaderboard.Add(new QuizRecordVM()
                    {
                        QuizRecordID = reader.GetInt32(0),
                        AttemptTypeID = reader.GetString(1),
                        UserID = reader.GetInt32(2),
                        GivenName = reader.GetString(3),
                        FamilyName = reader.GetString(4),
                        QuizID = reader.GetInt32(5),
                        QuizName = reader.GetString(6),
                        Score = reader.GetDecimal(7),
                        DateTaken = reader.GetDateTime(8),
                        IsPublic = reader.GetBoolean(9)
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
            return leaderboard;
        }

        public List<QuizRecordVM> SelectQuizzesByTaker(int userID)
        {
            List<QuizRecordVM> takenQuizzes = new List<QuizRecordVM>();

            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_select_quizzes_by_taker", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@UserID", SqlDbType.Int);
            cmd.Parameters["@UserID"].Value = userID;
            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();
                while(reader.Read())
                {
                    takenQuizzes.Add(new QuizRecordVM()
                    {
                        QuizRecordID = reader.GetInt32(0),
                        QuizID = reader.GetInt32(1),
                        QuizTopicID = reader.GetString(2),
                        QuizName = reader.GetString(3),
                        Score = reader.GetDecimal(4),
                        AttemptTypeID = reader.GetString(5),
                        DateTaken = reader.GetDateTime(6),
                        IsPublic = reader.GetBoolean(7)
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
            return takenQuizzes;
        }

        public int InsertQuizRecord(QuizRecord quizRecord)
        {
            int newRecordID = 0;

            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_insert_new_quizRecord", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@AttemptTypeID", SqlDbType.NVarChar, 50);
            cmd.Parameters.Add("@UserID", SqlDbType.Int);
            cmd.Parameters.Add("@QuizID", SqlDbType.Int);
            cmd.Parameters.Add("@Score", SqlDbType.Decimal);

            cmd.Parameters["@AttemptTypeID"].Value = quizRecord.AttemptTypeID;
            cmd.Parameters["@UserID"].Value = quizRecord.UserID;
            cmd.Parameters["@QuizID"].Value = quizRecord.QuizID;
            cmd.Parameters["@Score"].Value = quizRecord.Score;
            try
            {
                conn.Open();
                var result = cmd.ExecuteScalar();
                newRecordID = Convert.ToInt32(result);
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }

            return newRecordID;
        }

        public int InsertMissedQuestion(int quizRecordID, int questionID)
        {
            int result = 0;

            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_insert_missed_question", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@QuizRecordID", SqlDbType.Int);
            cmd.Parameters.Add("@QuestionID", SqlDbType.Int);

            cmd.Parameters["@QuizRecordID"].Value = quizRecordID;
            cmd.Parameters["@QuestionID"].Value = questionID;

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

        public int UpdateQuizRecordIsPublicStatus(int quizRecordID, bool isPublic)
        {
            int result = 0;

            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_update_quizRecord_isPublic_status", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@QuizRecordID", SqlDbType.Int);
            cmd.Parameters.Add("@IsPublic", SqlDbType.Bit);

            cmd.Parameters["@QuizRecordID"].Value = quizRecordID;
            cmd.Parameters["@IsPublic"].Value = isPublic;

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

        public QuizRecordVM SelectQuizRecordByID(int quizRecordID)
        {
            QuizRecordVM quizRecord = null;

            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_select_quizRecord_by_quizRecordID", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@QuizRecordID", SqlDbType.Int);
            cmd.Parameters["@QuizRecordID"].Value = quizRecordID;
            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();
                if(reader.HasRows)
                {
                    reader.Read();
                    quizRecord = new QuizRecordVM()
                    {
                        QuizRecordID = reader.GetInt32(0),
                        QuizID = reader.GetInt32(1),
                        QuizTopicID = reader.GetString(2),
                        QuizName = reader.GetString(3),
                        Score = reader.GetDecimal(4),
                        AttemptTypeID = reader.GetString(5),
                        DateTaken = reader.GetDateTime(6),
                        IsPublic = reader.GetBoolean(7)
                    };
                }
                else
                {
                    throw new ArgumentException("Quiz Record Not Found...");
                }
            }
            catch(Exception ex)
            {
                throw new ApplicationException("No Record Found...", ex);
            }

            return quizRecord;
        }

        public List<MissedQuestion> SelectActiveMissedQuestionsByQuizRecordID(int quizRecordID)
        {
            List<MissedQuestion> missedQuestions = new List<MissedQuestion>();

            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_select_active_missed_questions_by_quizRecordID", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@QuizRecordID", SqlDbType.Int);
            cmd.Parameters["@QuizRecordID"].Value = quizRecordID;
            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();
                while(reader.Read())
                {
                    missedQuestions.Add(new MissedQuestion()
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
                        QuizName = reader.GetString(9),
                        AttemptTypeID = reader.GetString(10),
                        QuizRecordID = reader.GetInt32(11)
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
            return missedQuestions;
        }
    }
}