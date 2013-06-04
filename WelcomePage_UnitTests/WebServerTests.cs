using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nancy.Testing;
using WelcomePage.Core;

namespace WelcomePage_UnitTests
{
    [TestClass]
    public class WebServerTests
    {
        [TestMethod]
        public void HomePage()
        {
            // Arrange
            var bootstrapper = new Bootstrapper();
            var browser = new Browser(bootstrapper);

            // Act
            var response = browser.Get("/");

            // Assert
            Assert.IsNotNull(response);
        }
    }
}