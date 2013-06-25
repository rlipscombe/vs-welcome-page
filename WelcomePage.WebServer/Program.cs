using System;
using System.Diagnostics;
using System.Threading;
using Nancy.Hosting.Self;
using WelcomePage.Core;

namespace WelcomePage.WebServer
{
    static class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length != 2 && args.Length != 3)
            {
                Console.WriteLine("WelcomePage.WebServer url root-directory [-open]");
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
                };

            var bootstrapper = new Bootstrapper(rootDirectory);
            var configuration = new HostConfiguration { RewriteLocalhost = false };
            var host = new NancyHost(bootstrapper, configuration, url);
            host.Start();
            Console.WriteLine("Nancy host listening on '{0}'. Press Ctrl+C to quit.", url);

            if (openBrowser)
                Process.Start(url.ToString());

            stop.WaitOne();

            host.Stop();
        }
    }
}
