using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace QApp
{
    public class AppConfig {
        string configPath;
        public AppConfig(string _configPath) {
            this.configPath = _configPath;
        }

        public string AppName => Init(() => m_AppName);
        public string LocalUrl => Init(() => m_LocalUrl);
        public bool UseSqlite => Init(() => m_UseSqlite);

        private object lockEverything = new object();

        private string m_AppName = null;
        private bool m_UseSqlite = false;
        private string m_LocalUrl;

        private IEnumerable<string> lines = null;
        public string GetVar(string varName) {
            lock (lockEverything) {
                if (lines == null) {
                    lines = File.ReadAllLines(configPath);
                }
                var line = lines
                    .Select(x => x.Trim())
                    .Where(x => !x.StartsWith("#") && x.Contains("="))
                    .FirstOrDefault(x => x.ToUpper()
                        .StartsWith(varName.ToUpper()));
                return line.Substring(line.IndexOf("=") + 1).Trim();
            }
        }

        private T Init<T>(Func<T> getFromPrivateVar) {
            lock (lockEverything) {
                if (lines == null) {
                    m_AppName = GetVar("AppName");
                    bool.TryParse(GetVar("UseSqlite"), out this.m_UseSqlite);
                    m_LocalUrl = GetVar("LocalUrl");
                }
                return getFromPrivateVar();
            }
        }
    }
}
