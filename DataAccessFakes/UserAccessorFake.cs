//using DataAccessInterfaces;
//using DataDomain;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace DataAccessFakes
//{
//    public class UserAccessorFake : IUserAccessor
//    {
//        private List<User> _users;
//        private List<UserRole> _userRoles;
//        public UserAccessorFake()
//        {
//            _users = new List<User>();
//            _userRoles = new List<UserRole>();

//            _users.Add(new UserVM()
//            {
//                UserID = 1,
//                GivenName = "Peggy",
//                FamilyName = "Hill",
//                Email = "test1@test.com",
//                PhoneNumber = "1234567890",
//                PasswordHash = "b03ddf3ca2e714a6548e7495e2a03f5e824eaac9837cd7f159c67b90fb4b7342",
//                ReactivationDate = null,
//                Active = true
//            });
//            _users.Add(new UserVM()
//            {
//                UserID = 2,
//                GivenName = "Test2",
//                FamilyName = "Test2",
//                Email = "test2@test.com",
//                PhoneNumber = "1234567890",
//                PasswordHash = "b03ddf3ca2e714a6548e7495e2a03f5e824eaac9837cd7f159c67b90fb4b7342",
//                ReactivationDate = null,
//                Active = true
//            });
//            _users.Add(new UserVM()
//            {
//                UserID = 3,
//                GivenName = "Test3",
//                FamilyName = "Test3",
//                Email = "test3@test.com",
//                PhoneNumber = "1234567890",
//                PasswordHash = "b03ddf3ca2e714a6548e7495e2a03f5e824eaac9837cd7f159c67b90fb4b7342",
//                ReactivationDate = null,
//                Active = true
//            });
//            _users.Add(new UserVM()
//            {
//                UserID = 4,
//                GivenName = "Test4",
//                FamilyName = "Test4",
//                Email = "test4@test.com",
//                PhoneNumber = "1234567890",
//                PasswordHash = "b03ddf3ca2e714a6548e7495e2a03f5e824eaac9837cd7f159c67b90fb4b7342",
//                ReactivationDate = null,
//                Active = false
//            });

//            _userRoles.Add(new UserRole()
//            {
//                UserID = 1,
//                RoleID = "Admin"
//            });
//            _userRoles.Add(new UserRole()
//            {
//                UserID = 1,
//                RoleID = "Quiz Maker"
//            });
//            _userRoles.Add(new UserRole()
//            {
//                UserID = 2,
//                RoleID = "Quiz Maker"
//            });
//            _userRoles.Add(new UserRole()
//            {
//                UserID = 3,
//                RoleID = "Quiz Taker"
//            });
//            _userRoles.Add(new UserRole()
//            {
//                UserID = 4,
//                RoleID = "Quiz Taker"
//            });
//        }

//        public int SelectUserCountByEmailAndPasswordHash(string email, string passwordHash)
//        {
//            int count = 0;
//            foreach(var user in _users)
//            {
//                if(user.Email == email && user.PasswordHash == passwordHash && user.Active == true)
//                {
//                    count++;
//                }
//            }
//            return count;
//        }
//        public UserVM SelectUserByEmail(string email)
//        {
//            foreach(UserVM user in _users)
//            {
//                if(user.Email == email)
//                {
//                    return user;
//                }
//            }
//            throw new ArgumentException("User Record Not Found.");
//        }
//        public List<string> SelectRolesByUserID(int userID)
//        {
//            List<string> roles = new List<string>();
//            foreach(var userRole in _userRoles)
//            {
//                if(userRole.UserID == userID)
//                {
//                    roles.Add(userRole.RoleID);
//                }
//            }
//            return roles;
//        }

//        public List<UserVM> SelectAllUsers()
//        {
//            throw new NotImplementedException();
//        }

//        public int InsertNewUser(string givenName, string familyName, string email, string phoneNumber)
//        {
//            throw new NotImplementedException();
//        }

//        public int InserNewUserRole(int userID, string roleID)
//        {
//            throw new NotImplementedException();
//        }

//        public int UpdateUserInformation(string oldGivenName, string newGivenName, string oldFamilyName, string newFamilyName, string oldEmail, string newEmail, string? oldPhoneNumber, string? newPhoneNumber, bool oldActive, bool newActive, DateTime? oldReactivationDate, DateTime? newReactivationDate)
//        {
//            throw new NotImplementedException();
//        }

//        public int DeleteUserRole(int userID, string roleID)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}