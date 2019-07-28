using QApp.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Actin;

namespace QApp.Processes {

    [Singleton]
    public class Db_CheckIfUp : Actor {
        private Atom<bool> m_DbIsUp = new Atom<bool>();
        public bool DbIsUp => m_DbIsUp.Value;

        protected override TimeSpan RunDelay => new TimeSpan(0, 0, 1);

        public async Task<bool> UpdateAndReturnIfDbIsUp() {
            try {
                await AppDbContext.WithContext(async db => {
                    var userId = await db.Users.Select(x => x.Id).FirstOrDefaultAsync();
                });
                m_DbIsUp.Value = true;
                return true;
            }
            catch (SqlException) {
                m_DbIsUp.Value = false;
                return false;
            }
        }

        protected override async Task OnRun(ActorUtil util) {
            await UpdateAndReturnIfDbIsUp();
        }
    }
}
