using KC.Actin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QApp.Actors {
    [Singleton]
    public class Example_SendARandomNumber : Actor {
        [Singleton] Signalr_SendUpdates signalr_sendUpdates;

        protected override TimeSpan RunDelay => new TimeSpan(0, 0, 5);

        Random r = new Random(314);
        string serverId = Guid.NewGuid().ToString();
        protected override async Task OnRun(ActorUtil util) {
            signalr_sendUpdates.Send_RandomNumber($"Server {serverId} sent: {r.Next(1,100)}");
            await Task.FromResult(0);
        }
    }
}
