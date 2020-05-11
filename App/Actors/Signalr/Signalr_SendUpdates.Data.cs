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
        public async Task subscribe(string group) {
            //TODO: Optionally, ensure that the client is allowed to subscribe to said group.
            //Ideally, you shouldn't be sending data with SignalR, just simple notifications that an update
            //exists. As such, there's not a whole lot of point in checking first.
            //TODO: Notify pubsub
            await this.Groups.AddToGroupAsync(this.Context.ConnectionId, group);
        }

        public async Task unsubscribe(string group) {
            //TODO: Notify pubsub
            await this.Groups.RemoveFromGroupAsync(this.Context.ConnectionId, group);
        }
    }
}
