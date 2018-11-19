using QApp.Web;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KC.NanoProcesses;

namespace QApp.Processes {
    [NanoDI]
    public class WebSite_Run : NanoProcess {
        public override string ProcessName => nameof(WebSite_Run);
        protected override TimeSpan RunDelay => new TimeSpan(0, 0, 30);

        private CancellationTokenSource cTokenSource = new CancellationTokenSource();

        protected override Task OnInit(NpUtil util) {
            var cToken = cTokenSource.Token;            

            var contentRoot =
                Path.Combine(Directory.GetCurrentDirectory());
            var host = WebHost.CreateDefaultBuilder();

            host.UseContentRoot(contentRoot)
            .UseStartup<AppWebService>()
            .UseUrls(App.Config.LocalUrl) //Use a reverse proxy to get this to port 443 with SSL.
            .Build()
            .RunAsync(cToken);
            return Task.FromResult(0);
        }

        protected override Task OnDispose(NpUtil util) {
            cTokenSource.Cancel();
            return Task.FromResult(0);
        }

        protected override Task OnRun(NpUtil util) {
            return Task.FromResult(0);
        }
    }
}
