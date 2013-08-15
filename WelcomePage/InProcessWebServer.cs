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
            if (_server != null)
                return;

            const string appDomainFriendlyName = "WelcomePage.WebServer";
            var applicationBase = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var info = new AppDomainSetup
                {
                    ApplicationName = appDomainFriendlyName,
                    ApplicationBase = applicationBase
                };

            var domain = AppDomain.CreateDomain(appDomainFriendlyName, null, info);
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;

            _server = (IServer) domain.CreateInstanceAndUnwrap(
                "WelcomePage.Core", "WelcomePage.Core.Server",
                false,
                BindingFlags.Instance| BindingFlags.Public | BindingFlags.CreateInstance,
                null,
                new object[] { url, rootFolder },
                null, null);
            _server.Start();
        }

        /// <summary>
        /// See http://www.west-wind.com/weblog/posts/2009/Jan/19/Assembly-Loading-across-AppDomains
        /// ...except that, it turns out, we didn't need all of that.
        /// </summary>
        private Assembly AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return Assembly.Load(args.Name);
        }

        public void Stop()
        {
            if (_server != null)
                _server.Stop();
        }
    }
}