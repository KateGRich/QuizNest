using DataDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessInterfaces
{
    public interface IUserAccessor
    {
        int SelectUserCountByEmailAndPasswordHash(string email, string passwordHash);
        UserVM SelectUserByEmail(string email);
        List<string> SelectRolesByUserID(int userID);
        List<UserVM> SelectAllUsers();
        int InsertNewUser(string givenName, string familyName, string email, string phoneNumber);
        int InserNewUserRole(int userID, string roleID);

        // I could only get this to work without checking the old values.
        // More comments in the UserAccessor implementation.
        int UpdateUserInformation(int userID, string newGivenName, string newFamilyName, string newEmail, string? newPhoneNumber,
                            bool newActive, DateTime? newReactivationDate);
        int DeleteUserRole(int userID, string roleID);
    }
}