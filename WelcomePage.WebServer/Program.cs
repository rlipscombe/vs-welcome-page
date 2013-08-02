using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using WelcomePage.Core;

namespace WelcomePage.WebServer
{
    static class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length != 2 && args.Length != 3)
            {
                Console.WriteLine(@"Usage:");
                Console.WriteLine(@"  WelcomePage.WebServer url root-directory [-open]");
                Console.WriteLine();
                Console.WriteLine(@"Example:");
                Console.WriteLine(@"  WelcomePage.WebServer http://localhost:12540 C:\Users\roger\Source\vs-welcome-page\WelcomePage_UnitTests\Samples -open");
                return;
            }

            var url = new Uri(args[0]);
            var rootDirectory = args[1];
            bool openBrowser = args.Length == 3 && args[2].Equals("-open", StringComparison.InvariantCultureIgnoreCase);

            var stop = new ManualResetEvent(false);
            Console.CancelKeyPress += (sender, e) =>
                {
                    Console.WriteLine("^C");
                    stop.Set();

                    // Don't run the default shutdown.
                    e.Cancel = true;
                };

            var domain = AppDomain.CreateDomain("WelcomePage.WebServer");
            var server =
                (IServer)
                domain.CreateInstanceAndUnwrap("WelcomePage.Core", "WelcomePage.Core.Server", false,
                                               BindingFlags.CreateInstance, null, new object[] { url, rootDirectory },
                                               null, null);
            server.Start();
            Console.WriteLine("Nancy host listening on '{0}'. Press Ctrl+C to quit.", url);

            if (openBrowser)
                Process.Start(url.ToString());

            stop.WaitOne();
            server.Stop();
        }
    }
}
