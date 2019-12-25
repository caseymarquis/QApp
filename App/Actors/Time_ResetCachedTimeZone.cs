using KC.Actin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QApp.Actors {
    [Singleton]
    public class Time_ResetCachedTimeZone : Actor {
        protected override TimeSpan RunDelay => new TimeSpan(0, 10, 0);

        protected async override Task OnRun(ActorUtil util) {
            System.Globalization.CultureInfo.CurrentCulture.ClearCachedData();
            await Task.FromResult(0);
        }
    }
}
