using DataDomain;
using LogicLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace QuizNestMVC.Controllers
{
    public class QuestionController : Controller
    {
        // new to include identity manager classes
        private UserManager<IdentityUser> _identityUserManager;
        private SignInManager<IdentityUser> _signInManager;
        private RoleManager<IdentityRole> _roleManager;

        private readonly ILogger<HomeController> _logger;

        // changed to acquire identity manager classes (injected by request pipeline, no ninject needed)
        public QuestionController(ILogger<HomeController> logger,
               UserManager<IdentityUser> userManager,
               SignInManager<IdentityUser> signInManager)
        {
            // new to instantiate identity manager classes
            _identityUserManager = userManager;
            _signInManager = signInManager;

            _logger = logger;
        }

        // added to get a legacy logiclayer user manager and data domain userVM for an access token
        private IUserManager _userManager = new UserManager();
        private AccessToken _accessToken = new AccessToken("");

        private void getAccessToken()
        {
            if(_signInManager.IsSignedIn(User))
            {
                string email = User.Identity.Name;
                try
                {
                    _accessToken = new AccessToken(email);
                }
                catch { }
            }
            else
            {
                return;
            }
        }

        QuizVM _quiz;

        IQuizManager _quizManager = new QuizManager();
        IQuestionManager _questionManager = new QuestionManager();
        List<QuestionVM> _questions;
        List<MissedQuestion> _missedQuestions;
        
        IQuizRecordManager _quizRecordManager = new QuizRecordManager();
        List<QuestionVM> _answeredQuestions = new List<QuestionVM>();

        string? _attemptType;

        //bool _isReviewingMissedOnly = false;

        int _count = 0;


        public ActionResult ViewQuestion(int id, int questionNumber, bool enableEdit)
        {
            // Get all questions by the quiz's quizID.
            // Add all of these questions to the _questions list.

            getAccessToken();

            try
            {
                _questions = _questionManager.GetAllQuestionsByQuizID(id);
                var quiz = _quizManager.GetQuizByID(id);
                _count = questionNumber - 1;

                if(questionNumber == 0)
                {
                    ViewBag.QuizID = id;
                    ViewBag.QuizName = quiz.Name;
                    ViewBag.Questions = new List<QuestionVM>();
                    return View();
                }

                ViewBag.Count = _questions.Count();

                if(_questions.Count() == 0)
                {
                    ViewBag.QuizID = id;
                    ViewBag.QuizName = quiz.Name;
                    ViewBag.Questions = new List<QuestionVM>();
                    return View();
                }

                _questions[_count].QuestionNumber = questionNumber;
                _questions[_count].EnableEdit = enableEdit;

                ViewBag.QuizID = id;
                ViewBag.QuizName = quiz.Name;

                return View(_questions[_count]);
            }
            catch(Exception ex)
            {
                string message = ex.InnerException == null ? ex.Message : ex.Message + "\n\n" + ex.InnerException.Message;
                ViewBag.ErrorMessage = message;
                return View("~/Shared/Error");
            }
        }


        [Authorize(Roles = "Quiz Maker, Admin")]
        public ActionResult Edit(int id, int questionNumber)
        {
            getAccessToken();

            QuestionVM question = null;
            try
            {
                getAllQuestionTypes();
                question = _questionManager.GetQuestionByID(id);
                question.QuestionNumber = questionNumber;
                question.EnableEdit = true;
                getAllAnswerOptions(question);


                return View(question);
            }
            catch(Exception ex)
            {
                string message = ex.InnerException == null ? ex.Message : ex.Message + "\n\n" + ex.InnerException.Message;
                ViewBag.ErrorMessage = message;
                return View("~/Shared/Error");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Quiz Maker, Admin")]
        public ActionResult Edit(int id, int questionNumber, string? shortAnswer, QuestionVM newQuestion)
        {
            getAccessToken();

            if(newQuestion.QuestionTypeID == "Short Answer" && shortAnswer != null)
            {
                newQuestion.CorrectAnswer = shortAnswer.ToString();
            }

            try
            {
                getAllQuestionTypes();

                QuestionVM oldQuestion = _questionManager.GetQuestionByID(id);

                if(ModelState.IsValid)
                {
                    newQuestion.QuestionNumber = questionNumber;
                    newQuestion.EnableEdit = true;
                    newQuestion.QuestionID = oldQuestion.QuestionID;
                    newQuestion.QuizID = oldQuestion.QuizID;
                    newQuestion.QuizName = oldQuestion.QuizName;
                    
                    if(User.IsInRole("Admin"))
                    {
                        newQuestion.Prompt = oldQuestion.Prompt;
                        newQuestion.Answer1 = oldQuestion.Answer1;
                        newQuestion.Answer2 = oldQuestion.Answer2;
                        newQuestion.Answer3 = oldQuestion.Answer3;
                        newQuestion.Answer4 = oldQuestion.Answer4;
                        newQuestion.CorrectAnswer = oldQuestion.CorrectAnswer;
                    }

                    getAllAnswerOptions(newQuestion);

                    bool result = _questionManager.EditQuestionInformation(newQuestion);
                    if(result == false)
                    {
                        ViewBag.ErrorMessage = "Failed to update your question...";
                        return View("~/Shared/Error");
                    }
                    else
                    {
                        return RedirectToAction("ViewQuestion", new { id = newQuestion.QuizID, questionNumber = newQuestion.QuestionNumber, enableEdit = true  });
                    }
                }
                else
                {
                    // Server-side validation
                    return View(newQuestion);
                }
            }
            catch
            {
                return View();
            }
        }

        private void getAllQuestionTypes()
        {
            // No try/catch, as this method is always called within a try/catch.
            List<string> questionTypes = _quizManager.GetAllQuestionTypes();

            ViewBag.QuestionTypes = questionTypes;
        }
        private void getAllAnswerOptions(QuestionVM question)
        {
            List<string> answerOptions = new List<string>();
            if(question.QuestionTypeID == "Multiple Choice")
            {
                answerOptions.Add(question.Answer1);
                answerOptions.Add(question.Answer2);
                answerOptions.Add(question.Answer3);
                answerOptions.Add(question.Answer4);
            }
            if(question.QuestionTypeID == "True/False")
            {
                answerOptions.Add(question.Answer2);
                answerOptions.Add(question.Answer3);
            }

            ViewBag.AnswerOptions = answerOptions;
        }


        [Authorize(Roles = "Quiz Maker")]
        public ActionResult Create(int id)
        {
            getAccessToken();

            QuestionVM newQuestion = new QuestionVM();
            try
            {
                getAllQuestionTypes();
                QuizVM quiz = _quizManager.GetQuizByID(id);

                ViewBag.QuizName = quiz.Name;
                newQuestion.QuizID = id;


                return View(newQuestion);
            }
            catch(Exception ex)
            {
                string message = ex.InnerException == null ? ex.Message : ex.Message + "\n\n" + ex.InnerException.Message;
                ViewBag.ErrorMessage = message;
                return View("~/Shared/Error");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Quiz Maker")]
        public ActionResult Create(QuestionVM newQuestion, string? shortAnswer)
        {
            getAccessToken();

            if(newQuestion.QuestionTypeID == "Short Answer" && shortAnswer != null)
            {
                newQuestion.CorrectAnswer = shortAnswer.ToString();
            }

            try
            {
                getAllQuestionTypes();

                if(ModelState.IsValid)
                {
                    bool result = _questionManager.AddNewQuizQuestion(newQuestion);
                    if(result == false)
                    {
                        ViewBag.ErrorMessage = "Failed to create your question...";
                        return View("~/Shared/Error");
                    }
                    else
                    {
                        return RedirectToAction("MyQuizzes", "Quiz");
                    }
                }
                else
                {
                    // Server-side validation
                    return View(newQuestion);
                }
            }
            catch
            {
                return View();
            }
        }


        [Authorize(Roles = "Quiz Taker")]
        public ActionResult ViewMissedQuestion(int id, int questionNumber)
        {
            getAccessToken();

            // Get all missed questions by the QuizRecordID.
            // Add all of these questions to the _missedQuestions list.
            try
            {
                if(_attemptType == null)
                {
                    _count = questionNumber - 1;

                    _missedQuestions = _quizRecordManager.GetActiveMissedQuestionsByQuizRecordID(id);

                    if(_missedQuestions.Count() == 0)
                    {
                        MissedQuestion missedQuestion = new MissedQuestion();

                        return View(missedQuestion);
                    }
                    else
                    {
                        _missedQuestions[_count].QuestionNumber = questionNumber;
                        ViewBag.Count = _missedQuestions.Count();

                        return View(_missedQuestions[_count]);
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "Could not load questions...";
                    return View("~/Shared/Error");
                }
            }
            catch(Exception ex)
            {
                string message = ex.InnerException == null ? ex.Message : ex.Message + "\n\n" + ex.InnerException.Message;
                ViewBag.ErrorMessage = message;
                return View("~/Shared/Error");
            }
        }


        [Authorize(Roles = "Quiz Taker")]
        public ActionResult TakeQuiz(int id, int questionNumber, string attemptType)
        {
            // POST BACK & FORTH UNTIL THEY REACH THE END OF THE QUESTIONS.
            
            getAccessToken();

            // Get all questions by the quiz's quizID, so the user can take the quiz.
            // Add all of these questions to the _questions list.
            try
            {
                _attemptType = attemptType;

                getAllQuestionTypes();

                ViewBag.QuizID = id;

                _count = questionNumber - 1;

                _questions = _questionManager.GetActiveQuestionsByQuizID(id);

                _questions[_count].QuestionNumber = questionNumber;
                _questions[_count].AttemptTypeID = _attemptType;
                getAllAnswerOptions(_questions[_count]);

                ViewBag.Count = _questions.Count;

                return View(_questions[_count]);
            }
            catch(Exception ex)
            {
                string message = ex.InnerException == null ? ex.Message : ex.Message + "\n\n" + ex.InnerException.Message;
                ViewBag.ErrorMessage = message;
                return View("~/Shared/Error");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Quiz Taker")]
        public ActionResult TakeQuiz(QuestionVM answeredQuestion, string? backButton, string? nextButton, string? submitQuiz)
        {
            try
            {
                if(backButton != null)
                {
                    if(answeredQuestion.MyAnswer == null)
                    {
                        return RedirectToAction("TakeQuiz", new { id = answeredQuestion.QuizID, questionNumber = answeredQuestion.QuestionNumber - 1, attemptType = answeredQuestion.AttemptTypeID });
                    }
                }

                if(ModelState.IsValid)
                {
                    HttpContext.Session.SetString("A" + answeredQuestion.QuestionNumber,  answeredQuestion.MyAnswer);

                    if(backButton != null)
                    {
                        // Go To Previous Question
                        return RedirectToAction("TakeQuiz", new { id = answeredQuestion.QuizID, questionNumber = answeredQuestion.QuestionNumber - 1, attemptType = answeredQuestion.AttemptTypeID });

                    }
                    if(nextButton != null)
                    {
                        // Go To Next Question
                        return RedirectToAction("TakeQuiz", new { id = answeredQuestion.QuizID, questionNumber = answeredQuestion.QuestionNumber + 1, attemptType = answeredQuestion.AttemptTypeID });

                    }
                    if(submitQuiz != null)
                    {
                        List<string> answerList = new List<string>();

                        for(int i = 1; i < answeredQuestion.QuestionNumber + 1; i++)
                        {
                            answerList.Add(HttpContext.Session.GetString("A" + i));
                        }

                        // Submit Quiz & View Score
                        return RedirectToAction("ViewScore", new { quizID = answeredQuestion.QuizID, quizRecordID = 0, answers = answerList, attemptType = answeredQuestion.AttemptTypeID });
                    }

                    // Server-side validation
                    return View(answeredQuestion);
                }
                else
                {
                    // Server-side validation
                    return View(answeredQuestion);
                }
            }
            catch(Exception ex)
            {
                string message = ex.InnerException == null ? ex.Message : ex.Message + "\n\n" + ex.InnerException.Message;
                ViewBag.ErrorMessage = message;
                return RedirectToAction("Error", "Home");
            }
        }


        [Authorize(Roles = "Quiz Taker")]
        public ActionResult TakeMissedQuestions(int quizRecordID, int questionNumber)
        {
            // POST BACK & FORTH UNTIL THEY REACH THE END OF THE QUESTIONS.

            getAccessToken();

            // Get all questions by the record's quizRecordID, so the user can take the quiz.
            // Add all of these questions to the _questions list.
            try
            {
                _attemptType = "Missed Only";

                getAllQuestionTypes();

                ViewBag.QuizID = quizRecordID;

                _count = questionNumber - 1;

                _missedQuestions = _quizRecordManager.GetActiveMissedQuestionsByQuizRecordID(quizRecordID);

                _missedQuestions[_count].QuestionNumber = questionNumber;
                _missedQuestions[_count].AttemptTypeID = _attemptType;
                getAllAnswerOptions(_missedQuestions[_count]);

                ViewBag.Count = _missedQuestions.Count;

                return View(_missedQuestions[_count]);
            }
            catch(Exception ex)
            {
                string message = ex.InnerException == null ? ex.Message : ex.Message + "\n\n" + ex.InnerException.Message;
                ViewBag.ErrorMessage = message;
                return View("~/Shared/Error");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Quiz Taker")]
        public ActionResult TakeMissedQuestions(MissedQuestion answeredQuestion, string? backButton, string? nextButton, string? submitQuiz)
        {
            try
            {
                if(backButton != null)
                {
                    if(answeredQuestion.MyAnswer == null)
                    {
                        return RedirectToAction("TakeMissedQuestions", new { id = answeredQuestion.QuizID, questionNumber = answeredQuestion.QuestionNumber - 1 });
                    }
                }

                if(ModelState.IsValid)
                {
                    HttpContext.Session.SetString("A" + answeredQuestion.QuestionNumber, answeredQuestion.MyAnswer);

                    if(backButton != null)
                    {
                        // Go To Previous Question
                        return RedirectToAction("TakeMissedQuestions", new { quizRecordID = answeredQuestion.QuizRecordID, questionNumber = answeredQuestion.QuestionNumber - 1 });

                    }
                    if(nextButton != null)
                    {
                        // Go To Next Question
                        return RedirectToAction("TakeMissedQuestions", new { quizRecordID = answeredQuestion.QuizRecordID, questionNumber = answeredQuestion.QuestionNumber + 1 });

                    }
                    if(submitQuiz != null)
                    {
                        List<string> answerList = new List<string>();

                        for(int i = 1; i < answeredQuestion.QuestionNumber + 1; i++)
                        {
                            answerList.Add(HttpContext.Session.GetString("A" + i));
                        }

                        // Submit Quiz & View Score
                        return RedirectToAction("ViewScore", new { quizID = answeredQuestion.QuizID, quizRecordID = answeredQuestion.QuizRecordID, answers = answerList, attemptType = answeredQuestion.AttemptTypeID });
                    }

                    // Server-side validation
                    return View(answeredQuestion);
                }
                else
                {
                    // Server-side validation
                    return View(answeredQuestion);
                }
            }
            catch(Exception ex)
            {
                string message = ex.InnerException == null ? ex.Message : ex.Message + "\n\n" + ex.InnerException.Message;
                ViewBag.ErrorMessage = message;
                return View("~/Shared/Error");
            }
        }


        [Authorize(Roles = "Quiz Taker")]
        public ActionResult ViewScore(int quizID, int quizRecordID, List<string> answers, string attemptType)
        {
            getAccessToken();

            try
            {
                if(quizRecordID == 0 && attemptType != "Missed Only")
                {
                    _questions = _questionManager.GetActiveQuestionsByQuizID(quizID);

                    if(answers.Count == _questions.Count)
                    {
                        // Calculate their score - how many they got right, & how many they missed.

                        List<QuestionVM> missedQuestions = new List<QuestionVM>();

                        // How many question did they miss?
                        for(int i = 0; i < _questions.Count; i++)
                        {
                            // Count through, if their answer does NOT equal the correct answer,
                            // add it to missedQuestions.
                            if(_questions[i].CorrectAnswer.ToLower() != answers[i].ToLower())
                            {
                                missedQuestions.Add(_questions[i]);
                            }
                        }

                        // Divide number of correct questions by the quiz question count.
                        decimal score = (decimal)(_questions.Count - missedQuestions.Count) / (decimal)_questions.Count;

                        // Round, to calculate it to 2 decimal places.
                        score = Decimal.Round((score *= 100), 2);

                        // Insert their QuizRecord.
                        // Get the ID, in case they missed any questions.
                        QuizRecord newQuizRecord = new QuizRecord()
                        {
                            AttemptTypeID = attemptType,
                            UserID = _accessToken.UserID,
                            QuizID = quizID,
                            Score = score,
                            IsPublic = true
                        };

                        int newRecordID = addQuizRecord(newQuizRecord);

                        // If missedQuestions.Count > 0, then insert all missed questions into the DB.
                        if(missedQuestions.Count > 0)
                        {
                            foreach(Question q in missedQuestions)
                            {
                                // Add to DB.
                                addMissedQuestion(newRecordID, q);
                            }
                        }

                        ViewBag.MissedQuestions = missedQuestions.Count();

                        QuizRecordVM quizRecord = _quizRecordManager.GetQuizRecordByID(newRecordID);
                        return View(quizRecord);
                    }
                    else
                    {
                        return View();
                    }
                }
                else
                {
                    _missedQuestions = _quizRecordManager.GetActiveMissedQuestionsByQuizRecordID(quizRecordID);

                    if(answers.Count == _missedQuestions.Count)
                    {
                        // Calculate their score - how many they got right, & how many they missed.

                        List<MissedQuestion> newMissedQuestions = new List<MissedQuestion>();

                        // How many question did they miss?
                        for(int i = 0; i < _missedQuestions.Count; i++)
                        {
                            // Count through, if their answer does NOT equal the correct answer,
                            // add it to newMissedQuestions.
                            if(_missedQuestions[i].CorrectAnswer.ToLower() != answers[i].ToLower())
                            {
                                newMissedQuestions.Add(_missedQuestions[i]);
                            }
                        }

                        // Divide number of correct questions by the _missedQuestions.Count.
                        decimal score = (decimal)(_missedQuestions.Count - newMissedQuestions.Count) / (decimal)_missedQuestions.Count;

                        // Round, to calculate it to 2 decimal places.
                        score = Decimal.Round((score *= 100), 2);

                        // Insert their new QuizRecord.
                        // Get the ID, in case they missed any questions.
                        QuizRecord newQuizRecord = new QuizRecord()
                        {
                            AttemptTypeID = attemptType,
                            UserID = _accessToken.UserID,
                            QuizID = quizID,
                            Score = score,
                            IsPublic = true
                        };

                        int newRecordID = addQuizRecord(newQuizRecord);

                        // If newMissedQuestions.Count > 0, then insert all missed questions into the DB.
                        if(newMissedQuestions.Count > 0)
                        {
                            foreach(Question q in newMissedQuestions)
                            {
                                // Add to DB.
                                addMissedQuestion(newRecordID, q);
                            }
                        }

                        ViewBag.MissedQuestions = newMissedQuestions.Count();

                        QuizRecordVM quizRecord = _quizRecordManager.GetQuizRecordByID(newRecordID);
                        return View(quizRecord);
                    }
                    else
                    {
                        return View();
                    }
                }
            }
            catch(Exception ex)
            {
                string message = ex.InnerException == null ? ex.Message : ex.Message + "\n\n" + ex.InnerException.Message;
                ViewBag.ErrorMessage = message;
                return View("~/Shared/Error");
            }
        }


        private int addQuizRecord(QuizRecord newQuizRecord)
        {
            int newRecordID = 0;

            newRecordID = _quizRecordManager.AddQuizRecord(newQuizRecord);

            if(newRecordID == 0)
            {
                throw new Exception("Record Not Added...");
            }
            else
            {
                return newRecordID;
            }
        }
        private void addMissedQuestion(int newRecordID, Question q)
        {
            bool result = _quizRecordManager.AddMissedQuestion(newRecordID, q.QuestionID);
            if(result == false)
            {
                throw new Exception("Missed Question Not Recorded...");
            }
        }
        private void getAllAnswerOptions(MissedQuestion question)
        {
            List<string> answerOptions = new List<string>();
            if(question.QuestionTypeID == "Multiple Choice")
            {
                answerOptions.Add(question.Answer1);
                answerOptions.Add(question.Answer2);
                answerOptions.Add(question.Answer3);
                answerOptions.Add(question.Answer4);
            }
            if(question.QuestionTypeID == "True/False")
            {
                answerOptions.Add(question.Answer2);
                answerOptions.Add(question.Answer3);
            }

            ViewBag.AnswerOptions = answerOptions;
        }
    }
}