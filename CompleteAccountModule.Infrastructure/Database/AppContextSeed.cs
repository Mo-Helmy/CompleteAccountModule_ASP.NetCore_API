using CompleteAccountModule.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompleteAccountModule.Infrastructure.Database
{
    public static class AppContextSeed
    {
        public static async Task AddSeedsAsync(AppDbContext dbContext, RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
        {
            #region Rols
            if (!roleManager.Roles.Any())
            {
                //await _roleManager.CreateAsync(new IdentityRole("USER"));
                await roleManager.CreateAsync(new IdentityRole("USER"));
                await roleManager.CreateAsync(new IdentityRole("ADMIN"));
                await roleManager.CreateAsync(new IdentityRole("SUPERADMIN"));

                //await dbContext.SaveChangesAsync();
            }
            #endregion

            #region Users
            if (!dbContext.Users.Any())
            {
                var users = new List<AppUser>()
                {

                    new AppUser() {Id = "user1", Email="user1@example.com", UserName = "username1" },
                    new AppUser() {Id = "user2", Email="user2@example.com", UserName = "username2" },
                    new AppUser() {Id = "admin1", Email="admin1@example.com", UserName = "admin1" },
                    new AppUser() {Id = "admin2", Email="admin2@example.com", UserName = "admin2" },
                    new AppUser() {Id = "admin3", Email="admin3@example.com", UserName = "admin3" },
                    new AppUser() {Id = "superadmin1", Email="superadmin1@example.com", UserName = "superadmin1"},
                };


                foreach (var user in users)
                {
                    await userManager.CreateAsync(user, "string123");

                    if (user.Id.Contains("user"))
                    {
                        await userManager.AddToRoleAsync(user, "USER");
                    }
                    else if (user.Id.Contains("superadmin"))
                    {
                        await userManager.AddToRoleAsync(user, "SUPERADMIN");
                    }
                    else if (user.Id.Contains("admin"))
                    {
                        await userManager.AddToRoleAsync(user, "ADMIN");
                    }
                }

            }
            #endregion

        }
    }
}

