using DataAccessFakes;
using LogicLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayerTests
{
    [TestClass]
    public class UserManagerTests
    {
        private IUserManager? _userManager;

        [TestInitialize]
        public void InitializeTest()
        {
            _userManager = new UserManager(new UserAccessorFake());
        }

        [TestMethod]
        public void TestHashSHA256ReturnsCorrectResult()
        {
            // Arrange
            const string valueToHash = "P@ssw0rd";
            const string expectedHash = "b03ddf3ca2e714a6548e7495e2a03f5e824eaac9837cd7f159c67b90fb4b7342";
            var actualHash = "";

            // Act
            actualHash = _userManager.HashSHA256(valueToHash);

            // Assert
            Assert.AreEqual(expectedHash, actualHash);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestGetHashSHA256ThrowsAnArgumentExceptionForEmptyString()
        {
            // Arrange
            const string valueToHash = "";

            // Act
            _userManager.HashSHA256(valueToHash);

            // Assert
            // Nothing to do - Looking for exception.
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestGetHashSHA256ThrowsAnArgumentExceptionForNull()
        {
            // Arrange
            const string? valueToHash = null;

            // Act
            _userManager.HashSHA256(valueToHash);

            // Assert
            // Nothing to do - Looking for exception.
        }

        [TestMethod]
        public void TestVerifyUserReturnsTrueForCorrectEmailAndPassword()
        {
            // Arrange
            const string email = "test1@test.com";
            const string password = "P@ssw0rd";
            const bool expectedResult = true; // Supposed to pass
            bool actualResult = false;

            // Act
            actualResult = _userManager.VerifyUser(email, password);

            // Assert
            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void TestVerifyUserReturnsFalseForIncorrectEmail()
        {
            // Arrange
            const string email = "bad1@test.com";
            const string password = "P@ssw0rd";
            const bool expectedResult = false; // Supposed to fail
            bool actualResult = true;

            // Act
            actualResult = _userManager.VerifyUser(email, password);

            // Assert
            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void TestVerifyUserReturnsFalseForIncorrectPassword()
        {
            // Arrange
            const string email = "test2@test.com";
            const string password = "badPassword";
            const bool expectedResult = false; // Supposed to fail
            bool actualResult = true;

            // Act
            actualResult = _userManager.VerifyUser(email, password);

            // Assert
            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void TestVerifyUserReturnsFalseForInactiveUser()
        {
            // Arrange
            const string email = "test4@test.com";
            const string password = "P@ssw0rd";
            const bool expectedResult = false; // Supposed to fail
            bool actualResult = true;

            // Act
            actualResult = _userManager.VerifyUser(email, password);

            // Assert
            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void TestGetUserRolesReturnsCorrectList()
        {
            // Arrange
            const int userID = 1;
            const int expectedRoleCount = 2;
            int actualRoleCount = 0;
            // Act
            actualRoleCount = _userManager.GetUserRoles(userID).Count;
            // Assert
            Assert.AreEqual(expectedRoleCount, actualRoleCount);
        }
    }
}