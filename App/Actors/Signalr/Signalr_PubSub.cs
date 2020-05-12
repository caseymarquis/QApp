using KC.Actin;
using Microsoft.AspNetCore.SignalR;
using QApp.Actors.Signalr;
using QApp.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QApp.Actors {
    [Singleton]
    public class Signalr_PubSub : Actor {
        [Singleton] Signalr_SendUpdates signalr_sendUpdates;

        protected override TimeSpan RunDelay => new TimeSpan(0, 0, 1);

        private MessageQueue<string> groupsToSubscribe = new MessageQueue<string>();

        public void SubscribeToBroadcastGroupFor15Minutes(string group) {
            groupsToSubscribe.Enqueue(group);
        }

        public void Publish(string group, string cmd) {
            if (AppDbContext.CanStartPubSubLoop) {
                AppDbContext.Publish(group, $"{myPubSubId}|{group}|{cmd}");
            }
        }

        int count = 0;
        string myPubSubId = Guid.NewGuid().ToString();
        Dictionary<string, SignalrPubSubRecord> subscribedGroups = new Dictionary<string, SignalrPubSubRecord>();

        protected override async Task OnRun(ActorUtil util) {
            if (!AppDbContext.CanStartPubSubLoop) {
                groupsToSubscribe.DequeueAll();
                await Task.FromResult(0);
                return;
            }

            var now = util.Now;

            //Subscribe to groups:
            while (groupsToSubscribe.TryDequeue(out var group)) {
                if (subscribedGroups.TryGetValue(group, out var pubSubRecord)) {
                    pubSubRecord.LastRequested = now;
                }
                else {

                    pubSubRecord = new SignalrPubSubRecord {
                        LastRequested = now,
                    };
                    pubSubRecord.Subscription = AppDbContext.Subscribe(group, notification => {
                        try {
                            var split = notification.Split("|");
                            var fromThisServer = split[0] == myPubSubId;
                            if (!fromThisServer) {
                                signalr_sendUpdates.Send_PubSubBroadcast(split[1], split[2]);
                            }
                        }
                        catch (Exception ex) {
                            util.Log.Error($"Failed to process pubsub notification: {notification}", ex);
                        }
                    });
                    subscribedGroups[group] = pubSubRecord;

                }
            }

            count++;
            if ((count % 15) == 0) {
                //Remove old subscriptions:
                var aWhileAgo = now.AddMinutes(-15);
                if (subscribedGroups.Any(x => x.Value.LastRequested <= aWhileAgo)) {
                    var toUnsubscribe = subscribedGroups.Where(x => x.Value.LastRequested <= aWhileAgo).ToList();
                    foreach (var subscription in toUnsubscribe) {
                        subscription.Value.Subscription?.Dispose();
                        subscribedGroups.Remove(subscription.Key); 
                    }
                }
            }
        }

        protected override async Task OnDispose(ActorUtil util) {
            foreach (var pair in subscribedGroups) {
                pair.Value.Subscription?.Dispose();
            }
            await Task.FromResult(0);
        }

    }
}
