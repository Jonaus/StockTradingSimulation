using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using Microsoft.AspNet.Identity.EntityFramework;
using StockTradingSimulationAPI.Models;

namespace StockTradingSimulationAPI.Core
{
    public class Configuration : DbMigrationsConfiguration<TradingContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(TradingContext context)
        {
            if (!context.Stocks.Any())
            {
                context.Stocks.Add(new Stock
                {
                    Fullname = "Apple",
                    Ticker = "AAPL"
                });
            }

            string adminRoleId;
            string userRoleId;
            if (!context.Roles.Any())
            {
                adminRoleId = context.Roles.Add(new IdentityRole(Roles.Admin)).Id;
                userRoleId = context.Roles.Add(new IdentityRole(Roles.User)).Id;
            }
            else
            {
                adminRoleId = context.Roles.First(c => c.Name == Roles.Admin).Id;
                userRoleId = context.Roles.First(c => c.Name == Roles.User).Id;
            }

            context.SaveChanges();

            if (!context.Users.Any())
            {
                var administrator = context.Users.Add(new User
                {
                    UserName = "administrator",
                    Email = "admin@local.com",
                    EmailConfirmed = true,
                    RegisterDatetime = DateTime.UtcNow
                });
                administrator.Roles.Add(new IdentityUserRole
                {
                    RoleId = adminRoleId
                });

                var standardUser = context.Users.Add(new User
                {
                    UserName = "suser",
                    Email = "user@local.com",
                    EmailConfirmed = true,
                    RegisterDatetime = DateTime.UtcNow
                });
                standardUser.Roles.Add(new IdentityUserRole
                {
                    RoleId = userRoleId
                });

                context.SaveChanges();

                var store = new MyUserStore();
                store.SetPasswordHashAsync(administrator, new MyUserManager().PasswordHasher.HashPassword("administrator123"));
                store.SetPasswordHashAsync(standardUser, new MyUserManager().PasswordHasher.HashPassword("user123"));
            }

            context.SaveChanges();
        }
    }

    public static class Roles
    {
        public const string Admin = "Administrator";
        public const string User = "User";

        public static IEnumerable<string> All()
        {
            return new List<string> {Admin, User};
        }
    }
}