namespace mono.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using mono.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<mono.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(mono.Models.ApplicationDbContext context)
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
            
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
            var adminUser = new ApplicationUser() { UserName = "admin", FirstName = "ImeA", LastName = "PrezimeA", Email = "test@monoCathering.com" };
            var restoran1User = new ApplicationUser() { UserName = "restoran1", FirstName = "Ime1R", LastName = "Prezime1R", Email = "restoran1@restorani.hr", Restaurant = "Restoran1", Address = "adresa1 11", Phone = "031 111 111" };
            var restoran2User = new ApplicationUser() { UserName = "restoran2", FirstName = "Ime2R", LastName = "Prezime2R", Email = "restoran2@restorani.hr", Restaurant = "Restoran2", Address = "adresa2 22", Phone = "031 222 222" };
            var user1User = new ApplicationUser() { UserName = "korisnik1", FirstName = "Ime1", LastName = "Prezime1", Email = "korisnik1@korisnici.hr"};
            var user2User = new ApplicationUser() { UserName = "korisnik2", FirstName = "Ime2", LastName = "Prezime2", Email = "korisnik2@korisnici.hr" };

            userManager.Create(adminUser, "admin123");
            userManager.Create(restoran1User, "restoran1");
            userManager.Create(restoran2User, "restoran2");
            userManager.Create(user1User, "korisnik1");
            userManager.Create(user2User, "korisnik2");

            roleManager.Create(new IdentityRole("admin"));
            roleManager.Create(new IdentityRole("restaurant"));
            roleManager.Create(new IdentityRole("user"));

            userManager.AddToRole(adminUser.Id, "admin");
            userManager.AddToRole(restoran1User.Id, "restaurant");
            userManager.AddToRole(restoran2User.Id, "restaurant");
            userManager.AddToRole(user1User.Id, "user");
            userManager.AddToRole(user2User.Id, "user");

            base.Seed(context);
        }
    }
}
