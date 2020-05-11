using System;
using System.Threading.Tasks;

namespace QApp
{
    class Program
    {
        public static async Task Main(string[] args) {
            var app = new App();
            await app.Run(logToDisk: false);
        }
    }
}
