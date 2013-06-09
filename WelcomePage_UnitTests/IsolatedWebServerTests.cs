using System;
using NUnit.Framework;
using RogerLipscombe.WelcomePage;

namespace WelcomePage_UnitTests
{
    [TestFixture]
    [Ignore("TODO: How to (simply) marshal the results back from the other appdomain?")]
    public class IsolatedWebServerTests
    {
        [Test]
        public void StartedInIsolatedAppDomain()
        {
            // Arrange
            MockWebServer.Reset();
            const string appDomainFriendlyName = "Isolated.WebServer";
            var server = new IsolatedWebServer(appDomainFriendlyName, typeof(MockWebServer));

            // Act
            var url = new Uri("http://localhost:10150");
            server.Start(url, @"Z:\some\path");

            // Assert
            Assert.AreEqual(MockWebServerState.Started, MockWebServer.State);
            Assert.AreEqual(appDomainFriendlyName, MockWebServer.AppDomainName);
        }

        [Test]
        public void Stopping()
        {
            // Arrange
            MockWebServer.Reset();
            const string appDomainFriendlyName = "Isolated.WebServer";
            var server = new IsolatedWebServer(appDomainFriendlyName, typeof(MockWebServer));
            var url = new Uri("http://localhost:10150");
            server.Start(url, @"Z:\some\path");

            // Act
            server.Stop();

            // Assert
            Assert.AreEqual(MockWebServerState.Stopped, MockWebServer.State);
        }

        /// <summary>
        /// Injected into the other app domain, so we can check it's working.
        /// </summary>
        public class MockWebServer : MarshalByRefObject, IWebServer
        {
            public void Dispose()
            {
                Stop();
            }

            public void Start(Uri url, string rootFolder)
            {
                AppDomainName = AppDomain.CurrentDomain.FriendlyName;
                State = MockWebServerState.Started;
            }

            public void Stop()
            {
                AppDomainName = AppDomain.CurrentDomain.FriendlyName;
                State = MockWebServerState.Stopped;
            }

            public static void Reset()
            {
                State = MockWebServerState.Invalid;
                AppDomainName = "";
            }

            public static string AppDomainName { get; set; }
            public static MockWebServerState State { get; set; }
        }

        public enum MockWebServerState
        {
            Invalid, Started, Stopped
        }
    }
}