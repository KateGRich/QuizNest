using DataDomain;
using LogicLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using NuGet.Protocol.Plugins;
using WebApplication1.Models;

namespace QuizNestMVC.Controllers
{
    public class UserController : Controller
    {
        // new to include identity manager classes
        private UserManager<IdentityUser> _identityUserManager;
        private SignInManager<IdentityUser> _signInManager;
        private RoleManager<IdentityRole> _roleManager;

        private readonly ILogger<HomeController> _logger;

        // changed to acquire identity manager classes (injected by request pipeline, no ninject needed)
        public UserController(ILogger<HomeController> logger,
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

        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            getAccessToken();

            ViewBag.AccessToken = _accessToken;

            try
            {
                _users = _userManager.GetAllUsers();
                return View(_users);
            }
            catch(Exception ex)
            {
                string message = ex.InnerException == null ? ex.Message : ex.Message + "\n\n" + ex.InnerException.Message;
                ViewBag.ErrorMessage = message;
                return View("~/Shared/Error");
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Details(int id)
        {
            getAccessToken();

            try
            {
                UserVM user = _userManager.GetUserByUserID(id);
                return View(user);
            }
            catch(Exception ex)
            {
                string message = ex.InnerException == null ? ex.Message : ex.Message + "\n\n" + ex.InnerException.Message;
                ViewBag.ErrorMessage = message;
                return View("~/Shared/Error");
            }
        }

        public ActionResult MyProfile()
        {
            getAccessToken();

            try
            {
                UserVM user = _userManager.GetUserByUserID(_accessToken.UserID);
                return View(user);
            }
            catch(Exception ex)
            {
                string message = ex.InnerException == null ? ex.Message : ex.Message + "\n\n" + ex.InnerException.Message;
                ViewBag.ErrorMessage = message;
                return View("~/Shared/Error");
            }
        }

        public ActionResult Edit(int id)
        {
            getAccessToken();

            ViewBag.LoggedInUserID = _accessToken.UserID;

            UserVM user = null;
            try
            {
                user = _userManager.GetUserByUserID(id);

                return View(user);
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
        public ActionResult Edit(int id, UserVM newUser, string? admin, string? quizMaker, string? quizTaker)
        {
            getAccessToken();

            try
            {
                if(ModelState.IsValid)
                {
                    // Get the current info for the user's whose account is being updated.
                    UserVM oldUser = _userManager.GetUserByUserID(id);

                    if(_accessToken.UserID == newUser.UserID)
                    {
                        // User is updating their own information.

                        newUser.Roles = oldUser.Roles;

                        bool result = _userManager.EditUserInformation(oldUser, newUser, newUser.Roles);
                        if(result == false)
                        {
                            ViewBag.ErrorMessage = "Failed to update your information...";
                            return View("~/Shared/Error");
                        }
                        else
                        {
                            return RedirectToAction(nameof(MyProfile), new { id = _accessToken.UserID });
                        }
                    }
                    else
                    {
                        // An admin is editting another user's information.

                        // Assign roles based on the new values passed back.
                        List<string> newRoles = new List<string>();

                        if(admin != null)
                        {
                            newRoles.Add("Admin");
                        }
                        if(quizMaker != null)
                        {
                            newRoles.Add("Quiz Maker");
                        }
                        if(quizTaker != null)
                        {
                            newRoles.Add("Quiz Taker");
                        }

                        // If no role values were passed back, assign them the lowest level role.
                        if(admin == null && quizMaker == null && quizTaker == null)
                        {
                            newRoles.Add("Quiz Taker");
                        }

                        bool result = _userManager.EditUserInformation(oldUser, newUser, newRoles);
                        if(result == false)
                        {
                            ViewBag.ErrorMessage = "Failed to update user's information...";
                            return View("~/Shared/Error");
                        }
                        else
                        {
                            return RedirectToAction(nameof(Index));
                        }
                    }
                }
                else
                {
                    // Server-side validation
                    return View(newUser);
                }
            }
            catch
            {
                return View();
            }
        }

        [Authorize(Roles = "Quiz Taker, Quiz Maker")]
        public ActionResult Deactivate()
        {
            getAccessToken();

            try
            {
                // Save current AccessToken into a new UserVM variable.
                UserVM user = new UserVM()
                {
                    UserID = _accessToken.UserID,
                    GivenName = _accessToken.GivenName,
                    FamilyName = _accessToken.FamilyName,
                    Email = _accessToken.Email,
                    PhoneNumber = _accessToken.PhoneNumber,
                    Roles = _accessToken.Roles
                };

                // Reuse this method to explicitly save their current info, but set their Active status to false & their ReactivationDate to null.
                User updatedUser = new User()
                {
                    GivenName = _accessToken.GivenName,
                    FamilyName = _accessToken.FamilyName,
                    Email = _accessToken.Email,
                    PhoneNumber = _accessToken.PhoneNumber,
                    Active = false,
                    ReactivationDate = null
                };
                
                bool result = _userManager.EditUserInformation(user, updatedUser, user.Roles);
                if(result == false)
                {
                    ViewBag.ErrorMessage = "Failed to delete your account...";
                    return View("~/Shared/Error");
                }
                else
                {
                    // LOG THEM OUT
                    _signInManager.SignOutAsync();

                    // Send them back to the home page.
                    return RedirectToAction("Index", "Home");
                }
            }
            catch(Exception ex)
            {
                string message = ex.InnerException == null ? ex.Message : ex.Message + "\n\n" + ex.InnerException.Message;
                ViewBag.ErrorMessage = message;
                return View("~/Shared/Error");
            }
        }




        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            getAccessToken();

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create(User user, string? admin, string? quizMaker, string? quizTaker)
        {
            getAccessToken();

            try
            {
                // Admin is creating a new User.

                if(ModelState.IsValid)
                {
                    // Get list of new user roles to pass when adding new user account.
                    List<string> roles = new List<string>();

                    if(admin != null)
                    {
                        roles.Add("Admin");
                    }
                    if(quizMaker != null)
                    {
                        roles.Add("Quiz Maker");
                    }
                    if(quizTaker != null)
                    {
                        roles.Add("Quiz Taker");
                    }

                    // If admin forgot to check any role boxes, assign new user the lowest level role.
                    if(admin == null && quizMaker == null && quizTaker == null)
                    {
                        roles.Add("Quiz Taker");
                    }

                    bool result = _userManager.AddNewUser(user, roles);
                    if(result == false)
                    {
                        ViewBag.ErrorMessage = "Failed to create new user account...";
                        return View("~/Shared/Error");
                    }
                    else
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
                else
                {
                    // Server-side validation
                    return View(user);
                }
            }
            catch(Exception ex)
            {
                string message = ex.InnerException == null ? ex.Message : ex.Message + "\n\n" + ex.InnerException.Message;
                ViewBag.ErrorMessage = message;
                return View("~/Shared/Error");
            }
        }
    }
}