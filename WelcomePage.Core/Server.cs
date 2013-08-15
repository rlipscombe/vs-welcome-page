using System;
using Nancy.Hosting.Self;

namespace WelcomePage.Core
{
    public class Server : MarshalByRefObject, IServer
    {
        private readonly Uri _url;
        private readonly string _rootDirectory;

        private NancyHost _host;

        public Server(Uri url, string rootDirectory)
        {
            _url = url;
            _rootDirectory = rootDirectory;
        }

        public void Start()
        {
            var bootstrapper = new Bootstrapper(_rootDirectory);
            var configuration = new HostConfiguration { RewriteLocalhost = false };
            _host = new NancyHost(bootstrapper, configuration, _url);
            _host.Start();
        }

        public void Stop()
        {
            _host.Stop();
        }
    }
}
