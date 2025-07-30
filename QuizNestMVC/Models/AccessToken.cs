using System.ComponentModel.DataAnnotations;
using LogicLayer;
using DataDomain;

// this class was created to allow an easy way to get a legacy access token for use on pages
namespace WebApplication1.Models
{
    public class AccessToken
    {
        User _legacyUser;
        List<string> _roles;

        public AccessToken(string email)
        {
            if(email == "" || email == null)
            {
                _legacyUser = new User()
                {
                    UserID = 0,
                    GivenName = "",
                    FamilyName = "",
                    Email = "",
                    PhoneNumber = ""
                };
                return;
            }

            UserManager userManager = new UserManager();
            _legacyUser = userManager.GetUserByEmail(email);
            _roles = userManager.GetUserRoles(_legacyUser.UserID);
        }

        public bool IsSet { get { return _legacyUser.UserID != 0; } }
        public int UserID { get { return _legacyUser.UserID; } }
        public string GivenName { get { return _legacyUser.GivenName; } }
        public string FamilyName { get { return _legacyUser.FamilyName; } }
        public string Email { get { return _legacyUser.Email; } }
        public string PhoneNumber { get { return _legacyUser.PhoneNumber; } }
        public List<String> Roles { get { return _roles; } }
    }
}
