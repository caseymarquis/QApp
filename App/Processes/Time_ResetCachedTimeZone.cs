using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppNS.Processes {
    public class Time_ResetCachedTimeZone : AppProcess {
        public override string ProcessName => nameof(Time_ResetCachedTimeZone);
        protected override TimeSpan RunDelay => new TimeSpan(0, 10, 0);

        protected async override Task OnInit() {
            await Task.FromResult(0);
        }

        protected async override Task OnRun() {
            System.Globalization.CultureInfo.CurrentCulture.ClearCachedData();
            await Task.FromResult(0);
        }

        protected async override Task OnDispose() {
            await Task.FromResult(0);
        }

    }
}
