using System;
using System.Diagnostics;
using System.Threading;
using Kiwi.Markdown.ContentProviders;
using Nancy.Hosting.Self;
using WelcomePage.Core;

namespace WelcomePage.WebServer
{
    static class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("WelcomePage.WebServer url root-directory");
                return;
            }

            var url = new Uri(args[0]);
            var rootDirectory = args[1];

            var stop = new ManualResetEvent(false);
            Console.CancelKeyPress += (sender, e) =>
                {
                    Console.WriteLine("^C");
                    stop.Set();
                };

            var innerProvider = new FileContentProvider(rootDirectory);
            var defaultDocumentPolicy = new DefaultDocumentPolicy();
            var contentProvider = new ContentProvider(innerProvider, defaultDocumentPolicy, rootDirectory);

            var bootstrapper = new Bootstrapper(contentProvider);
            var configuration = new HostConfiguration {RewriteLocalhost = false};
            var host = new NancyHost(bootstrapper, configuration, url);
            host.Start();
            Console.WriteLine("Nancy host listening on '{0}'. Press Ctrl+C to quit.", url);

            Process.Start(url.ToString());

            stop.WaitOne();

            host.Stop();
        }
    }
}
