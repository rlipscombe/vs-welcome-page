using Moq;
using NUnit.Framework;
using Nancy;
using Nancy.Testing;
using WelcomePage.Core;

namespace WelcomePage_UnitTests
{
    [TestFixture]
    public class HomeModuleTests
    {
        [Test]
        public void HomePage()
        {
            // Arrange
            var readme = new Mock<IDocumentFile>(MockBehavior.Strict);
            var documentFolder = new Mock<IDocumentFolder>(MockBehavior.Strict);
            documentFolder.Setup(x => x.Open("README")).Returns(readme.Object);
            readme.Setup(x => x.ReadAllText()).Returns("Hello\r\n==\r\nWorld");
            var browser =
                new Browser(
                    with => with.Module(new HomeModule(documentFolder.Object)));

            // Act
            var response = browser.Get("/");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var html = response.Body.AsHtmlDocument();
            var content = html.GetElementbyId("content");

            Assert.That(content.InnerHtml, Is.StringContaining("<h1>Hello</h1>"));
            Assert.That(content.InnerHtml, Is.StringContaining("<p>World</p>"));
        }

        [Test]
        public void OtherPage()
        {
            // Arrange
            var page = new Mock<IDocumentFile>(MockBehavior.Strict);
            var documentFolder = new Mock<IDocumentFolder>(MockBehavior.Strict);
            documentFolder.Setup(x => x.Open(It.IsAny<string>())).Returns(page.Object);
            page.Setup(x => x.ReadAllText()).Returns("Hello\r\n==\r\nWorld");
            var browser =
                new Browser(
                    with => with.Module(new HomeModule(documentFolder.Object)));

            // Act
            var response = browser.Get("/other-page");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var html = response.Body.AsHtmlDocument();
            var content = html.GetElementbyId("content");

            Assert.That(content.InnerHtml, Is.StringContaining("<h1>Hello</h1>"));
            Assert.That(content.InnerHtml, Is.StringContaining("<p>World</p>"));
        }
    }
}