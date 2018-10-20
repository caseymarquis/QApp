using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppNS.Web {
    public class AuthException : Exception {
        public AuthException(string msg) : base(msg) { }
    }
}
