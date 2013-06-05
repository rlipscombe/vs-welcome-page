using Kiwi.Markdown;
using Moq;
using NUnit.Framework;
using Nancy.Testing;
using WelcomePage.Core;

namespace WelcomePage_UnitTests
{
    [TestFixture]
    public class WebServerTests
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


    //[TestFixture]
    //public class ContentProviderTests
    //{
    //    [Test]
    //    public void MissingDefaultDocument()
    //    {
    //        // Arrange
    //        var inner = new Mock<IContentProvider>(MockBehavior.Strict);
    //        var policy = new Mock<IDefaultDocumentPolicy>(MockBehavior.Strict);
    //        policy.Setup(x => x.GetDefaultDocument(It.IsAny<string>())).Returns((string) null);
    //        var contentProvider = new ContentProvider(inner.Object, policy.Object, @"Z:\some\path");

    //        // Act/Assert
    //        Assert.Throws<DefaultDocumentNotFoundException>(() => contentProvider.GetContent(""));
    //    }
    //}
}