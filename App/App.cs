using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QApp.Utility;
using QApp.Processes;
using QApp.Database;
using System.IO;
using System.Reflection;
using KC.NanoProcesses;

namespace QApp {
    public class App {

        private NanoProcessManager procManager;
        public App() {
            var logDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                App.Config.AppName,
                "logs");
            procManager = new NanoProcessManager(logDir);
        }

        public async Task Run() {
            Util.Log = procManager.StandardLog;
            await procManager.Run(async (util) => {
                var criticalBootPassed = false;
                var firstRun = true;
                while (!criticalBootPassed) {
                    util.UtcNow = DateTime.UtcNow;
                    //If we don't make it through this loop, 
                    //then the service can't start. The DB isn't up,
                    //and we haven't ensured it's migrated.
                    //If anything MUST happen before the app starts, 
                    //it should be added in below in the style of the other items.
                    if (!firstRun) {
                        await Task.Delay(5000);
                    }
                    firstRun = false;
                    try {
                        //Needs to be removed if a custom logger is used.
                        await procManager.StandardLog.Run(util);
                    }
                    catch { }
                    try {
                        AppDbContext.DbConnectionSettings.UpdateConnectionString();
                    }
                    catch (Exception ex) {
                        util.Log.Error(null, "StartUp.UpdateConnectionString", ex);
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
            });
        }

        public void Dispose() {
            procManager.Dispose();
        }

        private static string getConfigPath() {
            string exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Directory.SetCurrentDirectory(exeDir);
            var di = new DirectoryInfo("./");
            var startDiPath = di.FullName;
            while (true) {
                var configFile = di.GetFiles("___config___.txt").FirstOrDefault();
                if (configFile != null) {
                    return configFile.FullName;
                }
                di = di.Parent;
                if (di == null) {
                    throw new InvalidOperationException("No ___config___.txt found. Start dir: " + startDiPath);
                }
            }
        }

        public static AppConfig Config = new AppConfig(getConfigPath());
    }
}
