using Kiwi.Markdown;
using Moq;
using NUnit.Framework;
using Nancy.Testing;
using WelcomePage.Core;

namespace WelcomePage_UnitTests
{
    [TestFixture]
    public class BootstrapperTests
    {
        [Test]
        public void HomePage()
        {
            // Arrange
            var contentProvider = new Mock<IContentProvider>(MockBehavior.Strict);
            contentProvider.Setup(x => x.GetContent(""))
                           .Returns("Title\n=====\n\nContent");
            var bootstrapper = new Bootstrapper(contentProvider.Object);
            var browser = new Browser(bootstrapper);

            // Act
            var response = browser.Get("/");

            // Assert
            Assert.IsNotNull(response);
        }
    }
}