using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SportsPro.Models;

namespace SportsPro
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var roleMgr = services.GetRequiredService<RoleManager<IdentityRole>>();
                var userMgr = services.GetRequiredService<UserManager<User>>();

                const string adminRole = "Admin";
                const string adminUser = "admin";
                const string adminPass = "P@ssw0rd";

                if (!roleMgr.RoleExistsAsync(adminRole).Result)
                {
                    var roleResult = roleMgr.CreateAsync(new IdentityRole(adminRole)).Result;
                    if (!roleResult.Succeeded)
                        throw new Exception($"Failed to create '{adminRole}' role");
                }

                var user = userMgr.FindByNameAsync(adminUser).Result;
                if (user == null)
                {
                    user = new User
                    {
                        UserName = adminUser,
                        Email = "admin@yourapp.com",  
                    };
                    var createResult = userMgr.CreateAsync(user, adminPass).Result;
                    if (!createResult.Succeeded)
                        throw new Exception($"Failed to create '{adminUser}' user: " +
                                            string.Join(", ", createResult.Errors));
                }

                if (!userMgr.IsInRoleAsync(user, adminRole).Result)
                {
                    var addToRoleResult = userMgr.AddToRoleAsync(user, adminRole).Result;
                    if (!addToRoleResult.Succeeded)
                        throw new Exception($"Failed to add '{adminUser}' to '{adminRole}' role");
                }
            }

            host.Run();

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
