using System;
using WelcomePage.Core;

namespace RogerLipscombe.WelcomePage
{
    public sealed class WebServer : MarshalByRefObject, IWebServer
    {
        //private NancyHost _host;

        public void Start(Uri url, string rootFolder)
        {
            //if (_host == null)
            //{
            //    var contentProvider = new ContentProvider(new FileContentProvider(rootFolder),
            //                                              new DefaultDocumentPolicy(), rootFolder);
            //    Log.Message("Content provider initialized; rootFolder = '{0}'.", rootFolder);
            //    var bootstrapper = new Bootstrapper(contentProvider);
            //    var configuration = new HostConfiguration { RewriteLocalhost = false };
            //    _host = new NancyHost(bootstrapper, configuration, url);

            //    Log.Message("Starting Nancy host on '{0}'.", url);
            //    _host.Start();
            //    Log.Message("Nancy host listening on '{0}'.", url);
            //}
        }

        public void Stop()
        {
            //if (_host != null)
            //    _host.Stop();
            //_host = null;
        }

        public void Dispose()
        {
            //if (_host != null)
            //    _host.Dispose();
            //_host = null;
        }
    }
}