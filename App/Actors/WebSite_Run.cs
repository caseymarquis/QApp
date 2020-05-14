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
using KC.Actin;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.SignalR;
using QApp.Actors.Signalr;

namespace QApp.Actors {
    [Singleton]
    public class WebSite_Run : Actor {
        protected override TimeSpan RunDelay => new TimeSpan(0, 0, 30);

        private CancellationTokenSource cTokenSource = new CancellationTokenSource();

        protected override Task OnInit(ActorUtil util) {
            var cToken = cTokenSource.Token;            

            var hostBuilder = Host.CreateDefaultBuilder();
            var host = hostBuilder.ConfigureWebHostDefaults(webBuilder => {
                webBuilder
                    .UseStartup<AppWebService>()
                    .UseUrls(App.Config.AppUrl);
            })
            .Build();
            host.RunAsync();

            var hubContext = host.Services.GetService(typeof(IHubContext<UpdateHub, IUpdateHubClient>));
            App.Director.AddSingletonDependency(hubContext, new Type[] { typeof(IHubContext<UpdateHub, IUpdateHubClient>) });

            return Task.FromResult(0);
        }

        protected override Task OnDispose(ActorUtil util) {
            cTokenSource.Cancel();
            return Task.FromResult(0);
        }

        protected override Task OnRun(ActorUtil util) {
            return Task.FromResult(0);
        }
    }
}
