using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using App.Database;
using App.Web;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace App {
    class Program {
        static async Task Main(string[] args) {
            var app = new App();
            await app.Run();
        }
    }
}
