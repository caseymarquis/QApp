using QApp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ServiceInstallNS {
    public class AppServiceInstaller {
        public void Install() {
            try {
                Console.WriteLine("Starting Installation");
                var serviceStopper = new ServiceStopper();
                if (!serviceStopper.StopTheService()) {
                    return;
                }
                
                if (new ServiceExistsChecker().ServiceExists()) {
                    Console.WriteLine("Service Already Installed");
                }
                else {
                    Console.WriteLine("Installing Service");
                    var bestInstallUtil = new InstallUtilGetter().getBestInstallUtil();
                    if (bestInstallUtil == null) {
                        return;
                    }
                    var execFilePath = Assembly.GetEntryAssembly().Location;
                    if (!new RunCmdProcess().RunProcess(bestInstallUtil.FullName, execFilePath, Path.GetFullPath(".\\"))) {
                        Console.WriteLine("Error installing service.");
                        Console.WriteLine("To manually install the service, run:");
                        Console.WriteLine($"\"{bestInstallUtil.FullName}\" \"{execFilePath}\"");
                        return;
                    }
                    else {
                        Console.WriteLine("Service Installed Successfully.");
                    }
                }

                if (new ServiceStarter().StartTheService()) {
                    Console.WriteLine("Successfully installed!");
                }
                else {
                    Console.WriteLine($"Successfully installed, but unable to start {App.Config.AppName} Service.");
                    Console.WriteLine("Try starting the service manually.");
                    Console.WriteLine($"If unable to start the service, contact {App.Config.AppName} support.");
                }

            }
            catch (Exception ex) {
                Console.WriteLine("Installation failed with error: " + ex.Message); 
            }
        }

        public void TryStopService() {
            Console.WriteLine("Stopping Service");
            var serviceStopper = new ServiceStopper();
            if (!serviceStopper.StopTheService()) {
            }
        }
    }
}
