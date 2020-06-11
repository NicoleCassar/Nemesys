using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Nemesys.Models
{
    public class DbInitializer
    {
        public static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.Roles.Any())
            {
                roleManager.CreateAsync(new IdentityRole("Reporter")).Wait();
                roleManager.CreateAsync(new IdentityRole("Investigator")).Wait();
            }
        }

        public static void SeedUsers(UserManager<ApplicationUser> userManager)
        {
            if (!userManager.Users.Any())
            {
                var reporter = new ApplicationUser()
                {
                    IdNum = "123498M",
                    Email = "reporter@test.com",
                    NormalizedEmail = "REPORTER@TEST.COM",
                    UserName = "reporter@test.com",
                    NormalizedUserName = "REPORTER@TEST.COM",
                    Name = "John",
                    Surname = "Doe",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString("D") //to track important profile updates (e.g. password change)
                };

                //Add to store
                IdentityResult result = userManager.CreateAsync(reporter, "NemesysTEST1!").Result;
                if (result.Succeeded)
                {
                    //Add to role
                    userManager.AddToRoleAsync(reporter, "Reporter").Wait();
                }


                var investigator = new ApplicationUser()
                {
                    IdNum = "432190M",
                    Email = "investigator@test.com",
                    NormalizedEmail = "INVESTIGATOR@TEST.COM",
                    UserName = "investigator@test.com",
                    NormalizedUserName = "INVESTIGATOR@TEST.COM",
                    Name = "Peter",
                    Surname = "Doe",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString("D") //to track important profile updates (e.g. password change)
                };

                //Add to store
                result = userManager.CreateAsync(investigator, "NemesysTEST2!").Result;
                if (result.Succeeded)
                {
                    //Add to role
                    userManager.AddToRoleAsync(investigator, "Investigator").Wait();
                }
            }
        }

        /*
        public static void SeedData(UserManager<ApplicationUser> userManager, AppDbContext context)
        {
            if (!context.Report.Any())
            {
                var reporter = userManager.GetUsersInRoleAsync("Reporter").Result.FirstOrDefault();

                context.AddRange(
                    new Report()
                    {
                        // fill data
                    }
                ); ;

                context.SaveChanges();
            }
        }
        */
}
}
