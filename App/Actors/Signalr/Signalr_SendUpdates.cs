using KC.Actin;
using Microsoft.AspNetCore.SignalR;
using QApp.Actors.Signalr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QApp.Actors {
    [Singleton]
    public class Signalr_SendUpdates : Actor {
        protected override TimeSpan RunDelay => new TimeSpan(0, 0, 1); //NOTE: You can adjust this to rate limit how often signalr updates send.

        protected override async Task OnInit(ActorUtil util) {
            while (this.hubContext == null) {
                if (App.Director.TryGetSingleton<IHubContext<UpdateHub, IUpdateHubClient>>(out var hubContext)) {
                    this.hubContext = hubContext;
                }
                else {
                    await Task.Delay(50);
                }
            }
        }

        private MessageQueue<Update> updates = new MessageQueue<Update>();
        private IHubContext<UpdateHub, IUpdateHubClient> hubContext;

        public void Send_Sample_Update(int id) {
            this.AddUpdate($"sample/{id}", "update/sample");
        }

        /// <summary>
        /// Create a new public function when you need to call this.
        /// This prevents magic strings from propagating all over the application.
        /// </summary>
        private void AddUpdate(string group, string cmd) {
            this.updates.Enqueue(new Update() {
                Group = group,
                Cmd = $"{group}|{cmd}",
            });
        }

        private long pingCount = 0;
        protected override async Task OnRun(ActorUtil util) {
            pingCount++;
            if ((pingCount % 5) == 0) {
                await hubContext.Clients.All.receiveUpdates(new List<string>() { "ping|ping" });
            }

            if (!updates.TryDequeueAll(out var toSend)) {
                return;
            }
            var updateGroups = toSend.GroupBy(update => update.Group).ToList();
            foreach (var updateGroup in updateGroups) {
                var groupName = updateGroup.Key;
                var cmds = updateGroup.Select(update => update.Cmd).Distinct().ToList();
                await hubContext.Clients.Group(groupName).receiveUpdates(cmds);
            }
        }
    }
}
