namespace solution_MVC_Arthouse.DAL.SecurityEntities.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<solution_MVC_Arthouse.DAL.SecurityEntities.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"DAL\SecurityEntities\Migrations";
        }

        protected override void Seed(solution_MVC_Arthouse.DAL.SecurityEntities.ApplicationDbContext context)
        {
            //Create a Role Manager
            var roleManager = new RoleManager<IdentityRole>(new
                                          RoleStore<IdentityRole>(context));
            //Create Role Admin if it does not exist
            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                var roleresult = roleManager.Create(new IdentityRole("Admin"));
            }
            //Create Role Director if it does not exist
            if (!context.Roles.Any(r => r.Name == "Director"))
            {
                var roleresult = roleManager.Create(new IdentityRole("Director"));
            }

            //Create a User Manager
            var manager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(context));

            //Now the Admin user named admin1 with password password
            var adminuser = new ApplicationUser
            {
                UserName = "admin1@outlook.com",
                Email = "admin1@outlook.com"
            };

            //Assign admin user to role
            if (!context.Users.Any(u => u.UserName == "admin1@outlook.com"))
            {
                manager.Create(adminuser, "password");
                manager.AddToRole(adminuser.Id, "Admin");
            }

            //Now the Director user named director1 with password password
            var directoruser = new ApplicationUser
            {
                UserName = "director1@outlook.com",
                Email = "director1@outlook.com"
            };

            //Assign director user to role
            if (!context.Users.Any(u => u.UserName == "director1@outlook.com"))
            {
                manager.Create(directoruser, "password");
                manager.AddToRole(directoruser.Id, "Director");
            }

            //Create a few generic users
            for (int i = 1; i <= 4; i++)
            {
                var user = new ApplicationUser
                {
                    UserName = string.Format("user{0}@outlook.com", i.ToString()),
                    Email = string.Format("user{0}@outlook.com", i.ToString())
                };
                if (!context.Users.Any(u => u.UserName == user.UserName))
                    manager.Create(user, "password");
            }
        }
    }
}
