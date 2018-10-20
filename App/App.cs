using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppNS.Utility;
using AppNS.Processes;
using AppNS.Database;

namespace AppNS {
    public class App {
        public static string AppName { get; set; } = "QApp";
        public static bool AppIsSqlite { get; set; } = true;
        public static string AppLocalUrl { get; set; } = "http://0.0.0.0:5000/";

        private bool __running__ = false;
        private object lockRunning = new object();
        private TimeSpan runLoopDelay = new TimeSpan(0, 0, 0, 0, 10);

        private object lockProcessPool = new object();
        private List<AppProcess> processPool = new List<AppProcess>();

        private object lockDisposeHandles = new object();
        private List<AppProcessDisposeHandle> disposeHandles = new List<AppProcessDisposeHandle>();

        public bool Running {
            get {
                lock (lockRunning) {
                    return __running__;
                }
            }
        }

        public void AddProcess(AppProcess process) {
            if (process != null) {
                lock (lockDisposeHandles) {
                    if (disposeHandles == null) {
                        //Means we started shutting down.
                        return;
                    }
                }
                lock (lockProcessPool) {
                    processPool.Add(process);
                }
            }
        }

        private async Task StartUp() {
            Util.Log = new Logs_WriteToDisk();
            await Util.Log.Init();
            this.AddProcess(Util.Log);
            Util.Log.Simple("Service-Starting");

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
                firstRun = false;
                try {
                    await Util.Log.Run();
                }
                catch { }
                try {
                    AppDbContext.DbConnectionSettings.UpdateConnectionString();
                }
                catch (Exception ex) {
                    Util.Log.Error(null, "StartUp.UpdateConnectionString", ex);
                    continue;
                }
                try {
                    await AppDbContext.Migrate();
                }
                catch (Exception ex) {
                    Util.Log.Error(null, "StartUp.Migrate", ex);
                    continue;
                }
                try {
                    await AppDbContext.Populate();
                }
                catch (Exception ex) {
                    Util.Log.Error(null, "StartUp.PopulateDatabase", ex);
                    continue;
                }
                criticalBootPassed = true;
            }

            var checkIfDbIsUp = new Db_CheckIfUp();
            this.AddProcess(checkIfDbIsUp);
            this.AddProcess(new WebSite_Run());
            this.AddProcess(new Time_ResetCachedTimeZone());
            Util.Log.Simple("Service-Started");
        }

        private bool shuttingDown = false;
        public void ShutDown() {
            lock (lockRunning) {
                if (shuttingDown) {
                    return;
                }
                shuttingDown = true;
                __running__ = false;
            }
            List<AppProcessDisposeHandle> handles = null;
            lock (lockDisposeHandles) {
                handles = disposeHandles;
                disposeHandles = null;
            }
            foreach (var handle in handles) {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                handle.DisposeProcess();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }
        }

        public async Task Run() {
            lock (lockRunning) {
                if (__running__) {
                    return;
                }
                __running__ = true;
            }
            try {
                await StartUp();
            }
            catch (Exception ex) {
                Util.Log.Error(null, "StartUp", ex);
            }

#pragma warning disable CS8321 // Local function is declared but never used
            void onDebug() {
#pragma warning restore CS8321 // Local function is declared but never used
                this.AddProcess(new TestProcess());
            }
#if DEBUG
            onDebug();
#endif
            var poolCopy = new List<AppProcess>();

            void printIfDebug(string msg) {
#if DEBUG
                Console.WriteLine($"{DateTime.Now.Second}: {msg}");
#endif
            }

            void safeLog(string location, Exception ex) {
                try {
                    Util.Log.Error("Main Loop", location, ex);
                }
                catch { }
            }
            //The whole service is basically a big task pool.
            //Everything runs as its own process in this loop.
            //Notable exception is the API and Website.
            while (Running) {
                try {

                    try {
                        if (Environment.UserInteractive) {
                            if (Console.KeyAvailable) {
                                var key = Console.ReadKey(true).Key;
                                if (key == ConsoleKey.Q || key == ConsoleKey.Escape) {
                                    this.ShutDown();
                                    await Task.Delay(5000); //Simulate the time we normally get for shutdown.
                                }
                            }
                        }
                    }
                    catch (Exception ex) {
                        safeLog("User Interactive Check", ex);
                    }

                    try {
                        poolCopy.Clear();
                        lock (lockProcessPool) {
                            poolCopy.AddRange(processPool);
                        }
                    }
                    catch (Exception ex) {
                        safeLog("Process Pool Copy", ex);
                    }


                    try {
                        var shouldRemove = poolCopy.Where(x => x.ShouldBeRemovedFromPool).ToList();
                        if (shouldRemove.Count > 0) {
                            lock (lockProcessPool) {
                                processPool.RemoveAll(x => shouldRemove.Contains(x));
                                poolCopy.Clear();
                                poolCopy.AddRange(processPool);
                            }
                        }
                    }
                    catch (Exception ex) {
                        safeLog("Process Pool Pruning", ex);
                    }

                    List<AppProcessDisposeHandle> handles = null;
                    lock (lockDisposeHandles) {
                        handles = disposeHandles;
                    }

                    try {
                        var remainingHandles = new List<AppProcessDisposeHandle>();
                        if (handles != null) {
                            foreach (var handle in handles) {
                                if (handle.MustDispose) {
                                    printIfDebug("dispose-" + handle.ProcessName);
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                                    handle.DisposeProcess();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                                }
                                else {
                                    remainingHandles.Add(handle);
                                }
                            }
                            lock (lockDisposeHandles) {
                                if (disposeHandles != null) {
                                    disposeHandles = remainingHandles;
                                }
                            }
                        }
                    }
                    catch (Exception ex) {
                        safeLog("Dispose Processes", ex);
                    }

                    try {
                        foreach (var process in poolCopy) {
                            try {
                                if (process.ShouldBeInit) {
                                    printIfDebug("init-" + process.ProcessName);
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                                    process.Init().ContinueWith(async task => {
                                        if (task.Status == TaskStatus.RanToCompletion) {
                                            if (task.Result != null) {
                                                var handle = task.Result;
                                                var mustDisposeNow = false;
                                                lock (lockDisposeHandles) {
                                                    if (disposeHandles != null) {
                                                        disposeHandles.Add(handle);
                                                    }
                                                    else {
                                                        //Means that the whole application has been disposed.
                                                        mustDisposeNow = true;
                                                    }
                                                }
                                                if (mustDisposeNow) {
                                                    await handle.DisposeProcess();
                                                }
                                            }
                                        }
                                    });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                                }
                                else if (process.ShouldBeRunNow) {
                                    printIfDebug("run-" + process.ProcessName);
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                                    process.Run();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                                }
                            }
                            catch (Exception ex) {
                                safeLog("Running Process", ex);
                            }
                        }
                    }
                    catch (Exception ex) {
                        safeLog("Running All Processes", ex);
                    }

                    await Task.Delay(runLoopDelay);
                }
                catch (Exception ex) {
                    safeLog("main while", ex);
                }
            }
        }
    }
}
