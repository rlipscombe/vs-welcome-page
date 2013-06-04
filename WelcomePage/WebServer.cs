using System;
using Nancy.Hosting.Self;
using WelcomePage.Core;

namespace RogerLipscombe.WelcomePage
{
    public class WebServer : IWebServer
    {
        private NancyHost _host;

        public void Start(Uri url, string rootFolder)
        {
            if (_host == null)
            {
                var bootstrapper = new Bootstrapper();
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
    }
}