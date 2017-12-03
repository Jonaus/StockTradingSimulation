using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using StockTradingSimulationAPI.Models;
using StockTradingSimulationAPI.Tests.Models;

namespace StockTradingSimulationAPI.Tests.Tests
{
    class UserTests : TestBase
    {
        private User _user;
        private string _userToken;
        private const string TestUsername = "NunitTestUser";

        [SetUp]
        public void Setup()
        {
            const string password = "Password";

            _user = APIClient.Post<User>(AdminToken, "api/account/register", new
            {
                Username = TestUsername,
                Password = password,
                ConfirmPassword = password
            });

            _userToken = APIClient.Post<Token>("", "api/account/login", new
            {
                Username = TestUsername,
                Password = password
            }).access_token;
        }

        [TearDown]
        public void Teardown()
        {
            APIClient.Delete<string>(AdminToken, $"api/users/{_user.Id}");
        }

        [TestCase(TestUsername, ExpectedResult = true)]
        [TestCase("User321", ExpectedResult = false)]
        [TestCase("", ExpectedResult = false)]
        public bool UserDeleteTest(string username)
        {
            var user = APIClient.Get<IEnumerable<User>>(AdminToken, "api/users")
                .FirstOrDefault(u => u.UserName == username);
            if (user == null) return false;
            
            var users = APIClient.Get<IEnumerable<User>>(AdminToken, "api/users");
            APIClient.Delete<string>(AdminToken, $"api/users/{user.Id}");
            var users2 = APIClient.Get<IEnumerable<User>>(AdminToken, "api/users");

            return users.Count() - 1 == users2.Count();
        }

        [TestCase("administrator", 500, ExpectedResult = true)]
        [TestCase("administrator2", -500, ExpectedResult = false)]
        [TestCase("testas", -500, ExpectedResult = true)]
        public bool SendMoneyTest(string userName, float amount)
        {
            var user = APIClient.Get<IEnumerable<User>>(AdminToken, "api/users")
                .FirstOrDefault(u => u.UserName == userName);
            if (user == null) return false;

            var userBalance = APIClient.Get<float>(AdminToken, $"api/users/{user.Id}/balance");
            APIClient.Post<string>(AdminToken, $"api/users/{user.Id}/balance", new
            {
                Amount = amount
            });
            var userBalance2 = APIClient.Get<float>(AdminToken, $"api/users/{user.Id}/balance");

            // 10f tolerence for floating point calculation or if user balance changes during test
            return Math.Abs(userBalance + amount - userBalance2) < 10;
        }

        [TestCase(TestUsername, ExpectedResult = true)]
        [TestCase("administrator", ExpectedResult = false)]
        public bool UserAccessTest(string userName)
        {
            var users = APIClient.Get<IEnumerable<User>>(AdminToken, "api/users");
            var user = users.FirstOrDefault(u => u.UserName == userName);
            if (user == null) return false;

            var userData = APIClient.Get<User>(_userToken, $"api/users/{user.Id}");

            return userData != null;
        }
    }
}
