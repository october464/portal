using Finportal.Data;
using Finportal.Enum;
using Finportal.Models;
using Finportal.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Finportal.Helper
{
    public class DataHelper
    {
        public static string GetConnectionString(IConfiguration configuration)
        {
            //The default connection string will come from appSettings Like usual
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            //It will be automatically overwritten if we are running on Heroku
            var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
            return string.IsNullOrEmpty(databaseUrl) ? connectionString : BuildConnectionString(databaseUrl);
        }

        public static string BuildConnectionString(string databaseUrl)
        {
            //Provides an object representation of a uniform resource identifier (URI) and easy access to the parts of the URI.
            var databaseUri = new Uri(databaseUrl);
            var userInfo = databaseUri.UserInfo.Split(':');

            //Provides a simple way to create and manage the contents of connection strings used by the NpgsqlConnection class.
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseUri.LocalPath.TrimStart('/')
            };
            return builder.ToString();
        }

        //Handles the Migration Programmatic
        public static async Task ManageDataAsync(IHost host)
        {
            try
            {
                //This technique is used to obtain references to services
                //Normally I would just inject these services but you cant use a constructor in a static class.
                using var svcScope = host.Services.CreateScope();
                var svcProvider = svcScope.ServiceProvider;

                //The services  will run your migrations
                var dbContextSvc = svcProvider.GetRequiredService<ApplicationsDbContext>();
                await dbContextSvc.Database.MigrateAsync();

                var userManager = svcProvider.GetRequiredService<UserManager<FPUser>>();
                var roleManager = svcProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var imageService = svcProvider.GetRequiredService < IImageService>();
                await ContextSeed.SeedRolesAsync(roleManager);
                await ContextSeed.SeedDefaultUserAsync(userManager, imageService);
                //var settings = svcProvider.GetRequiredService<IOptions<AdminSettings>>();

                //await SeedDataAsync(seedConfigSvc);
            }
            catch (PostgresException ex)
            {
                Console.WriteLine($"Exception while running Manage Data => {ex}");
            }
        }

        //public static async Task SeedDataAsync(SeedConfiguration seedSvc)
        //{

        //}

        //public static async Task SeedUsersAsync(SeedConfiguration seedSvc)
        //{

        //}

        //public static async Task SeedRolesAsync(SeedConfiguration seedSvc)
        //{
        //    //I am leveragaing the PortalRole Enum to help eliminate "magic strings" in our application
        //    await seedSvc.RoleManager.CreateAsync(new IdentityRole(PortalRole.Admin.ToString()));
        //    await seedSvc.RoleManager.CreateAsync(new IdentityRole(PortalRole.HOH.ToString()));
        //    await seedSvc.RoleManager.CreateAsync(new IdentityRole(PortalRole.Member.ToString()));

        //}
    }
}
