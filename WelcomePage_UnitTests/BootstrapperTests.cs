using System.IO;
using System.Reflection;
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
            var rootDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var bootstrapper = new Bootstrapper(rootDirectory);
            var browser = new Browser(bootstrapper);

            // Act
            var response = browser.Get("/");

            // Assert
            Assert.IsNotNull(response);
        }
    }
}