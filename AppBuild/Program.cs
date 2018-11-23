using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;

namespace ServiceInstallNS
{
    //This is run on release builds
    public class Program
    {
        public static void Main(string[] args)
        {
            var skipNpmInstall = true; //For debugging
            var skipWebpack = true;

            var isFramework = args?.Length > 0? args[0].Contains("AppFramework") : true;
            var isCore = args?.Length > 0? args[0].Contains("AppCore") : true;

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

            var binFramework = Path.Combine(solutionDir.FullName, "AppFramework", "bin", "Release");
            //This needs to be updated if the .net core version changes.
            //This only matters if running with no command line arguments however, as arg[0] is the output directory
            var binCore = Path.Combine(solutionDir.FullName, "AppCore", "bin", "Release", "netcoreapp2.1");
            if (isFramework != isCore) {
                binFramework = binCore = args[0].Trim('"');
            }

            var confFileName = "___config___.txt";
            var confFilePath = Path.Combine(solutionDir.FullName, confFileName);

            var siteDev = Path.Combine(solutionDir.FullName, "App", "wwwdev");
            var siteRoot = Path.Combine(solutionDir.FullName, "App", "wwwroot");
            var serviceFile = Path.Combine(solutionDir.FullName, "service");
            var windowsServiceOutputDirectory = Path.Combine(solutionDir.FullName, "AppFramework", "_running-as-service");

            copyConfig(Path.Combine(binFramework, confFileName));
            copyConfig(Path.Combine(binFramework, confFileName));

            void copyConfig(string toPath) {
                if (new FileInfo(toPath).Directory.Exists) {
                    File.Copy(confFilePath, toPath, true);
                }
            }

            void log(string msg) {
                Console.WriteLine(msg);
            }

            log("npm install");
            var npmProcInfo = new ProcessStartInfo() {
                Arguments = $"install",
                FileName = "npm",
                WorkingDirectory = siteDev,
            };
            if (isFramework) {
                npmProcInfo.FileName = @"C:\Program Files\nodejs\npm.cmd";
            }
            try {
                if (!skipNpmInstall) {
                    Process.Start(npmProcInfo).WaitForExit();
                }
            }
            catch (Exception ex){
                if (!isFramework) {
                    log("npm install Windows");
                    npmProcInfo.FileName = @"C:\Program Files\nodejs\npm.cmd";
                    Process.Start(npmProcInfo).WaitForExit();
                }
                else throw ex;
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
            if (!skipWebpack) {
                Process.Start(webpackProcInfo).WaitForExit();
            }

            if (isFramework) {
                copyFiles(binFramework);
            }
            if (isCore) {
                copyFiles(binCore);
            }

            void copyFiles(string bin) {
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
