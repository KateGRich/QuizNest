using DataDomain;
using LogicLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace QuizNestMVC.Controllers
{
    public class QuizController : Controller
    {
        // new to include identity manager classes
        private UserManager<IdentityUser> _identityUserManager;
        private SignInManager<IdentityUser> _signInManager;
        private RoleManager<IdentityRole> _roleManager;

        private readonly ILogger<HomeController> _logger;

        // changed to acquire identity manager classes (injected by request pipeline, no ninject needed)
        public QuizController(ILogger<HomeController> logger,
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

        List<UserVM> _users;

        IQuizManager _quizManager = new QuizManager();
        List<QuizVM> _quizzes;
        List<QuizTopic> _quizTopics;

        IQuestionManager _questionManager = new QuestionManager();

        IQuizRecordManager _quizRecordManager = new QuizRecordManager();
        List<QuizRecordVM>? _leaderboard;
        List<QuizRecordVM> _takenQuizzes;


        // All Quizzes Actions
        public ActionResult AllQuizzes(string searchString, string quizTopic)
        {
            getAccessToken();

            try
            {
                _quizzes = _quizManager.GetAllActiveQuizes();

                _quizzes = QuizFilters(_quizzes, searchString, quizTopic);

                return View(_quizzes);
            }
            catch(Exception ex)
            {
                string message = ex.InnerException == null ? ex.Message : ex.Message + "\n\n" + ex.InnerException.Message;
                ViewBag.ErrorMessage = message;
                return View("~/Shared/Error");
            }
        }
        public List<QuizVM> QuizFilters(List<QuizVM> quizzes, string searchString, string quizTopic)
        {
            getAllQuizTopics();

            if(!String.IsNullOrEmpty(searchString))
            {
                quizzes = quizzes.Where(q => q.Name.Contains(searchString)
                                       || q.Name.Contains(searchString)).Cast<QuizVM>().ToList();
            }
            if(!String.IsNullOrEmpty(quizTopic))
            {
                quizzes = quizzes.Where(q => q.QuizTopicID.Equals(quizTopic)
                                       || q.QuizTopicID.Equals(quizTopic)).Cast<QuizVM>().ToList();
            }

            return quizzes;
        }

        public ActionResult ViewLeaderboard(int id)
        {
            getAccessToken();

            try
            {
                // Get Quiz ID & Name either way.
                //     - ID in case they want to take the quiz (no existing records).
                //     - Name for display.
                QuizVM quiz = _quizManager.GetQuizByID(id);
                ViewBag.QuizID = id;
                ViewBag.QuizName = quiz.Name;

                // Get Leaderboard records.
                _leaderboard = _quizRecordManager.GetQuizLeaderboard(id);
                return View(_leaderboard);
            }
            catch(Exception ex)
            {
                string message = ex.InnerException == null ? ex.Message : ex.Message + "\n\n" + ex.InnerException.Message;
                ViewBag.ErrorMessage = message;
                return View("~/Shared/Error");
            }
        }

        // SURPRISE ME QUIZ
        public ActionResult SurpriseMe()
        {
            getAccessToken();

            try
            {
                // Random class for generating a random number.
                Random random = new Random(DateTime.Now.ToString().GetHashCode());
                
                // Get all quizzes.
                _quizzes = _quizManager.GetAllActiveQuizes();

                // Choose a random index that exists in the _quizzes list.
                int index = random.Next(0, _quizzes.Count);

                var quiz = _quizzes[index];

                return RedirectToAction("TakeQuiz", "Question", new { id = quiz.QuizID, questionNumber = 1, attemptType = "First" });
            }
            catch(Exception ex)
            {
                string message = ex.InnerException == null ? ex.Message : ex.Message + "\n\n" + ex.InnerException.Message;
                ViewBag.ErrorMessage = message;
                return View("~/Shared/Error");
            }
        }



        // My Quizzes Actions
        [Authorize(Roles = "Quiz Maker")]
        public ActionResult MyQuizzes()
        {
            getAccessToken();

            try
            {
                _quizzes = _quizManager.GetQuizzesByCreator(_accessToken.UserID);

                return View(_quizzes);
            }
            catch(Exception ex)
            {
                string message = ex.InnerException == null ? ex.Message : ex.Message + "\n\n" + ex.InnerException.Message;
                ViewBag.ErrorMessage = message;
                return View("~/Shared/Error");
            }
        }
        [Authorize(Roles = "Quiz Maker, Admin")]
        public ActionResult Details(int id)
        {
            getAccessToken();

            QuizVM quiz = null;
            try
            {
                quiz = _quizManager.GetQuizByID(id);
                return View(quiz);
            }
            catch(Exception ex)
            {
                string message = ex.InnerException == null ? ex.Message : ex.Message + "\n\n" + ex.InnerException.Message;
                ViewBag.ErrorMessage = message;
                return View("~/Shared/Error");
            }
        }

        [Authorize(Roles = "Quiz Maker, Admin")]
        public ActionResult Edit(int id)
        {
            getAccessToken();

            QuizVM quiz = null;
            try
            {
                getAllQuizTopics();
                quiz = _quizManager.GetQuizByID(id);
                return View(quiz);
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
        public ActionResult Edit(int id, QuizVM newQuiz)
        {
            getAccessToken();

            try
            {
                getAllQuizTopics();

                if(ModelState.IsValid)
                {
                    QuizVM oldQuiz = _quizManager.GetQuizByID(id);
                    if(User.IsInRole("Admin"))
                    {
                        newQuiz.QuizID = oldQuiz.QuizID;
                        newQuiz.QuizTopicID = oldQuiz.QuizTopicID;
                        newQuiz.QuizTopicDescription = oldQuiz.QuizTopicDescription;
                        newQuiz.Name = oldQuiz.Name;
                        newQuiz.Description = oldQuiz.Description;
                        newQuiz.CreatedBy = oldQuiz.CreatedBy;
                    }
                    getAndAddQuizTopic(newQuiz);

                    bool result = _quizManager.EditQuizInformation(oldQuiz, newQuiz);
                    if(result == false)
                    {
                        ViewBag.ErrorMessage = "Failed to update your quiz...";
                        return View("~/Shared/Error");
                    }
                    else
                    {
                        if(User.IsInRole("Admin"))
                        {
                            return RedirectToAction(nameof(AllQuizzes));
                        }
                        else
                        {
                            return RedirectToAction(nameof(MyQuizzes), new { id = _accessToken.UserID });
                        }
                    }
                }
                else
                {
                    // Server-side validation
                    return View(newQuiz);
                }
            }
            catch
            {
                return View();
            }
        }

        [Authorize(Roles = "Quiz Maker")]
        public ActionResult Create()
        {
            getAccessToken();

            getAllQuizTopics();

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Quiz Maker")]
        public ActionResult Create(QuizVM newQuiz)
        {
            getAccessToken();

            try
            {
                getAllQuizTopics();

                if(ModelState.IsValid)
                {
                    getAndAddQuizTopic(newQuiz);

                    newQuiz.CreatedBy = _accessToken.UserID;
                    int result = _quizManager.AddNewQuiz((Quiz)newQuiz);
                    if(result == 0)
                    {
                        ViewBag.ErrorMessage = "Failed to create your quiz...";
                        return View("~/Shared/Error");
                    }
                    else
                    {
                        return RedirectToAction(nameof(MyQuizzes), new { id = _accessToken.UserID });
                    }
                }
                else
                {
                    // Server-side validation
                    return View(newQuiz);
                }
            }
            catch
            {
                return View();
            }
        }

        private void getAndAddQuizTopic(QuizVM newQuiz)
        {
            // Get topics & descriptions.
            List<string> topics = new List<string>();
            List<string> descriptions = new List<string>();
            foreach(QuizTopic topic in _quizTopics)
            {
                topics.Add(topic.QuizTopicID);
                descriptions.Add(topic.Description);
            }

            if(topics.Contains(newQuiz.QuizTopicID))
            {

                // They chose an existing topic, so set the existing description for it.
                foreach(QuizTopic topic in _quizTopics)
                {
                    if(topic.QuizTopicID == newQuiz.QuizTopicID)
                    {
                        newQuiz.QuizTopicDescription = topic.Description;
                    }
                }
            }
            else
            {
                // They entered a new topic.

                if(descriptions.Contains(newQuiz.QuizTopicDescription))
                {
                    // They did not enter a new topic description - set it to empty.
                    newQuiz.QuizTopicDescription = "";
                }

                // Whether or not they entered a new topic description, add it to the DB.
                // No try/catch, as this method is always called within a try/catch.
                QuizTopic newQuizTopic = new QuizTopic()
                {
                    QuizTopicID = newQuiz.QuizTopicID,
                    Description = newQuiz.QuizTopicDescription
                };
                // No try/catch, as this method is always called within a try/catch.
                _quizManager.AddNewQuizTopic(newQuizTopic);
            }
        }
        private void getAllQuizTopics()
        {
            // No try/catch, as this method is always called within a try/catch.
            _quizTopics = _quizManager.GetAllQuizTopics();

            List<string> topics = new List<string>();
            List<string> descriptions = new List<string>();
            foreach(QuizTopic topic in _quizTopics)
            {
                topics.Add(topic.QuizTopicID);
                descriptions.Add(topic.Description);
            }

            ViewBag.QuizTopics = _quizTopics;
            ViewBag.Topics = topics;
            ViewBag.Descriptions = descriptions;
        }


        // View Taken Quizzes
        [Authorize(Roles = "Quiz Taker")]
        public ActionResult TakenQuizzes()
        {
            getAccessToken();

            try
            {
                _takenQuizzes = _quizRecordManager.GetTakenQuizzes(_accessToken.UserID);
                return View(_takenQuizzes);
            }
            catch(Exception ex)
            {
                string message = ex.InnerException == null ? ex.Message : ex.Message + "\n\n" + ex.InnerException.Message;
                ViewBag.ErrorMessage = message;
                return View("~/Shared/Error");
            }
        }

        [Authorize(Roles = "Quiz Taker")]
        public ActionResult UpdatePublicStatus(int id)
        {
            getAccessToken();

            QuizRecordVM quizRecord = null;

            try
            {
                quizRecord = _quizRecordManager.GetQuizRecordByID(id);
                return View(quizRecord);
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
        public ActionResult UpdatePublicStatus(QuizRecordVM quizRecord)
        {
            getAccessToken();

            try
            {
                if(ModelState.IsValid)
                {
                    bool result = _quizRecordManager.EditQuizRecordIsPublicStatus(quizRecord.QuizRecordID, quizRecord.IsPublic);
                    if(result == false)
                    {
                        ViewBag.ErrorMessage = "Failed to update your quiz record...";
                        return View("~/Shared/Error");
                    }
                    else
                    {
                        return RedirectToAction(nameof(TakenQuizzes));
                    }
                }
                else
                {
                    // Server-side validation
                    return View(quizRecord);
                }
            }
            catch
            {
                return View();
            }
        }
    }
}