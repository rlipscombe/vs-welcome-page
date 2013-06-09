using System;

namespace RogerLipscombe.WelcomePage
{
    public sealed class WelcomePageImpl : IDisposable
    {
        private readonly ISolutionFolder _solutionFolder;
        private readonly IDefaultDocumentPolicy _defaultDocumentPolicy;
        private readonly IItemOperations _itemOperations;
        private readonly IWebServer _server;
        private Uri _url;

        public WelcomePageImpl(ISolutionFolder solutionFolder, IDefaultDocumentPolicy defaultDocumentPolicy, IItemOperations itemOperations, IWebServer server)
        {
            _solutionFolder = solutionFolder;
            _itemOperations = itemOperations;
            _defaultDocumentPolicy = defaultDocumentPolicy;
            _server = server;
        }

        public void Dispose()
        {
            _server.Stop();
        }

        /// <summary>
        /// Called when the solution is opened.
        /// </summary>
        public void OnAfterOpenSolution()
        {
            _url = GenerateUrl();

            // Start the web server.
            string solutionDirectoryName = _solutionFolder.GetDirectoryName();
            _server.Start(_url, solutionDirectoryName);

            if (_defaultDocumentPolicy.ContainsDefaultDocument(solutionDirectoryName))
            {
                // Open the web browser.
                Navigate(_url);
            }
        }

        private void Navigate(Uri url)
        {
            // TODO: How do we (should we?) bring it to the front?
            // TODO: If we don't bring it to the front, how do we bring it to the user's attention?
            Log.Message("Navigating to '{0}'", url);
            _itemOperations.Navigate(url.ToString());
        }

        private static Uri GenerateUrl()
        {
            // Figure out a port number.
            var random = new Random();
            var port = random.Next(10000, short.MaxValue);
            var url = new Uri(string.Format("http://localhost:{0}/", port));
            Log.Message("Using URL '{0}'", url);
            return url;
        }

        /// <summary>
        /// Called from the menu command.
        /// </summary>
        public void OnViewWelcomePage()
        {
            // TODO: The menu item should be disabled; this'll do for now.
            if (_url == null)
                return;

            // If the server's not running, restart it:
            string solutionDirectoryName = _solutionFolder.GetDirectoryName();
            _server.Start(_url, solutionDirectoryName);

            // Open the web browser.
            Navigate(_url);
        }

        public void OnAfterCloseSolution()
        {
            // TODO: Close the browser window.

            _server.Stop();
        }
    }

    // TODO: Find some way to merge this with the one in Core, without bringing in the rest
    // of the references.
    public interface IDefaultDocumentPolicy
    {
        bool ContainsDefaultDocument(string solutionDirectoryName);
    }
}
