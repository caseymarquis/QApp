using KC.BaseDb;
using QApp.Database.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace QApp.Database.Models {
    public class User : BaseModelWithDates {

        public string Email { get; set; }
        public string PasswordHash { get; set; }

        public bool Enabled { get; set; }
        public bool ResetPassword { get; set; }

        public string Name { get; set; }
        public string SessionToken { get; set; }
        public DateTime SessionTokenExpiresUtc { get; set; } = KC.BaseDb.Util.InitDateTime;
        public Guid PasswordSalt { get; set; }

        public bool IsAdmin { get; set; }
    }
}
