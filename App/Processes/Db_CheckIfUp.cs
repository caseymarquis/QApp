using QApp.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QApp.Processes {
    public class Db_CheckIfUp : AppProcess {
        object lockEverything = new object();
        private bool m_DbIsUp;
        public bool DbIsUp {
            get {
                lock (lockEverything) {
                    return m_DbIsUp;
                }
            }
        }

        public async Task<bool> UpdateAndReturnIfDbIsUp() {
            try {
                await AppDbContext.WithContext(async db => {
                    var userId = await db.Users.Select(x => x.Id).FirstOrDefaultAsync();
                });
                lock (lockEverything) {
                    m_DbIsUp = true;
                    return true;
                }
            }
            catch (SqlException) {
                lock (lockEverything) {
                    m_DbIsUp = false;
                    return false;
                }
            }
        }

        public override string ProcessName => nameof(Db_CheckIfUp);

        protected override TimeSpan RunDelay => new TimeSpan(0, 0, 1);

        protected override Task OnDispose() {
            return Task.FromResult(0);
        }

        protected override Task OnInit() {
            return Task.FromResult(0);
        }

        protected override async Task OnRun() {
            await UpdateAndReturnIfDbIsUp();
        }
    }
}
