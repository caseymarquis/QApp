using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppNS {
    class Program {
        public static void Main(string[] args) {
            var app = new App();
            app.Run().Wait();
        }
    }
}
