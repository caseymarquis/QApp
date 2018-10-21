using QApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ServiceInstallNS {
    public class ServiceExistsChecker {
        public bool ServiceExists() {
            ServiceController service = null;
            try {
                service = ServiceController.GetServices().FirstOrDefault(x => x.ServiceName == App.Config.AppName);
                return service != null;
            }
            catch (Exception ex) {
                Console.WriteLine($"Failed to Find the {App.Config.AppName} Service: " + ex.Message);
                return false;
            }
        }
    }
}
