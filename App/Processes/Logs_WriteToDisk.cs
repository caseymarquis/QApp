using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QApp.Processes {
    public class Logs_WriteToDisk : AppProcess {
        public override string ProcessName => "Logger";
        protected override TimeSpan RunDelay => new TimeSpan(0, 0, 15);

        private object lockEverything = new object();
        private List<EntLog> queuedLogs = new List<EntLog>();
        private string logDir = null;

        private object lockRealTimeCache = new object();
        private Queue<EntLog> realTimeCache = new Queue<EntLog>();
        private int maxRealTimeLogs = 1000;

        public bool TodaysLogExists {
            get { return getTodaysLogFile().Exists; }
        }

        protected async override Task OnInit() {
            //Generally, C:\ProgramData
            logDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            logDir = Path.Combine(logDir, App.AppName, "Logs");
            await Task.FromResult(0);
        }

        private FileInfo getTodaysLogFile() {
            var fileName = DateTime.Now.ToString("yyyy-MM-dd") + ".xml";
            var fileInfo = new FileInfo(Path.Combine(this.logDir, fileName));
            return fileInfo;
        }

        public void DeleteTodaysLog() {
            var info = this.getTodaysLogFile();
            if (info.Exists) {
                info.Delete();
            }
        }

        public void Error(string context, string location, string message) {
            writeLog(context, location, message, "Error");
        }

        public void Error(string context, string location, Exception ex) {
            this.Error(context, location, ex?.ToString());
        }

        public void Simple(string message) {
            writeLog(message, message, message, "Simple");
        }

        private void writeLog(string context, string location, string message, string type) {
            context = context ?? "";
            location = location ?? "";
            message = message ?? "";
            var log = new EntLog(DateTime.Now, context, location, message, type);
            lock (lockEverything) {
                queuedLogs.Add(log);
            }
        }

        public void RealTime(string context, string location, string message) {
            var now = DateTime.Now;
            lock (lockRealTimeCache) {
                while (realTimeCache.Count > maxRealTimeLogs) {
                    realTimeCache.Dequeue();
                }
                realTimeCache.Enqueue(new EntLog(now, context, location, message, ""));
            }
        }

        public void RealTime(string context, string location, Exception ex) {
            this.RealTime(context, location, ex.ToString());
        }

        public List<EntLog> GetRealTimeLogs(){
            lock (lockRealTimeCache) {
                return realTimeCache.ToList();
            }
        }

        protected async override Task OnRun() {
            var toWrite = new List<EntLog>();
            lock (lockEverything) {
                if (queuedLogs.Count == 0) {
                    return;
                }
                toWrite.AddRange(queuedLogs);
                queuedLogs.Clear();
            }

            var fileInfo = getTodaysLogFile();
            if (!fileInfo.Directory.Exists) {
                fileInfo.Directory.Create();
            }
            if (!fileInfo.Exists) {

            }
            using (var stream = File.OpenWrite(fileInfo.FullName)) {
                var endBytes = Encoding.UTF8.GetBytes("</Logs>");
                if (stream.Length > 0) {
                    stream.Seek(-endBytes.Length, SeekOrigin.End);
                }
                else {
                    var startBytes = Encoding.UTF8.GetBytes("<Logs>" + Environment.NewLine);
                    await stream.WriteAsync(startBytes, 0, startBytes.Length);
                }
                var sb = new StringBuilder();
                var bytes = new byte[0];
                async Task writeToDisk() {
                    if (sb.Length == 0) {
                        return;
                    }
                    var text = sb.ToString();
                    sb.Clear();
                    if (bytes.Length < (4 * text.Length)) {
                        bytes = new byte[text.Length * 4];
                    }
                    int len = Encoding.UTF8.GetBytes(text, 0, text.Length, bytes, 0);
                    sb.Clear();
                    await stream.WriteAsync(bytes, 0, len);
                }
                string getEscaped(string x) {
                    if (x.Contains("&"))
                        x = x.Replace("&", "&amp;");
                    if (x.Contains("<"))
                        x = x.Replace("<", "&lt;");
                    if (x.Contains(">"))
                        x = x.Replace(">", "&gt;");
                    if (x.Contains("\""))
                        x = x.Replace("\"", "&quot;");
                    if (x.Contains("'"))
                        x = x.Replace("'", "&apos;");
                    return x;
                }
                foreach (var log in toWrite) {
                    sb.Append("<Log time=\"");
                    sb.Append(getEscaped(log.Time.ToString()));
                    sb.Append("\" type=\"");
                    sb.Append(getEscaped(log.Type));
                    sb.Append("\" location=\"");
                    sb.Append(getEscaped(log.Location));
                    sb.Append("\" context=\"");
                    sb.Append(getEscaped(log.Context));
                    sb.AppendLine("\">");
                    sb.Append("  ");
                    sb.AppendLine(getEscaped(log.Message));
                    sb.AppendLine("</Log>");
                    if (sb.Length > 10000) {
                        await writeToDisk();
                    }
                }
                await writeToDisk();
                await stream.WriteAsync(endBytes, 0, endBytes.Length);
            }
        }

        protected async override Task OnDispose() {
            //Make sure any final logs get written.
            await this.OnRun();
        }
    }

    public struct EntLog {
        public DateTime Time;
        public string Context;
        public string Location;
        public string Message;
        public string Type;

        public EntLog(DateTime utcNow, string context, string location, string message, string type) : this() {
            Time = utcNow;
            Context = context;
            Location = location;
            Message = message;
            Type = type;
        }
    }
}
