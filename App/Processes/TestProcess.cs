using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppNS.Processes {
    public class TestProcess : AppProcess {
        public override string ProcessName => nameof(TestProcess);
        protected override TimeSpan RunDelay => new TimeSpan(0, 0, 2);

        protected async override Task OnInit() {
            Console.WriteLine("Test-Init");
            await Task.FromResult(0);
        }

        protected async override Task OnRun() {
            await Task.FromResult(0);
        }

        protected async override Task OnDispose() {
            Console.WriteLine("Test-Dispose");
            await Task.FromResult(0);
        }

    }
}
