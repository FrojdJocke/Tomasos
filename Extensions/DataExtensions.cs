using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using TomasosASP.Models;

namespace TomasosASP.Extensions
{
    public static class DataExtensions
    {
        public static IHost SeedData(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var manager = scope.ServiceProvider.GetRequiredService<SignInManager<User>>();
                //var context = serviceProvider.GetService<TomasosContext>();

                var user = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = "Admin",
                    NormalizedUserName = "ADMIN",
                    Email = "admin@example.com",
                    NormalizedEmail = "ADMIN@EXAMPLE.COM",
                    Name = "Foo Bar"
                };
                var createResult = manager.UserManager.CreateAsync(user, "qwerty123").Result;

                if (createResult.Succeeded)
                {
                    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                    var adminExist = roleManager.RoleExistsAsync("ADMIN").Result;
                    if (!adminExist)
                        roleManager.CreateAsync(new IdentityRole { Name = "Admin", NormalizedName = "ADMIN" });

                    var roleResult = manager.UserManager.AddToRoleAsync(user, "Admin").Result;
                }
            }
            return host;
        }
    }
}
