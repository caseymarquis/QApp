using QApp.Database.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.BaseDb;

namespace QApp.Database {
    public partial class AppDbContext : BaseDbContext<AppDbContext> {

        public AppDbContext() : base(App.Config.AppName, App.Config.BaseDbType, App.Config.ConnectionStringFromEnvironment, App.Config.ConnectionStringFromFilePath, App.Config.ConnectionStringHardCoded, App.Config.CustomDbBuilderSetup) {
        }

        public DbSet<User> Users { get; set; }

        public static async Task Populate() {
            await AppDbContext.WithContext(async db => {
                bool mustSave = false;

                //Add the admin user:
                if (await db.Users.CountAsync() == 0) {
                    mustSave = true;
                    db.Users.Add(new User() {
                        Enabled = true,
                        IsAdmin = true,
                        Name = "Casey Marquis",
                        Email = "admin",
                        ResetPassword = true,
                    });
                }

                if (mustSave) {
                    await db.SaveChangesAsync();
                }

            });
        }
    }
}
