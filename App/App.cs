using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QApp.Actors;
using QApp.Database;
using System.IO;
using System.Reflection;
using KC.Actin;
using KC.BaseDb;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using KC.Actin.ActorUtilNS;

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

        //This is static so we can inject dependencies into
        //out web controllers.
        public static Director Director = new Director();

        public async Task Run(bool logToDisk = true) {
            await Director.Run(configure: config => {
                config.Set_DirectorName("Main Director");
                config.Set_AssembliesToCheckForDependencies(typeof(WebSite_Run).Assembly);
                if (logToDisk) {
                    config.Set_StandardLogOutputFolder(Config.AppLogDirPath);
                }
                else {
                    //Will log to standard output.
                    //You can also set a custom log.
                }

                var sw = new Stopwatch();
                Console.WriteLine("App Starting...");
                sw.Start();
                config.Run_AfterStart(async util => {
                    Console.WriteLine($"App started in {sw.ElapsedMilliseconds}ms.");
                    var log = Director.GetSingleton<ActinStandardLogger>();

                    await retryUntilSuccessful("Start Site", async () => {
                        var runSite = Director.GetSingleton<WebSite_Run>();
                        return await Task.FromResult(true);
                    });

                    try {
                        await AppDbContext.Migrate();
                    }
                    catch (Exception ex) {
                        //In multiserver environments with controlled rollouts, old application versions may start
                        //after a new version has updated the DB (or if you roll back). In this case, you want the app to still start.
                        //It is up to the developer to ensure DB Schema changes allow this (ie only additive until a feature is fully removed)
                        util.Log.Error("Failed to Migrate DB. Proceeding regardless.", ex);
                    }

                    try {
                        if (AppDbContext.CanStartPubSubLoop) {
                            AppDbContext.StartPubSubLoop(ex => {
                                util.Log.Error("Failed to start DB PubSub loop (inner). Proceeding regardless.", ex);
                            });
                        }
                    }
                    catch (Exception ex) {
                        util.Log.Error("Failed to start DB PubSub loop (outer). Proceeding regardless.", ex);
                    }

                    await retryUntilSuccessful("Populate DB", async () => {
                        await AppDbContext.Populate();
                        return true;
                    });

                    async Task retryUntilSuccessful(string taskName, Func<Task<bool>> doTask) {
                        while (Director.Running) {
                            try {
                                util.Log.Info("Attempting: " + taskName);
                                await writeLogs();
                                var success = await doTask();
                                if (!success) {
                                    util.Log.Error("Failed: " + taskName);
                                    await writeLogs();
                                    await Task.Delay(5000);
                                    continue;
                                }
                                util.Log.Info("Finished: " + taskName);
                                await writeLogs();
                                break;
                            }
                            catch (Exception ex) {
                                util.Log.Error("Failed: " + taskName, ex);
                                await writeLogs();
                                await Task.Delay(5000);
                                continue;
                            }
                            async Task writeLogs() {
                                try {
                                    await log.Run(() => new DispatchData {
                                        MainLog = log,
                                        Time = DateTimeOffset.Now,
                                    });
                                }
                                catch { }
                            }
                        }
                    }
                });
            });
        }

        public void Dispose() {
            Director.Dispose();
            if (AppDbContext.CanStartPubSubLoop) {
                AppDbContext.StopPubSubLoop();
            }
        }
    }
}
