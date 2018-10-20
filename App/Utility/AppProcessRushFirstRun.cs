using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App {
    /// <summary>
    /// This is an AppProcess which spams attempts to complete its initial run,
    /// then runs at a slower interval afterwards. This is useful for processes
    /// which check or maintain something which other systems depend on.
    /// </summary>
    public abstract class AppProcessRushFirstRun : AppProcess {

        private bool firstRunCompleted = false;
        protected abstract TimeSpan RunDelayAfterFirstRun { get; }

        protected abstract Task<bool> OnRun_ReturnTrueIfCompleted();

        protected override TimeSpan RunDelay => firstRunCompleted? RunDelayAfterFirstRun : new TimeSpan(0, 0, 0, 0, 100);

        protected override async Task OnRun() {
            var complete = await OnRun_ReturnTrueIfCompleted();
            if (!firstRunCompleted && complete) {
                firstRunCompleted = true;
            }
        }
    }
}
