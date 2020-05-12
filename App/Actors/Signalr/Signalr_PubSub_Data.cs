using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QApp.Actors.Signalr {
    public class SignalrPubSubRecord {
        public DateTimeOffset LastRequested;
        public IDisposable Subscription;
    }
}
