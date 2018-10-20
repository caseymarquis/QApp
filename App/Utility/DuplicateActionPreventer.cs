using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Utility {
    public class DuplicateActionPreventer<T> {
        private object lockEverything = new object();
        private List<T> activeTransactions = new List<T>();

        public async Task<U> ExclusiveAction<U>(T lockObject, Func<Task<U>> doThis) {
            lock (lockEverything) {
                if (activeTransactions.Contains(lockObject)) {
                    throw new ApplicationException("Duplicate action was prevented.");
                }
                activeTransactions.Add(lockObject);
            }
            try {
                return await doThis();
            }
            finally {
                lock (lockEverything) {
                    activeTransactions.Remove(lockObject);
                }
            }
        }

        public async Task ExclusiveAction<U>(T lockObject, Func<Task> doThis) {
            await this.ExclusiveAction(lockObject, async () => {
                await doThis();
                return 0;
            });
        }
    }
}
