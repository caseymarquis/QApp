using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QApp.Processes;
using QApp.Database;
using System.IO;
using System.Reflection;
using KC.NanoProcesses;
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
                BaseDbType = BaseDbType.SqlServer;
                ConnectionStringFromEnvironment = "Data Source =$|-DBHOST-$|; Initial Catalog = $|-DBNAME-$|; Integrated Security = False; User ID = $|-DBUSER-$|; Password = $|-DBPASS-$|; MultipleActiveResultSets = True";
                AppUrl = Environment.GetEnvironmentVariable("ASPNETCORE_URLS") ?? @"http://0.0.0.0:5000/";
                ConnectionStringFromFilePath = Path.Combine(AppDataDirPath, "db.txt");
                ConnectionStringHardCoded = $"Data Source =.\\ESR; Initial Catalog = QAppExample; Integrated Security = False; User ID = euler; Password = 3.14159265358979323846264338327; MultipleActiveResultSets = True";
                CustomDbBuilderSetup = (builder) => {
                    //If you need this, you might be better off not using the KC.BaseDb library.
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

        private NanoProcessManager procManager;
        public App() {
            procManager = new NanoProcessManager(Config.AppLogDirPath);
        }

        public async Task Run(bool logToDisk = true) {
            Util.Log = procManager.StandardLog;
            Util.Log.LogToDisk = logToDisk;
            await procManager.Run(async (util) => {
                var criticalBootPassed = false;
                var firstRun = true;
                while (!criticalBootPassed) {
                    //If we don't make it through this loop, 
                    //then the service can't start. The DB isn't up,
                    //and we haven't ensured it's migrated.
                    //If anything MUST happen before the app starts, 
                    //it should be added in below in the style of the other items.
                    if (!firstRun) {
                        await Task.Delay(5000);
                    }
                    util.UtcNow = DateTime.UtcNow;
                    firstRun = false;
                    try {
                        //Needs to be removed if a custom logger is used.
                        await procManager.StandardLog.Run(util);
                    }
                    catch { }
                    try {
                        //If we're failing to access the db, continually refresh the connection string:
                        AppDbContext.DbConnectionSettings.Reset();
                    }
                    catch (Exception ex) {
                        util.Log.Error(null, "StartUp.ResetConnectionString", ex);
                        continue;
                    }
                    try {
                        await AppDbContext.Migrate();
                    }
                    catch (Exception ex) {
                        util.Log.Error(null, "StartUp.Migrate", ex);
                        continue;
                    }
                    try {
                        await AppDbContext.Populate();
                    }
                    catch (Exception ex) {
                        util.Log.Error(null, "StartUp.PopulateDatabase", ex);
                        continue;
                    }
                    criticalBootPassed = true;
                }
            }, this.GetType().Assembly);
        }

        public void Dispose() {
            procManager.Dispose();
        }
    }
}
