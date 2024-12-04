using DataDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayer
{
    public interface IUserManager
    {
        string HashSHA256(string password);
        bool VerifyUser(string email, string password);
        User GetUserByEmail(string email);
        List<string> GetUserRoles(int userID);
        UserVM LogInUser(string email, string password);
        List<UserVM> GetAllUsers();
        bool AddNewUser(string givenName, string familyName, string email, string phoneNumber, List<string> roles);
        bool EditUserInformation(string newGivenName, string newFamilyName, string newEmail, string? newPhoneNumber,
                                bool newActive, DateTime? newReactivationDate, UserVM userEdit, List<string> newRoles);


        //public bool UpdatePassword(string email, string oldPassword, string newPassword);
    }
}