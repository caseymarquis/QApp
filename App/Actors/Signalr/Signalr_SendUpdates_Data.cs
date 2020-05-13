using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QApp.Actors.Signalr {
    public struct Update {
        public string Group;
        public string Cmd;
    }

    public interface IUpdateHubClient {
        Task receiveUpdates(List<string> updateCommands);
    }

    public class UpdateHub : Hub<IUpdateHubClient> {
        public async Task renew(string groups) {
            if (App.Director.TryGetSingleton<Signalr_PubSub>(out var signalr_PubSub)){
                foreach (var group in groups.Split('|')) {
                    signalr_PubSub.SubscribeToBroadcastGroupFor15Minutes(group);
                    await this.Groups.AddToGroupAsync(this.Context.ConnectionId, group);
                }
            }
        }

        public async Task subscribe(string group) {
            if (App.Director.TryGetSingleton<Signalr_PubSub>(out var signalr_PubSub)){
                signalr_PubSub.SubscribeToBroadcastGroupFor15Minutes(group);
            }
            await this.Groups.AddToGroupAsync(this.Context.ConnectionId, group);
        }

        public async Task unsubscribe(string group) {
            await this.Groups.RemoveFromGroupAsync(this.Context.ConnectionId, group);
        }
    }
}
