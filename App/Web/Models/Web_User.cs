using AppNS.Database.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppNS.Web.Models
{
    public class Web_User
    {
        public int id { get; set; }
        public string name { get; set; }
        public bool? enabled { get; set; }
        public bool? isAdmin { get; set; }
        public string email { get; set; }

        public void PullData(User from) {
            Util.CopyPropsIgnoreCase(from, this);
        }

        public void PushData(User to) {
            Util.CopyPropsIgnoreCase(this, to);
        }
    }
}
