namespace mono.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using mono.Models;
    using mono.DAL;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<mono.DAL.MonoDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(mono.DAL.MonoDbContext context)
        {
            /*
            var ph = new PasswordHasher();


            var adminRole = new IdentityRole("admin");
            var userRole = new IdentityRole("user");

            context.Roles.AddOrUpdate(
                adminRole,
                userRole
            );

            ApplicationUser adminUser = new ApplicationUser() { UserName = "admin", PasswordHash = ph.HashPassword("test123") };
            ApplicationUser userUser = new ApplicationUser() { UserName = "user", PasswordHash = ph.HashPassword("test123") };                      

            var adminUserRole = new IdentityUserRole() { Role = adminRole, User = adminUser};
            var userUserRole = new IdentityUserRole() { Role = userRole, User = userUser };

            adminUser.Roles.Add(adminUserRole);
            userUser.Roles.Add(userUserRole);

            context.Users.AddOrUpdate(
                adminUser,
                userUser
            );
            */

            var userManager = new UserManager<User>(new UserStore<User>(new MonoDbContext()));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new MonoDbContext()));

            var adminUser = new User() { UserName = "admin", FirstName = "admin", LastName = "adminic", Email = "d.d@gmail.com" };

            userManager.Create(adminUser, "admin123");

            roleManager.Create(new IdentityRole("admin"));
            roleManager.Create(new IdentityRole("restaurant"));
            roleManager.Create(new IdentityRole("user"));

            userManager.AddToRole(adminUser.Id, "admin");

            base.Seed(context);
         }
    }
}
