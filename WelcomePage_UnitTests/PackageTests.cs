using System.ComponentModel.Design;
using System.Reflection;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VsSDK.UnitTestLibrary;
using NUnit.Framework;
using RogerLipscombe.WelcomePage;

namespace WelcomePage_UnitTests
{
    [TestFixture]
    public class PackageTests
    {
        [Test]
        public void CreateInstance()
        {
            var package = new WelcomePagePackage();
        }

        [Test]
        public void SetSite()
        {
            // Arrange
            var package = (IVsPackage) new WelcomePagePackage();
            var serviceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices();

            // Act/Assert
            Assert.AreEqual(VSConstants.S_OK, package.SetSite(serviceProvider));
            Assert.AreEqual(VSConstants.S_OK, package.SetSite(null));
        }

        [Test]
        public void InitializeMenuCommand()
        {
            // Arrange
            var package = (IVsPackage)new WelcomePagePackage();
            var serviceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices();

            // Act
            Assert.AreEqual(VSConstants.S_OK, package.SetSite(serviceProvider));

            // Assert
            var mcs = GetService<WelcomePagePackage, IMenuCommandService>(package);
            var expectedCommandId = new CommandID(GuidList.guidWelcomePageCmdSet, (int) PkgCmdIDList.cmdidViewWelcomePage);
            var menuCommand = mcs.FindCommand(expectedCommandId);
            Assert.IsNotNull(menuCommand);
        }

        private static TService GetService<TPackage, TService>(IVsPackage package)
        {
            var methodInfo = typeof (TPackage).GetMethod("GetService", BindingFlags.Instance | BindingFlags.NonPublic);
            return (TService) methodInfo.Invoke(package, new object[] { typeof (TService) });
        }
    }
}
