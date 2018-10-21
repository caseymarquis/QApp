using QApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceInstallNS {
    public class ServiceStopper {
        public bool StopTheService() {
            ServiceController service = null;
            try {
                service = ServiceController.GetServices().FirstOrDefault(x => x.ServiceName == App.Config.AppName);
            }
            catch (Exception ex) {
                Console.WriteLine($"Failed to Find the {App.Config.AppName} Service: " + ex.Message);
                return true;
            }
            if (service == null) {
                Console.WriteLine($"The {App.Config.AppName} Service is not installed.");
                return true;
            }
            try {
                Console.WriteLine("Service Status: " + service.Status.ToString("g"));
                if (service.Status == ServiceControllerStatus.Stopped) {
                    Console.WriteLine("Service was already stopped.");
                    return true;
                }
                try {
                    Console.WriteLine("Stopping Service");
                    service.Stop();
                    var sleepDuration = 200;
                    var tryForMils = 10000;
                    for (int i = 0; i < tryForMils; i += sleepDuration) {
                        Console.Write(".");
                        service.Refresh();
                        if (service.Status == ServiceControllerStatus.Stopped) {
                            break;
                        }
                        //We wouldn't do this in a real application, but this is just an installer:
                        Thread.Sleep(sleepDuration);
                    }
                    Console.WriteLine();
                    if (service.Status != ServiceControllerStatus.Stopped) {
                        Console.WriteLine("Failed to stop service after " + (tryForMils / 1000) + " seconds.");
                        Console.WriteLine($"Try manually stopping the {App.Config.AppName} service, then run the uninstaller again.");
                        Console.WriteLine("If the service is locked in a stopping state, you may need to restart the computer.");
                        return false;
                    }
                    else {
                        Console.WriteLine("Waiting 2 seconds to ensure service is no longer in use.");
                        Thread.Sleep(2000);
                    }
                    return true;
                }
                catch (Exception ex) {
                    Console.WriteLine("Failed to stop service: " + ex.Message);
                    Console.WriteLine($"Try manually stopping the {App.Config.AppName} service, then running this task again.");
                    return false;
                }
            }
            catch (Exception ex) {
                Console.WriteLine($"Failed to Find {App.Config.AppName} Service Status: " + ex.ToString());
                Console.WriteLine($"Try manually stopping the {App.Config.AppName} service, then running this task again.");
                return false;
            }
        }
    }
}
