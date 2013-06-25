using System;
using Moq;
using NUnit.Framework;
using RogerLipscombe.WelcomePage;
using IDefaultDocumentPolicy = RogerLipscombe.WelcomePage.IDefaultDocumentPolicy;

namespace WelcomePage_UnitTests
{
    [TestFixture]
    public class PackageImplTests
    {
        /// <summary>
        /// Quick smoke test -- can it actually be constructed?
        /// </summary>
        [Test]
        public void CreateInstance()
        {
            var impl = new WelcomePageImpl(Mock.Of<ISolutionFolder>(), Mock.Of<IDefaultDocumentPolicy>(),
                                           Mock.Of<IItemOperations>(), Mock.Of<IWebServer>());
        }

        /// <summary>
        /// If you open a solution and there's no readme file, then the server may not be started and the browser should not be opened.
        /// </summary>
        [Test]
        public void OpenSolutionWithNoReadMe()
        {
            // Arrange
            var itemOperations = new Mock<IItemOperations>();
            var solutionFolder = new Mock<ISolutionFolder>();
            var policy = new Mock<IDefaultDocumentPolicy>();
            const string rootFolder = @"Z:\some\path";
            solutionFolder.Setup(x => x.GetDirectoryName())
                          .Returns(rootFolder);
            policy.Setup(x => x.ContainsDefaultDocument(It.IsAny<string>()))
                  .Returns(false);
            var server = new Mock<IWebServer>();

            var impl = new WelcomePageImpl(solutionFolder.Object, policy.Object, itemOperations.Object, server.Object);

            // Act
            impl.OnAfterOpenSolution();

            // Assert
            server.Verify(x => x.Start(It.IsAny<Uri>(), It.IsAny<string>()), Times.AtMostOnce());
            itemOperations.Verify(x => x.Navigate(It.IsAny<string>()), Times.Never());
        }

        /// <summary>
        /// If you open a solution and there's a valid readme file, then the web server should be started,
        /// and the browser should be opened.
        /// </summary>
        [Test]
        public void OpenSolutionWithReadMe()
        {
            // Arrange
            var itemOperations = new Mock<IItemOperations>();
            var solutionFolder = new Mock<ISolutionFolder>();
            var policy = new Mock<IDefaultDocumentPolicy>();
            const string rootFolder = @"Z:\some\path";
            solutionFolder.Setup(x => x.GetDirectoryName())
                          .Returns(rootFolder);
            policy.Setup(x => x.ContainsDefaultDocument(It.IsAny<string>()))
                  .Returns(true);
            var server = new Mock<IWebServer>();
            var impl = new WelcomePageImpl(solutionFolder.Object, policy.Object, itemOperations.Object, server.Object);

            // Act
            impl.OnAfterOpenSolution();

            // Assert
            server.Verify(x => x.Start(It.IsAny<Uri>(), rootFolder), Times.Once());
            itemOperations.Verify(x => x.Navigate(It.IsAny<string>()), Times.Once());
        }

        /// <summary>
        /// If you use the menu item and there's a readme, then the browser should be opened.
        /// </summary>
        [Test]
        public void OpenWelcomePageWithReadMe()
        {
            // Arrange
            var itemOperations = new Mock<IItemOperations>();
            var solutionFolder = new Mock<ISolutionFolder>();
            var policy = new Mock<IDefaultDocumentPolicy>();
            const string rootFolder = @"Z:\some\path";
            solutionFolder.Setup(x => x.GetDirectoryName())
                          .Returns(rootFolder);
            policy.Setup(x => x.ContainsDefaultDocument(It.IsAny<string>()))
                  .Returns(true);
            var server = new Mock<IWebServer>();
            var impl = new WelcomePageImpl(solutionFolder.Object, policy.Object, itemOperations.Object, server.Object);

            // Act
            impl.OnAfterOpenSolution();
            impl.OnViewWelcomePage();

            // Assert
            // Server should be started.
            server.Verify(x => x.Start(It.IsAny<Uri>(), rootFolder), Times.AtLeastOnce());
            // Browser should be opened twice: once for solution, once for menu item.
            itemOperations.Verify(x => x.Navigate(It.IsAny<string>()), Times.Exactly(2));
        }

        /// <summary>
        /// If you use the menu item and there's no readme, then the server should be started and the browser should be opened.
        /// </summary>
        [Test]
        public void OpenWelcomePageWithNoReadMe()
        {
            // Arrange
            var itemOperations = new Mock<IItemOperations>();
            var solutionFolder = new Mock<ISolutionFolder>();
            var policy = new Mock<IDefaultDocumentPolicy>();
            const string rootFolder = @"Z:\some\path";
            solutionFolder.Setup(x => x.GetDirectoryName())
                          .Returns(rootFolder);
            policy.Setup(x => x.ContainsDefaultDocument(It.IsAny<string>()))
                  .Returns(false);
            var server = new Mock<IWebServer>();
            var impl = new WelcomePageImpl(solutionFolder.Object, policy.Object, itemOperations.Object, server.Object);

            // Act
            impl.OnAfterOpenSolution();
            impl.OnViewWelcomePage();

            // Assert
            server.Verify(x => x.Start(It.IsAny<Uri>(), rootFolder), Times.AtLeastOnce());
            itemOperations.Verify(x => x.Navigate(It.IsAny<string>()), Times.Once());
        }

        /// <summary>
        /// If you use the menu item and there's no longer a readme, then the browser should be opened.
        /// </summary>
        [Test]
        public void OpenWelcomePageWithDeletedReadMe()
        {
            // Arrange
            var itemOperations = new Mock<IItemOperations>();
            var solutionFolder = new Mock<ISolutionFolder>();
            var policy = new Mock<IDefaultDocumentPolicy>();
            const string rootFolder = @"Z:\some\path";
            solutionFolder.Setup(x => x.GetDirectoryName())
                          .Returns(rootFolder);
            policy.Setup(x => x.ContainsDefaultDocument(It.IsAny<string>()))
                  .Returns(true);
            var server = new Mock<IWebServer>();
            var impl = new WelcomePageImpl(solutionFolder.Object, policy.Object, itemOperations.Object, server.Object);

            // Act
            impl.OnAfterOpenSolution();
            policy.Setup(x => x.ContainsDefaultDocument(It.IsAny<string>()))
                  .Returns(false);
            impl.OnViewWelcomePage();

            // Assert
            // Server should be started.
            server.Verify(x => x.Start(It.IsAny<Uri>(), rootFolder), Times.AtLeastOnce());
            // Browser should be opened twice -- once for the initial solution opening, and again for the menu item.
            itemOperations.Verify(x => x.Navigate(It.IsAny<string>()), Times.Exactly(2));
        }

        /// <summary>
        /// If you use the menu item and there's a new readme, then the server should be started and the browser should be opened.
        /// </summary>
        [Test]
        public void OpenWelcomePageWithNewReadMe()
        {
            // Arrange
            var itemOperations = new Mock<IItemOperations>();
            var solutionFolder = new Mock<ISolutionFolder>();
            var policy = new Mock<IDefaultDocumentPolicy>();
            const string rootFolder = @"Z:\some\path";
            solutionFolder.Setup(x => x.GetDirectoryName())
                          .Returns(rootFolder);
            policy.Setup(x => x.ContainsDefaultDocument(It.IsAny<string>()))
                  .Returns(false);
            var server = new Mock<IWebServer>();
            var impl = new WelcomePageImpl(solutionFolder.Object, policy.Object, itemOperations.Object, server.Object);

            // Act
            impl.OnAfterOpenSolution();
            policy.Setup(x => x.ContainsDefaultDocument(It.IsAny<string>()))
                  .Returns(true);
            impl.OnViewWelcomePage();

            // Assert
            server.Verify(x => x.Start(It.IsAny<Uri>(), rootFolder), Times.AtLeastOnce());
            itemOperations.Verify(x => x.Navigate(It.IsAny<string>()), Times.Once());
        }

        /// <summary>
        /// Invoking the menu item with no solution open should result in nothing happening.
        /// </summary>
        [Test]
        public void OpenWelcomePageWithNoSolution()
        {
            // Arrange
            var itemOperations = new Mock<IItemOperations>();
            var solutionFolder = new Mock<ISolutionFolder>();
            var policy = new Mock<IDefaultDocumentPolicy>();
            const string rootFolder = @"Z:\some\path";
            solutionFolder.Setup(x => x.GetDirectoryName())
                          .Returns(rootFolder);
            policy.Setup(x => x.ContainsDefaultDocument(It.IsAny<string>()))
                  .Returns(true);
            var server = new Mock<IWebServer>();
            var impl = new WelcomePageImpl(solutionFolder.Object, policy.Object, itemOperations.Object, server.Object);

            // Act
            impl.OnViewWelcomePage();

            // Assert
            server.Verify(x => x.Start(It.IsAny<Uri>(), rootFolder), Times.AtMostOnce());
            itemOperations.Verify(x => x.Navigate(It.IsAny<string>()), Times.Never());
        }

        /// <summary>
        /// Closing the solution should stop the web server.
        /// TODO: It should also close the browser.
        /// </summary>
        [Test]
        public void CloseSolution()
        {
            // Arrange
            var itemOperations = new Mock<IItemOperations>();
            var solutionFolder = new Mock<ISolutionFolder>();
            var policy = new Mock<IDefaultDocumentPolicy>();
            const string rootFolder = @"Z:\some\path";
            solutionFolder.Setup(x => x.GetDirectoryName())
                          .Returns(rootFolder);
            policy.Setup(x => x.ContainsDefaultDocument(It.IsAny<string>()))
                  .Returns(false);
            var server = new Mock<IWebServer>();
            var impl = new WelcomePageImpl(solutionFolder.Object, policy.Object, itemOperations.Object, server.Object);
            impl.OnAfterOpenSolution();

            // Act
            impl.OnAfterCloseSolution();

            // Assert
            server.Verify(x => x.Stop(), Times.Once());
        }

        [Test]
        public void DisposeStopsServer()
        {
            // Arrange
            var server = new Mock<IWebServer>();
            var impl = new WelcomePageImpl(Mock.Of<ISolutionFolder>(), Mock.Of<IDefaultDocumentPolicy>(),Mock.Of<IItemOperations>(), server.Object);
            impl.OnAfterOpenSolution();

            // Act
            impl.Dispose();

            // Assert
            server.Verify(x => x.Stop(), Times.Once());
        }

        // TODO: If you open the browser, and there's no readme, then you get the "why don't you write a readme?" page.
    }
}