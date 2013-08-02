using System;
using System.IO;
using System.Reflection;
using WelcomePage.Core;

namespace RogerLipscombe.WelcomePage
{
    public sealed class InProcessWebServer : IWebServer
    {
        private IServer _server;

        public void Dispose()
        {
            Stop();
        }

        public void Start(Uri url, string rootFolder)
        {
            var applicationBase = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var info = new AppDomainSetup { ApplicationBase = applicationBase };

            var domain = AppDomain.CreateDomain("WelcomePage.WebServer", null, info);
            var oh = domain.CreateInstance("WelcomePage.Core", "WelcomePage.Core.Server", false, BindingFlags.CreateInstance, null,
                                           new object[] { url, rootFolder }, null, null);
            _server = (IServer) oh.Unwrap();
//            _server = domain.CreateInstanceAndUnwrap<IServer>("WelcomePage.Core", "WelcomePage.Core.Server", new object[] { url, rootFolder });
            _server.Start();
        }

        public void Stop()
        {
            _server.Stop();
        }
    }
}