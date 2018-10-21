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

namespace QApp {
    public static class Util {
        public static Logs_WriteToDisk Log = new Logs_WriteToDisk();

        /// <summary>
        /// Because SQL Server has a limited date range.
        /// </summary>
        public static DateTime InitDateTime => new DateTime(1900, 1, 1);

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
            var stopAt = DateTime.UtcNow.Add(timeout);
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
                var utcNow = DateTime.UtcNow;
                lock (lockEverything) {
                    if (utcNow > stopAt) {
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

        public static IEnumerable<T> ShallowCopyRange<T>(IEnumerable<T> originalItems) where T : class, new() {
            var ret = new List<T>(originalItems.Count());
            var props = Util.GetProps<T>();
            foreach (var item in originalItems) {
                ret.Add(Util.ShallowCopyItem(item, props));
            }
            return ret;
        }

        public static T ShallowCopyItem<T>(T item, PropertyCache props = null) where T : class, new() {
            if (props == null) {
                props = GetProps<T>();
            }
            var newT = new T();
            foreach (var prop in props.ValueAndStringProperties) {
                prop.Copy(item, newT);
            }
            return newT;
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
            var timeoutQuit = DateTime.UtcNow.Add(timeout);
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
                if (DateTime.UtcNow > timeoutQuit) {
                    throw new TimeoutException("Task timed out.");
                }
            }
        }

        private static object GetMemberValue(MemberExpression member) {
            var objectMember = Expression.Convert(member, typeof(object));
            var getterLambda = Expression.Lambda<Func<object>>(objectMember);
            var getter = getterLambda.Compile();
            return getter();
        }

        //Kind of proud that I was able to figure this out.
        public static IQueryable<IGrouping<TKey, TSource>> GroupByDateDiff<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector) {
            var body = (NewExpression)keySelector.Body;
            foreach (var arg in body.Arguments) {
                if (arg.NodeType == ExpressionType.Call) {
                    var callNode = (MethodCallExpression)arg;
                    if (callNode.Method.Name == "DateDiff") {
                        var dateDiffFirstArg = callNode.Arguments[0];
                        if (dateDiffFirstArg.NodeType == ExpressionType.Constant) {
                            //It was already a constant, so we're good.
                        }
                        else {
                            //HACK: This will break if the internal implementation of ReadOnlyCollection changes.
                            var listInfo = typeof(ReadOnlyCollection<Expression>).GetField("list", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                            var list = (IList)listInfo.GetValue(callNode.Arguments);
                            if (dateDiffFirstArg.NodeType == ExpressionType.MemberAccess) {
                                list[0] = Expression.Constant((string)GetMemberValue((MemberExpression)dateDiffFirstArg));
                            }
                            else {
                                throw new ArgumentException($"{nameof(GroupByDateDiff)} was unable to parse the datePartArg argument to the DateDiff function.");
                            }
                        }
                    }
                }
            }
            return source.GroupBy(keySelector);
        }

        public class TimeSelectorSpan {
            public DateTime StartTimeUtc = Util.InitDateTime;
            public DateTime EndTimeUtc = Util.InitDateTime;
            public DateTime StartTimeLocal = Util.InitDateTime;
            public DateTime EndTimeLocal = Util.InitDateTime;
            public TimeSpan TimeZoneOffsetStart;
            public TimeSpan TimeZoneOffsetEnd;
            public string DateDiffSelector = "";
            public string UnitTime = "";

            public DateTime GetDateUtcFromBarriersCrossed(int dateDiffBarriersCrossed) {
                if (DateDiffSelector == "hh") {
                    return StartTimeUtc.AddHours(dateDiffBarriersCrossed);
                }
                else if (DateDiffSelector == "dd") {
                    return StartTimeUtc.AddDays(dateDiffBarriersCrossed);
                }
                else if (DateDiffSelector == "ww") {
                    return StartTimeUtc.AddDays(dateDiffBarriersCrossed * 7);
                }
                else if (DateDiffSelector == "mm") {
                    return Util.GetLocalStartOfUnitTimeInUtc(StartTimeUtc.AddDays(dateDiffBarriersCrossed * 30 + 15), "mm");
                }
                else {
                    throw new ArgumentException("Time Selector not supported: " + DateDiffSelector);
                }
            }
        }

        public static TimeSelectorSpan GetDateDiffSelector(DateTime startTimeUtc, DateTime endTimeUtc, string overrideDateDiffSelector = null) {
            var totalTime = endTimeUtc - startTimeUtc;
            TimeSelectorSpan span = null;
            if (overrideDateDiffSelector != null) {
                switch (overrideDateDiffSelector) {
                    case "hh":
                        span = new TimeSelectorSpan() {
                            DateDiffSelector = overrideDateDiffSelector,
                            UnitTime = "Hours"
                        };
                        break;
                    case "dd":
                        span = new TimeSelectorSpan() {
                            DateDiffSelector = overrideDateDiffSelector,
                            UnitTime = "Days"
                        };
                        break;
                    case "ww":
                        span = new TimeSelectorSpan() {
                            DateDiffSelector = overrideDateDiffSelector,
                            UnitTime = "Weeks"
                        };
                        break;
                    case "mm":
                        span = new TimeSelectorSpan() {
                            DateDiffSelector = overrideDateDiffSelector,
                            UnitTime = "Months"
                        };
                        break;
                }
            }
            else if (totalTime < new TimeSpan(0, 336, 0, 0)) { //Less than 14 days, show hours.
                span = new TimeSelectorSpan() {
                    DateDiffSelector = "hh",
                    UnitTime = "Hours",
                };
            }
            else if (totalTime < new TimeSpan(100, 0, 0, 0)) { //Less than 14 Weeks show days.
                span = new TimeSelectorSpan() {
                    DateDiffSelector = "dd",
                    UnitTime = "Days",
                };
            }
            else if (totalTime < new TimeSpan(30*14, 0, 0, 0)) { //Less than 14 months show weeks.
                span = new TimeSelectorSpan() {
                    DateDiffSelector = "ww",
                    UnitTime = "Weeks",
                };
            }
            else {
                span = new TimeSelectorSpan() {
                    DateDiffSelector = "mm",
                    UnitTime = "Months",
                };
            }
            span.StartTimeUtc = Util.GetLocalStartOfUnitTimeInUtc(startTimeUtc, span.DateDiffSelector);
            span.EndTimeUtc = Util.GetLocalEndOfUnitTimeInUtc(endTimeUtc, span.DateDiffSelector);
            span.StartTimeLocal = span.StartTimeUtc.ToLocalTime();
            span.EndTimeLocal = span.EndTimeUtc.ToLocalTime();
            span.TimeZoneOffsetStart = (span.StartTimeLocal - span.StartTimeUtc);
            span.TimeZoneOffsetEnd = (span.EndTimeLocal - span.EndTimeUtc);
            return span;
        }

        /// <summary>
        /// Convert the given date to local time, find the start of the hour/day/week/month/etc, convert this to UTC time, and return it.
        /// <returns></returns>
        /// </summary>
        public static DateTime GetLocalStartOfUnitTimeInUtc(DateTime startTimeUtc, string dateDiffSelector) {
            var localTime = startTimeUtc.ToLocalTime();
            if (dateDiffSelector == "hh") {
                return new DateTime(localTime.Year, localTime.Month, localTime.Day, localTime.Hour, 0, 0).ToUniversalTime();
            }
            else if (dateDiffSelector == "dd") {
                return new DateTime(localTime.Year, localTime.Month, localTime.Day, 0, 0, 0).ToUniversalTime();
            }
            var localDate = localTime.Date;
            if (dateDiffSelector == "ww") {
                return (localDate.AddDays(-(int)localDate.DayOfWeek)).ToUniversalTime();
            }
            else if (dateDiffSelector == "mm") {
                return new DateTime(localTime.Year, localTime.Month, 1).ToUniversalTime();
            }
            else {
                throw new ArgumentException("Time Selector not supported: " + dateDiffSelector);
            }
        }

        /// <summary>
        /// Convert the given date to local time, find the end of the hour/day/week/month/etc, convert this to UTC time, and return it.
        /// <returns></returns>
        /// </summary>
        public static DateTime GetLocalEndOfUnitTimeInUtc(DateTime endTimeUtc, string dateDiffSelector) {
            var start = Util.GetLocalStartOfUnitTimeInUtc(endTimeUtc, dateDiffSelector);
            if (dateDiffSelector == "hh") {
                return start.AddHours(1);
            }
            else if (dateDiffSelector == "dd") {
                return start.AddDays(1);
            }
            else if (dateDiffSelector == "ww") {
                return start.AddDays(7);
            }
            else if (dateDiffSelector == "mm") {
                return Util.GetLocalStartOfUnitTimeInUtc(start.AddDays(40), "mm");
            }
            else {
                throw new ArgumentException("Time Selector not supported: " + dateDiffSelector);
            }
        }

        public static PropertyCache GetProps(Type classType) {
            return PropertyCache.GetCache(classType);
        }

        public static PropertyCache GetProps<T>()
            where T : class {
            return PropertyCache.GetCache(typeof(T));
        }

        public static string GetPropertyName<T, U>(Expression<Func<T, U>> getProperty) {
            var name = GetPropertyNameOrNull(getProperty);
            if (name == null) {
                throw new Exception("Could not find property name!");
            }
            return name;
        }

        public static string GetPropertyNameOrNull<T, U>(Expression<Func<T, U>> getProperty) {
            string name;
            if (getProperty.Body is MemberExpression) {
                var expression = (MemberExpression)getProperty.Body;
                name = expression.Member.Name;
            }
            else {
                var op = ((UnaryExpression)getProperty.Body).Operand;
                name = ((MemberExpression)op).Member.Name;
            }
            return name;
        }

        /// <summary>
        /// I wouldn't worry too much about how this works, but you'll want to understand what it's doing.
        /// 
        /// We're efficiently looping through the properties on two different types, matching by name (ignoring their case),
        /// and then copying the value of one matching property to another. If the property types don't match,
        /// there will be a runtime error. These properties should be marked with the [ReflectionIgnore]
        /// attribute.
        /// 
        /// If you want to understand how this works, you'll first need to play around with reflection and understand that.
        /// Then you'll need to look into expression trees and compiling code at run time.
        /// 
        /// The gist of it is that reflection is really slow, so we use reflection to dynamically compile code
        /// which allows us to dynamically enumerate through properties on an object, see their names, and get/set their values.
        /// </summary>
        public static void CopyPropsIgnoreCase<T, U>(T fromT, U toU, bool copyNullProperties = false) where T : class where U : class {
            var fromProps = Util.GetProps<T>();
            var toProps = Util.GetProps<U>();

            foreach (var toProp in toProps.ValueAndStringProperties) {
                var fromProp = fromProps.ValueAndStringProperties.FirstOrDefault(x => string.Compare(x.Name, toProp.Name, true) == 0);
                if (fromProp == null) {
                    continue;
                }

                var fromValue = fromProp.GetVal(fromT);
                if (!copyNullProperties && object.Equals(fromValue, null)) {
                    continue;
                }

                toProp.SetVal(toU, fromValue);
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
