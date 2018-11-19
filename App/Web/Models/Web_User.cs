using QApp.Database.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace QApp.Web.Models
{
    public class Web_User
    {
        public int id { get; set; }
        public string name { get; set; }
        public bool? enabled { get; set; }
        public bool? isAdmin { get; set; }
        public string email { get; set; }

        public void PullData(User from) {
            KC.Ricochet.Util.CopyProps(from, this);
        }

        public void PushData(User to) {
            KC.Ricochet.Util.CopyProps(this, to);
        }
    }
}
