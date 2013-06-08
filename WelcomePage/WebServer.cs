using System;
using Kiwi.Markdown.ContentProviders;
using Nancy.Hosting.Self;
using WelcomePage.Core;

namespace RogerLipscombe.WelcomePage
{
    public sealed class WebServer : IWebServer
    {
        private NancyHost _host;

        public void Start(Uri url, string rootFolder)
        {
            if (_host == null)
            {
                var bootstrapper =
                    new Bootstrapper(new ContentProvider(new FileContentProvider(rootFolder),
                                                         new DefaultDocumentPolicy(), rootFolder));
                var configuration = new HostConfiguration { RewriteLocalhost = false };
                _host = new NancyHost(bootstrapper, configuration, url);
                _host.Start();
            }
        }

        public void Stop()
        {
            if (_host != null)
                _host.Stop();
            _host = null;
        }

        public void Dispose()
        {
            Stop();
        }
    }
}