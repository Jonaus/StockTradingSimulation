using NUnit.Framework;
using StockTradingSimulationAPI.Models;
using StockTradingSimulationAPI.Tests.Models;
using System.Collections.Generic;
using System.Linq;

namespace StockTradingSimulationAPI.Tests.Tests
{
    [TestFixture]
    public class AccountTests : TestBase
    {
        [TestCase("Testas2", "Testas", "Testas", "", true)]
        [TestCase("Testas3", "Testas", "Testas2", "", false)]
        [TestCase("Testas3", "", "", "", false)]
        public void RegisterTest(string username, string password, string confirmPassword, string email,
            bool shouldCreate)
        {
            var users = APIClient.Get<IEnumerable<User>>(AdminToken, "api/users");
            var createdUser = APIClient.Post<User>("", "api/account/register", new
            {
                Username = username,
                Password = password,
                ConfirmPassword = confirmPassword,
                Email = email
            });

            if (shouldCreate)
            {
                var users2 = APIClient.Get<IEnumerable<User>>(AdminToken, "api/users");
                Assert.True(users.Count() + 1 == users2.Count(), "User was not created");

                // CLEANUP
                APIClient.Delete<string>(AdminToken, $"api/users/{createdUser.Id}");
            }

            var users3 = APIClient.Get<IEnumerable<User>>(AdminToken, "api/users");
            Assert.True(users.Count() == users3.Count(), "Expected to not see more users.");
        }

        [TestCase("administrator", "administrator123", ExpectedResult = true)]
        [TestCase("administrator", "asdasd", ExpectedResult = false)]
        [TestCase("testas", "testas", ExpectedResult = true)]
        [TestCase("testas", "asdasd", ExpectedResult = false)]
        [TestCase("testas", "", ExpectedResult = false)]
        public bool LoginTest(string username, string password)
        {
            var token = APIClient.Post<Token>("", "api/account/login", new
            {
                Username = username,
                Password = password
            });

            return !string.IsNullOrEmpty(token.access_token);
        }
    }
}
