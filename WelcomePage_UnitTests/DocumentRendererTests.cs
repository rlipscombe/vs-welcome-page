using Kiwi.Markdown;
using Moq;
using NUnit.Framework;
using WelcomePage.Core;

namespace WelcomePage_UnitTests
{
    [TestFixture]
    public class DocumentRendererTests
    {
        [Test]
        public void GetDefaultDocument()
        {
            // Arrange
            var contentProvider = new Mock<IContentProvider>(MockBehavior.Strict);
            contentProvider.Setup(x => x.GetContent(""))
                           .Returns("Title\n=====\n\nContent");
            var converter = new MarkdownService(contentProvider.Object);
            var renderer = new DocumentRenderer(converter);

            // Act
            var doc = renderer.GetDefaultDocument();

            // Assert
            Assert.AreEqual("", doc.Title);
            Assert.AreEqual("<h1>Title</h1>\r\n\r\n<p>Content</p>\r\n", doc.Content);
        }
    }
}