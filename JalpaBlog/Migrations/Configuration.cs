namespace JalpaBlog.Migrations
{
    using System.Data.Entity.Migrations;
    using System.Linq;
    using JalpaBlog.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;

    internal sealed class Configuration : DbMigrationsConfiguration<JalpaBlog.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(JalpaBlog.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.

            #region roleManager
            var roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(context));

            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }

            if (!context.Roles.Any(r => r.Name == "Moderator"))
            {
                roleManager.Create(new IdentityRole { Name = "Moderator" });
            }
            #endregion

            //I need to create a few users that will eventually occupy the roles of either Admin or Moderator

            var userManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(context));

            if(!context.Users.Any(u => u.Email == "JPatel@Mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "JPatel@Mailinator.com",
                    Email = "JPatel@Mailinator.com",
                    FirstName = "Jalpa",
                    LastName = "Patel",
                    DisplayName = "Jpatel"
                }, "Abc&123!");
            }
            if (!context.Users.Any(u => u.Email == "Moderator@Mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "Moderator@Mailinator.com",
                    Email = "Moderator@Mailinator.com",
                    FirstName = "Jason",
                    LastName = "Twichell",
                    DisplayName = "Twich"
                }, "Abc&123!");
            }

            var userId = userManager.FindByEmail("JPatel@Mailinator.com").Id;
            userManager.AddToRole(userId, "Admin");

             userId = userManager.FindByEmail("Moderator@Mailinator.com").Id;
            userManager.AddToRole(userId, "Moderator");


        }
    }
}
