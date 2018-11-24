using QApp;
using System;
using System.Threading.Tasks;

namespace AppCore
{
    class Program
    {
        public static async Task Main(string[] args) {
            var app = new App();
            await app.Run(logToDisk: false);
        }
    }
}
