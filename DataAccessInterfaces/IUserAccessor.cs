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

        int InsertNewUser(User user);
        //int InsertNewUser(string givenName, string familyName, string email, string phoneNumber);
        int InserNewUserRole(UserRole userRole);
        //int InserNewUserRole(int userID, string roleID);
        int UpdateUserInformation(User user, User updatedUser);
        //int UpdateUserInformation(int userID, string newGivenName, string newFamilyName, string newEmail, string? newPhoneNumber,
        //                    bool newActive, DateTime? newReactivationDate); // I could only get this to work without checking the old values.
        //                                                                    // More comments in the UserAccessor implementation.
        int DeleteUserRole(UserRole userRole);
        //int DeleteUserRole(int userID, string roleID);

        UserVM SelectUserByUserID(int userID);
        int UpdateUserPasswordHash(string email, string currentPasswordHash, string newPasswordHash);
    }
}