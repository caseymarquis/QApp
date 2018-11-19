using KC.NanoProcesses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QApp.Processes {
    [NanoDI]
    public class Time_ResetCachedTimeZone : NanoProcess {
        public override string ProcessName => nameof(Time_ResetCachedTimeZone);
        protected override TimeSpan RunDelay => new TimeSpan(0, 10, 0);

        protected async override Task OnInit(NpUtil util) {
            await Task.FromResult(0);
        }

        protected async override Task OnRun(NpUtil util) {
            System.Globalization.CultureInfo.CurrentCulture.ClearCachedData();
            await Task.FromResult(0);
        }

        protected async override Task OnDispose(NpUtil util) {
            await Task.FromResult(0);
        }

    }
}
