using System;
using System.IO;
using System.Reflection;
using System.Security.Policy;
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
            //var info = AppDomain.CurrentDomain.SetupInformation;
            var info = new AppDomainSetup();
            info.ApplicationBase = applicationBase;

            var domain = AppDomain.CreateDomain("WelcomePage.WebServer", null, info);
            _server = (IServer) domain.CreateInstanceAndUnwrap(
                "WelcomePage.Core", "WelcomePage.Core.Server",
                false,
                BindingFlags.Instance| BindingFlags.Public | BindingFlags.CreateInstance,
                null,
                new object[] { url, rootFolder },
                null, null);
            _server.Start();
        }

        public void Stop()
        {
            _server.Stop();
        }
    }
}