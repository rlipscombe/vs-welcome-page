using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RogerLipscombe.WelcomePage;
using WelcomePage.Core;

namespace WelcomePage_UnitTests
{
    [TestClass]
    public class PackageImplTests
    {
        [TestMethod]
        public void CreateInstance()
        {
            var impl = new WelcomePageImpl(Mock.Of<ISolutionFolder>(), Mock.Of<IDefaultDocumentPolicy>(),
                                           Mock.Of<IItemOperations>(), Mock.Of<IWebServer>());
        }

        /// <summary>
        /// If you open a solution and there's no readme file, then the server should not be started and the browser should not be opened.
        /// </summary>
        [TestMethod]
        public void OpenSolutionWithNoReadMe()
        {
            // Arrange
            var itemOperations = new Mock<IItemOperations>();
            var solutionFolder = new Mock<ISolutionFolder>();
            var policy = new Mock<IDefaultDocumentPolicy>();
            var rootFolder = @"Z:\some\path";
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
        [TestMethod]
        public void OpenSolutionWithReadMe()
        {
            // Arrange
            var itemOperations = new Mock<IItemOperations>();
            var solutionFolder = new Mock<ISolutionFolder>();
            var policy = new Mock<IDefaultDocumentPolicy>();
            var rootFolder = @"Z:\some\path";
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
        [TestMethod]
        public void OpenWelcomePageWithReadMe()
        {
            // Arrange
            var itemOperations = new Mock<IItemOperations>();
            var solutionFolder = new Mock<ISolutionFolder>();
            var policy = new Mock<IDefaultDocumentPolicy>();
            var rootFolder = @"Z:\some\path";
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
            server.Verify(x => x.Start(It.IsAny<Uri>(), rootFolder), Times.Once());
            // Browser should be opened twice: once for soltion, once for menu item.
            itemOperations.Verify(x => x.Navigate(It.IsAny<string>()), Times.Exactly(2));
        }

        /// <summary>
        /// If you use the menu item and there's no readme, then the server should be started and the browser should be opened.
        /// </summary>
        [TestMethod]
        public void OpenWelcomePageWithNoReadMe()
        {
            // Arrange
            var itemOperations = new Mock<IItemOperations>();
            var solutionFolder = new Mock<ISolutionFolder>();
            var policy = new Mock<IDefaultDocumentPolicy>();
            var rootFolder = @"Z:\some\path";
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
            server.Verify(x => x.Start(It.IsAny<Uri>(), rootFolder), Times.Once());
            itemOperations.Verify(x => x.Navigate(It.IsAny<string>()), Times.Once());
        }

        /// <summary>
        /// If you use the menu item and there's no longer a readme, then the browser should be opened.
        /// </summary>
        [TestMethod]
        public void OpenWelcomePageWithDeletedReadMe()
        {
            // Arrange
            var itemOperations = new Mock<IItemOperations>();
            var solutionFolder = new Mock<ISolutionFolder>();
            var policy = new Mock<IDefaultDocumentPolicy>();
            var rootFolder = @"Z:\some\path";
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
            server.Verify(x => x.Start(It.IsAny<Uri>(), rootFolder), Times.Once());
            // Browser should be opened twice -- once for the initial solution opening, and again for the menu item.
            itemOperations.Verify(x => x.Navigate(It.IsAny<string>()), Times.Exactly(2));
        }

        /// <summary>
        /// If you use the menu item and there's a new readme, then the server should be started and the browser should be opened.
        /// </summary>
        [TestMethod]
        public void OpenWelcomePageWithNewReadMe()
        {
            // Arrange
            var itemOperations = new Mock<IItemOperations>();
            var solutionFolder = new Mock<ISolutionFolder>();
            var policy = new Mock<IDefaultDocumentPolicy>();
            var rootFolder = @"Z:\some\path";
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
            server.Verify(x => x.Start(It.IsAny<Uri>(), rootFolder), Times.Once());
            itemOperations.Verify(x => x.Navigate(It.IsAny<string>()), Times.Once());
        }

        [TestMethod]
        public void CloseSolution()
        {
            // Arrange
            var itemOperations = new Mock<IItemOperations>();
            var solutionFolder = new Mock<ISolutionFolder>();
            var policy = new Mock<IDefaultDocumentPolicy>();
            var rootFolder = @"Z:\some\path";
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

        [TestMethod]
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