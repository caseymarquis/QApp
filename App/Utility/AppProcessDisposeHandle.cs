using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QApp.Utility {
    public class AppProcessDisposeHandle {
        Func<Task> actuallyDisposeProcess;
        private AppProcess process; //This is really just here for debugging. It's not used for anything.

        public AppProcessDisposeHandle(Func<Task> _actuallyDisposeProcess, AppProcess _process) {
            this.actuallyDisposeProcess = _actuallyDisposeProcess;
            this.process = _process;
        }

        public async Task DisposeProcess() {
            await actuallyDisposeProcess();
        }

        private object lockEverything = new object();
        private bool m_MustDispose;
        public bool MustDispose {
            get {
                lock (lockEverything) {
                    return m_MustDispose;
                }
            }
            set {
                lock (lockEverything) {
                    m_MustDispose = value;
                }
            }
        }

        public string ProcessName => process?.ProcessName ?? "null";
    }
}
