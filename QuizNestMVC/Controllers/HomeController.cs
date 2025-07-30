using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuizNestMVC.Models;
using System.Diagnostics;
using LogicLayer;
using WebApplication1.Models;

namespace QuizNestMVC.Controllers
{
    public class HomeController : Controller
    {
        // new to include identity manager classes
        private UserManager<IdentityUser> _identityUserManager;
        private SignInManager<IdentityUser> _signInManager;
        private RoleManager<IdentityRole> _roleManager;

        private readonly ILogger<HomeController> _logger;

        // changed to acquire identity manager classes (injected by request pipeline, no ninject needed)
        public HomeController(ILogger<HomeController> logger,
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

        public async Task<IActionResult> Index()
        {
            getAccessToken();
            if(_accessToken.IsSet)
            {

                var id = _identityUserManager.GetUserId(User);
                var users = _identityUserManager.Users;
                var u = users.First(u => u.Id == id);

                foreach(var role in _accessToken.Roles)
                {
                    if(!User.IsInRole(role))
                    {
                        await _identityUserManager.AddToRoleAsync(u, role);
                    }
                }
            }

            return RedirectToAction("Welcome");
        }


        public IActionResult Welcome()
        {
            getAccessToken();
            if(_accessToken.IsSet)
            {
                ViewBag.Name = _accessToken.GivenName + " " +
                    _accessToken.FamilyName + "!";
            }
            return View("Welcome");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}