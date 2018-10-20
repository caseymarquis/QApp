using App.Database.Models;
using Microsoft.EntityFrameworkCore;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Database {
    public partial class AppDbContext : DbContext {
        public static readonly DbConnectionSettings DbConnectionSettings = new DbConnectionSettings();
        private static AsyncReaderWriterLock sqliteLock = new AsyncReaderWriterLock();

        public static async Task<T> WithContext<T>(Func<AppDbContext, Task<T>> getSomething, bool isWrite = true) {
            var connectionString = DbConnectionSettings.ConnectionString;
            IDisposable lockHandle = null;
            if (App.AppIsSqlite) {
                lockHandle = await (isWrite ? sqliteLock.WriterLockAsync() : sqliteLock.ReaderLockAsync());
            }
            try {
                using (var db = new AppDbContext(connectionString)) {
                    return await getSomething(db);
                }
            }
            finally {
                if (lockHandle != null) {
                    lockHandle.Dispose();
                }
            }
        }

        public static async Task WithContext(Func<AppDbContext, Task> doSomething, bool isWrite = true) {
            await AppDbContext.WithContext(async (db) => {
                await doSomething(db);
                return 0;
            }, isWrite);
        }

        public static async Task Migrate() {
            await AppDbContext.WithContext(async db => {
                await db.Database.MigrateAsync();
            });
        }

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
                        Email = "caseym@macdac.com",
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
