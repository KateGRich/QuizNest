using DataDomain;
using LogicLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace QuizNestMVC.Controllers
{
    public class ChatController : Controller
    {
        // new to include identity manager classes
        private UserManager<IdentityUser> _identityUserManager;
        private SignInManager<IdentityUser> _signInManager;
        private RoleManager<IdentityRole> _roleManager;

        private readonly ILogger<HomeController> _logger;

        // changed to acquire identity manager classes (injected by request pipeline, no ninject needed)
        public ChatController(ILogger<HomeController> logger,
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

        IChatManager _chatManager = new ChatManager();
        List<ChatType> _chatTypes;
        List<ChatVM> _chats;

        List<User> _adminUsers;

        public ActionResult MyChats()
        {
            getAccessToken();
            try
            {
                if(User.IsInRole("Admin"))
                {
                    _chats = _chatManager.GetReceivedChats(_accessToken.UserID);
                }
                else
                {
                    _chats = _chatManager.GetStartedChats(_accessToken.UserID);
                }

                return View(_chats);
            }
            catch(Exception ex)
            {
                string message = ex.InnerException == null ? ex.Message : ex.Message + "\n\n" + ex.InnerException.Message;
                ViewBag.ErrorMessage = message;
                return View("~/Shared/Error");
            }
        }


        public ActionResult ViewChat(int id, string recipient, string chatTopic)
        {
            getAccessToken();

            try
            {
                var messages = _chatManager.GetMessagesByChatID(id);

                ViewBag.SenderID = _accessToken.UserID;
                ViewBag.Recipient = recipient;
                ViewBag.ChatTopic = chatTopic;
                ViewBag.Messages = messages;

                return View();
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
        public ActionResult Reply(Message message)
        {
            getAccessToken();

            try
            {
                if(ModelState.IsValid)
                {
                    bool result = _chatManager.AddNewMessage(message);
                    if(result == false)
                    {
                        ViewBag.ErrorMessage = "Failed to send your message...";
                        return View("~/Shared/Error");
                    }
                    else
                    {
                        return RedirectToAction(nameof(ViewChat), new { id = message.ChatID });
                    }
                }
                else
                {
                    // Server-side validation
                    return View();
                }
            }
            catch
            {
                return View();
            }
        }


        [Authorize(Roles = "Quiz Taker, Quiz Maker")]
        public ActionResult NewChat()
        {
            getAccessToken();

            getChatTypes();
            getAdmins();
            ViewBag.UserID = _accessToken.UserID;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Quiz Taker, Quiz Maker")]
        public ActionResult NewChat(ChatVM newChat)
        {
            getAccessToken();

            try
            {
                getChatTypes();

                if(ModelState.IsValid)
                {
                    //newChat.Originator = _accessToken.UserID;
                    bool result = _chatManager.AddNewChat(newChat);
                    if(result == false)
                    {
                        ViewBag.ErrorMessage = "Failed to send your chat...";
                        return View("~/Shared/Error");
                    }
                    else
                    {
                        return RedirectToAction(nameof(MyChats), new { id = _accessToken.UserID });
                    }
                }
                else
                {
                    // Server-side validation
                    return View(newChat);
                }
            }
            catch
            {
                return View();
            }
        }

        public void getAdmins()
        {
            // No try/catch, as this method is always called within a try/catch.
            _adminUsers = _chatManager.GetAllAdmins();

            List<string> names = new List<string>();

            foreach(var adminUser in _adminUsers)
            {
                names.Add(adminUser.GivenName + " " + adminUser.FamilyName);
            }

            ViewBag.Admins = _adminUsers;
            ViewBag.Names = names;
        }
        public void getChatTypes()
        {
            // No try/catch, as this method is always called within a try/catch.
            _chatTypes = _chatManager.GetAllChatTypes();

            List<string> types = new List<string>();
            List<string> descriptions = new List<string>();
            foreach(ChatType type in _chatTypes)
            {
                types.Add(type.ChatTypeID);
                descriptions.Add(type.Description);
            }

            ViewBag.Types = types;
            ViewBag.Descriptions = descriptions;
        }
    }
}