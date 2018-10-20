using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppNS.Web {
    public class Web_AuthResponse<T> {
        public bool tokenNotFound { get; set; }
        public bool notAuthorized { get; set; }
        public T data { get; set; }
    }
}
