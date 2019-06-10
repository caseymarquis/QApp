using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Security.Cryptography;
using System.Linq.Expressions;
using System.Collections.ObjectModel;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using QApp.Processes;
using KC.NanoProcesses;

namespace QApp {
    public static class Util {
        public static NPStandardLogger Log;

        public static string[] GetEnumOptionsAsString<T>() where T : struct
        {
            var enumArray = (T[])Enum.GetValues(typeof(T));
            return enumArray
                .Select(x => Enum.GetName(typeof(T), x))
                .ToArray();
        }

        public static T? GetEnumFromString<T>(string value) where T : struct
        {
            T ret;
            if (Enum.TryParse(value, out ret)) {
                return ret;
            }
            return null;
        }

        public static async Task WaitAsync(this Task userTask, TimeSpan timeout, CancellationToken? cToken = null, TimeSpan? waitInterval = null) {
            await Util.WaitAsync(Task.Run(async () => {
                await userTask;
                return 0;
            }), timeout, cToken, waitInterval);
        }

        [SuppressMessage("", "CS4014")] //We want to run async tasks in here without waiting.
        public static async Task<T> WaitAsync<T>(this Task<T> userTask, TimeSpan timeout, CancellationToken? cToken = null,  TimeSpan? waitInterval = null) {
            if (!waitInterval.HasValue) {
                waitInterval = new TimeSpan(0, 0, 0, 0, 10); //Mils
            }
            if (!cToken.HasValue) {
                cToken = CancellationToken.None;
            }
            var stopAt = DateTimeOffset.Now.Add(timeout);
            var finished = false;
            var success = false;
            var lockEverything = new object();

            T result = default(T);
            Exception ex = null;
            var awaitedTask = userTask.ContinueWith(task => {
                lock (lockEverything) {
                    if (finished) {
                        return;
                    }
                    finished = true;
                    if (task.IsFaulted) {
                        ex = task.Exception;
                    }
                    else if (task.IsCanceled) {
                        ex = new TaskCanceledException("Task canceled.");
                    }
                    else if (!task.IsCompleted) {
                        ex = new TaskCanceledException("Bad task state. Util.cs");
                    }
                    else {
                        result = task.Result;
                        success = true;
                    }
                }
            }, cToken.Value);

            //Wait for completion or timeout.
            while (true) {
                var now = DateTimeOffset.Now;
                lock (lockEverything) {
                    if (now > stopAt) {
                        lock (lockEverything) {
                            finished = true;
                        }
                        throw new TimeoutException("Task timed out.");
                    }
                    if (finished) {
                        break;
                    }
                    cToken?.ThrowIfCancellationRequested();
                }
                await Task.Delay(waitInterval.Value);
            }

            lock (lockEverything) {
                if (success) {
                    return result;
                }
                else throw ex ?? new Exception("Util.WaitAsync Exception was not set.");
            }
        }

        public static async Task WaitForThreadAsync(TimeSpan timeout, CancellationToken? cToken, Action doThis) {
            await Util.WaitForThreadAsync(timeout, cToken, () => {
                doThis();
                return 0;
            });
        }

        public static ulong GetCryptoLong() {
            var r = RandomNumberGenerator.Create();
            ulong n = 0;
            var bytes = new byte[8];
            r.GetNonZeroBytes(bytes);
            foreach (var b in bytes) {
                n <<= 8;
                n += b;
            }
            return n;
        }

        public static async Task<T> WaitForThreadAsync<T>(TimeSpan timeout, CancellationToken? cToken, Func<T> doThis) {
            var lockEverything = new object();
            var finished = false;
            Exception exOuter = null;
            var timeoutQuit = DateTimeOffset.Now.Add(timeout);
            T result = default(T);
            var t = new Thread(()=> {
                try {
                    result = doThis();
                }
                catch (Exception ex) {
                    lock (lockEverything) {
                        exOuter = ex;
                    }
                }
                finally {
                    lock (lockEverything) {
                        finished = true;
                    }
                }
            });
            t.IsBackground = true;
            t.Start();

            while (true) {
                await Task.Delay(10);
                lock (lockEverything) {
                    if (finished) {
                        if (exOuter != null) {
                            throw exOuter;
                        }
                        return result;
                    }
                }
                if (t.ThreadState == ThreadState.Aborted) {
                    throw new TaskCanceledException("Thread was aborted.");
                }
                if (cToken?.IsCancellationRequested ?? false) {
                    throw new TaskCanceledException("Task was cancelled.");
                }
                if (DateTimeOffset.Now > timeoutQuit) {
                    throw new TimeoutException("Task timed out.");
                }
            }
        }

        public static Guid GetCryptoGuid() {
            var r = RandomNumberGenerator.Create();
            var cryptoBytes = new byte[16];
            r.GetNonZeroBytes(cryptoBytes);
            return new Guid(cryptoBytes);
        }

    }
}
