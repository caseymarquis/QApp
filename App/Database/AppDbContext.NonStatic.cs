using QApp.Database.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QApp.Database {
    public partial class AppDbContext : DbContext {
        /// <summary>
        /// Don't use this directly!
        /// This constructor allows NuGet to migrate/update/etc the database.
        /// </summary>
        public AppDbContext() : this(AppDbContext.DbConnectionSettings.UpdateConnectionString()) {
        }

        private static DbContextOptions getDbOptions(string connectionString) {
            var builder = new DbContextOptionsBuilder<AppDbContext>() {
                
            };
            if (App.AppIsSqlite) {
                builder.UseSqlite(connectionString);
            }
            else {
                builder.UseSqlServer(connectionString);
            }
            return builder.Options;
        }

        /// <summary>
        /// Don't use this either! Use the AppDbContext.WithContext functions.
        /// That way if we have to do something at every access in the future,
        /// it'll be easy.
        /// </summary>
        /// <param name="connectionString"></param>
        private AppDbContext(string connectionString) : base(getDbOptions(connectionString)) {

        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder b) {
            base.OnModelCreating(b);
        }
    }
}
