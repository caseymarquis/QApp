using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QApp.Processes;
using QApp.Database;
using System.IO;
using System.Reflection;
using KC.Actin;
using KC.BaseDb;
using Microsoft.EntityFrameworkCore;

namespace QApp {
    public class App {

        public class AppConfig {
            public AppConfig() {
                //Initialize things or grab them from a configuration file if needed.
                AppName = "QAppExample";
                AppDataDirPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), AppName);
                AppLogDirPath = Path.Combine(AppDataDirPath, "logs");
                AppUrl = Environment.GetEnvironmentVariable("ASPNETCORE_URLS") ?? @"http://0.0.0.0:5000/";
                BaseDbType = BaseDbType.Postgres;
                ConnectionStringFromEnvironment = "DATABASE_URL";
                ConnectionStringFromFilePath = Path.Combine(AppDataDirPath, "db.txt");
                ConnectionStringHardCoded = $"UserID=euler;Password=3.14159265358979323846264338327;Host=localhost;Port=5432;Database={AppName};";
                CustomDbBuilderSetup = (builder) => {

                };
            }

            public string AppName;

            public string AppDataDirPath;
            public string AppLogDirPath;

            public string AppUrl;

            public BaseDbType BaseDbType;
            public string ConnectionStringFromEnvironment;
            public string ConnectionStringFromFilePath;
            public string ConnectionStringHardCoded;
            public Action<DbContextOptionsBuilder<AppDbContext>> CustomDbBuilderSetup;
        }

        public static AppConfig Config = new AppConfig();

        private Director director;
        public App() {
            director = new Director(Config.AppLogDirPath);
        }

        public async Task Run(bool logToDisk = true) {
            Util.Log = director.StandardLog;
            Util.Log.LogToDisk = logToDisk;
            await director.Run(startUp_loopUntilSucceeds: true, startUp: async (util) => {
                AppDbContext.DbConnectionSettings.Reset();
                await AppDbContext.Migrate();
                await AppDbContext.Populate();
            }, assembliesToCheckForDI: this.GetType().Assembly);
        }

        public void Dispose() {
            director.Dispose();
        }
    }
}
