﻿using DataAccessInterfaces;
using DataAccessLayer;
using DataDomain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayer
{
    public class QuizManager : IQuizManager
    {
        private IQuizAccessor _quizAccessor;

        // Constructor for Tests
        public QuizManager(IQuizAccessor quizAccessor)
        {
            _quizAccessor = quizAccessor;
        }

        // Constructor for DB
        public QuizManager()
        {
            _quizAccessor = new QuizAccessor();
        }

        public List<QuizVM> GetQuizzesByCreator(int userID)
        {
            List<QuizVM> quizzes = null;

            try
            {
                quizzes = _quizAccessor.SelectQuizzesByCreator(userID);

                foreach(QuizVM quiz in quizzes)
                {
                    quiz.NumberOfQuestions = _quizAccessor.SelectTotalCountOfQuestionsByQuiz(quiz.QuizID);
                }
            }
            catch(Exception ex)
            {
                throw new ApplicationException("Something is very wrong...", ex);
            }

            return quizzes;
        }

        public List<QuizVM> GetAllActiveQuizes()
        {
            List<QuizVM> quizzes = null;

            try
            {
                quizzes = _quizAccessor.SelectAllActiveQuizzes();

                foreach(QuizVM quiz in quizzes)
                {
                    quiz.NumberOfQuestions = _quizAccessor.SelectCountOfActiveQuestionsByQuiz(quiz.QuizID);
                }
            }
            catch(Exception ex)
            {
                throw new ApplicationException("Something is very wrong...", ex);
            }

            return quizzes;
        }

        public List<QuizTopic> GetAllQuizTopics()
        {
            List<QuizTopic> quizTopics = null;

            try
            {
                quizTopics = _quizAccessor.SelectAllQuizTopics();
            }
            catch(Exception ex)
            {
                throw new ApplicationException("Something is very wrong...", ex);
            }

            return quizTopics;
        }

        public List<string> GetAllQuestionTypes()
        {
            List<string> questionTypes = null;

            try
            {
                questionTypes = _quizAccessor.SelectAllQuestionTypes();
            }
            catch(Exception ex)
            {
                throw new ApplicationException("Something is very wrong...", ex);
            }

            return questionTypes;
        }

        public bool AddNewQuizTopic(QuizTopic quizTopic)
        {
            bool added = false;

            int rowsAffected = 0;

            try
            {
                rowsAffected = _quizAccessor.InsertNewQuizTopic(quizTopic);
                if(rowsAffected == 1)
                {
                    added = true;
                }
            }
            catch(Exception ex)
            {
                throw new ApplicationException("New Quiz Topic Creation Failed...", ex);
            }

            return added;
        }

        public int AddNewQuiz(Quiz quiz)
        {
            int newQuizID = 0;

            try
            {
                newQuizID = _quizAccessor.InsertNewQuiz(quiz);
            }
            catch(Exception ex)
            {
                throw new ApplicationException("New Quiz Creation Failed...", ex);
            }

            return newQuizID;
        }

        public bool EditQuizInformation(Quiz quiz, Quiz newQuiz)
        {
            bool updated = false;

            int rowsAffected = 0;

            try
            {
                rowsAffected = _quizAccessor.UpdateQuizInformation(quiz, newQuiz);
                if(rowsAffected == 1)
                {
                    updated = true;
                }
            }
            catch(Exception ex)
            {
                throw new ApplicationException("Update Failed...", ex);
            }

            return updated;
        }

        public QuizVM GetQuizByID(int quizID)
        {
            QuizVM? quiz = null;

            try
            {
                quiz = _quizAccessor.SelectQuizByID(quizID);
            }
            catch(Exception ex)
            {
                throw new ApplicationException("Quiz Not Found...", ex);
            }

            return quiz;
        }
    }
}