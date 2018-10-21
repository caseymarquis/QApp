using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace AppBuild
{
    //This is run on release builds
    public class Program
    {
        public static void Main(string[] args)
        {
            //Find solution directory:
            var solutionDir = new DirectoryInfo("./");
            while (true) {
                var files = solutionDir.GetFiles()
                    .Select(x => x.Name.ToUpper())
                    .ToArray();

                if (files.Any(x => x.ToUpper().EndsWith(".SLN"))) {
                    break;
                }

                solutionDir = solutionDir.Parent;
                if (solutionDir == null) {
                    throw new Exception("Solution Directory Not Found!");
                }
            }

            var siteDev = Path.Combine(solutionDir.FullName, "App", "wwwdev");
            var siteRoot = Path.Combine(solutionDir.FullName, "App", "wwwroot");
            var binFramework = Path.Combine(solutionDir.FullName, "AppFramework", "bin", "Release");

            void log(string msg) {
                Console.WriteLine(msg);
            }

            log("npm install");
            var npmProcInfo = new ProcessStartInfo() {
                Arguments = $"install",
                FileName = "npm",
                WorkingDirectory = siteDev,
            };
            try {
                Process.Start(npmProcInfo).WaitForExit();
            }
            catch {
                log("npm install Windows");
                npmProcInfo.FileName = @"C:\Program Files\nodejs\npm.cmd";
                Process.Start(npmProcInfo).WaitForExit();
            }

            log("remove old files");
            var srDir = new DirectoryInfo(siteRoot);
            var ignoreList = new string[] { "index", "favicon", ".gitignore" };
            foreach (var file in srDir.GetFiles()) {
                if (ignoreList.Any(x => file.Name.ToLower().StartsWith(x))) {
                    continue;
                }
                file.Delete();
            }

            log("webpack");
            var webpackPath = Path.Combine(siteDev, "node_modules/webpack/bin/webpack");
            var webpackProcInfo = new ProcessStartInfo() {
                Arguments = $"\"{webpackPath}\" --progress --colors --display-error-details --env.build=prod",
                FileName = "node",
                WorkingDirectory = siteDev,
            };
            Process.Start(webpackProcInfo).WaitForExit();

            copyFiles(binFramework, false);

            void copyFiles(string bin, bool zip) {
                log("copy webpack output to bin");
                var oldSiteRoot = new DirectoryInfo(Path.Combine(bin, "wwwroot"));
                if (oldSiteRoot.Exists) {
                    oldSiteRoot.Delete(true);
                }
                oldSiteRoot.Create();

                foreach (var file in new DirectoryInfo(siteRoot).GetFiles()) {
                    if (!file.Name.StartsWith(".gitignore")) {
                        file.CopyTo(Path.Combine(oldSiteRoot.FullName, file.Name));
                    }
                }

                if (zip) {
                    log("delete old zip file");
                    var zipFile = new FileInfo(Path.Combine(solutionDir.FullName, "app.zip"));
                    if (zipFile.Exists) {
                        zipFile.Delete();
                    }

                    log("create new zip file");
                    ZipFile.CreateFromDirectory(bin, zipFile.FullName);
                }
                
            }
            
        }
    }
}
