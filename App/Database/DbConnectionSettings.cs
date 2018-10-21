using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QApp.Database {
    public class DbConnectionSettings {
        private string storedConnectionString = null;
        private object lockEverything = new object();

        public string ConnectionString {
            get {
                lock (lockEverything) {
                    return storedConnectionString;
                }
            }
        }

        private string getConnectionStringFromFile(string filePath) {
            return File.ReadAllLines(filePath).FirstOrDefault(x => x.Trim() != "" && !x.Trim().StartsWith("#"));
        }

        public string UpdateConnectionString() {
            lock (lockEverything) {
                var isSqlite = App.Config.UseSqlite; //Change to use SQL Server instead. Can modify the below if you need something different.

                string gev(string varName) {
                    return Environment.GetEnvironmentVariable(varName);
                }

                var cs = "";
                if (gev("DBHOST") == null) {
                    //Using a connection string from a file:
                    var devCSFilePath = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                        App.Config.AppName, "db.txt"); //Set local connection string file here.
                    new FileInfo(devCSFilePath).Directory.Create();
                    if (!File.Exists(devCSFilePath)) {
                        if (isSqlite) {
                            File.WriteAllText(devCSFilePath, "Filename=" + Path.Combine(new FileInfo(devCSFilePath).DirectoryName, "db.sqlite"));
                        }
                        else {
                            File.WriteAllText(devCSFilePath, $"Data Source =.\\ESR; Initial Catalog = {App.Config.AppName}; Integrated Security = False; User ID = sa; Password = Aa1!0c8335fd-dfd2-48bf-96de-f340b80747ef; MultipleActiveResultSets = True");
                        }
                    }
                    cs = getConnectionStringFromFile(devCSFilePath);
                }
                else {
                    //Otherwise set with environment variables:
                    cs = $"Data Source =.\\{gev("DBHOST")}; Initial Catalog = {gev("DBNAME")}; Integrated Security = False; User ID = {gev("DBUSER")}; Password = {gev("DBPASSWORD")}; MultipleActiveResultSets = True";
                }

                storedConnectionString = cs;
                return cs;
            }
        }

    }
}
