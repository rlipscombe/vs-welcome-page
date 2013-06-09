using System;
using System.Security.Policy;

namespace RogerLipscombe.WelcomePage
{
    public sealed class IsolatedWebServer : IWebServer
    {
        private readonly string _friendlyName;
        private readonly Type _serverType;

        private AppDomain _domain;
        private IWebServer _server;

        public IsolatedWebServer(string friendlyName, Type serverType)
        {
            _friendlyName = friendlyName;
            _serverType = serverType;
        }

        public void Dispose()
        {
            Stop();
        }

        public void Start(Uri url, string rootFolder)
        {
            const Evidence securityInfo = null;
            var setup = new AppDomainSetup
                {
                    ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase
                };
            _domain = AppDomain.CreateDomain(_friendlyName, securityInfo, setup);

            var assemblyName = _serverType.Assembly.FullName;
            var typeName = _serverType.FullName;

            _server = (IWebServer)_domain.CreateInstanceAndUnwrap(assemblyName, typeName);
            _server.Start(url, rootFolder);
        }

        public void Stop()
        {
            if (_server != null)
            {
                _server.Stop();
                _server = null;
            }

            if (_domain != null)
            {
                AppDomain.Unload(_domain);
                _domain = null;
            }
        }
    }
}