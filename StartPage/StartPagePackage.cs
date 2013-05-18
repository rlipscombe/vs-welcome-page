using System;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using EnvDTE;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Process = System.Diagnostics.Process;

namespace RogerLipscombe.StartPage
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // This attribute is used to register the information needed to show this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("Menus.ctmenu", 1)]
    // We need to load early enough to be able to hook solution events.
    // VSConstants.UICONTEXT_NoSolution
    [ProvideAutoLoad("ADFC4E64-0397-11D1-9F4E-00A0C911004F")]
    [Guid(GuidList.guidStartPagePkgString)]
    public sealed class StartPagePackage : Package, IVsSolutionEvents
    {
        private IVsSolution _solution;
        private uint _dwCookie;
        private Process _webServerProcess;
        private int _port;

        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public StartPagePackage()
        {
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
        }



        /////////////////////////////////////////////////////////////////////////////
        // Overridden Package Implementation
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Debug.WriteLine (string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            AddMenuCommands();

            _solution = GetService(typeof (SVsSolution)) as IVsSolution;
            if (_solution != null)
                _solution.AdviseSolutionEvents(this, out _dwCookie);
        }

        protected override void Dispose(bool disposing)
        {
            if (_solution != null && _dwCookie != 0)
                _solution.UnadviseSolutionEvents(_dwCookie);
            _dwCookie = 0;
        
            base.Dispose(disposing);
        }

        private void AddMenuCommands()
        {
            // Add our command handlers for menu (commands must exist in the .vsct file)
            var mcs = GetService(typeof (IMenuCommandService)) as OleMenuCommandService;
            if (mcs == null)
                return;

            // Create the command for the menu item.
            var menuCommandId = new CommandID(GuidList.guidStartPageCmdSet, (int) PkgCmdIDList.cmdidViewWelcomePage);
            var menuItem = new MenuCommand((sender, e) => ViewWelcomePage(), menuCommandId);
            mcs.AddCommand(menuItem);
        }

        #endregion

        private void ShowMessageBox(string pszTitle, string pszText)
        {
            IVsUIShell uiShell = (IVsUIShell) GetService(typeof (SVsUIShell));
            Guid clsid = Guid.Empty;
            int result;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(uiShell.ShowMessageBox(
                0,
                ref clsid,
                pszTitle,
                pszText,
                string.Empty,
                0,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                OLEMSGICON.OLEMSGICON_INFO,
                0, // false
                out result));
        }

        public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            ViewWelcomePage();
            return VSConstants.S_OK;
        }

        private void ViewWelcomePage()
        {
            try
            {
                ViewWelcomePageThrows();
            }
            catch (Exception ex)
            {
                ShowMessageBox("Whoops", ex.Message);
            }
        }

        private void ViewWelcomePageThrows()
        {
            var dte = (DTE)GetService(typeof(DTE));
            if (_webServerProcess == null)
            {
                // Figure out where the .SLN file lives.
                var solutionFileName = GetSolutionFileName(dte);
                if (solutionFileName == null)
                    return;

                var solutionFolder = Path.GetDirectoryName(solutionFileName);

                // Figure out where the web application binaries are.
                // TODO: Do this properly.
                var webApplicationSourceRoot = @"C:\Users\roger\Source\vs-start-page\StartPage.WebApplication";

                // Copy the web application binaries to a new temporary location.
                var instanceDirectory = GetWebAppInstanceDirectory(solutionFileName);
                CopyWebAppFiles(webApplicationSourceRoot, instanceDirectory);

                // TODO: Can we avoid doing this by (somehow) configuring application root in Web.config?

                // Write the solution path to Web.config.
                ConfigureWebAppInstance(instanceDirectory, solutionFolder);

                // Figure out a port number.
                var random = new Random();
                _port = random.Next(10000, short.MaxValue);
                Debug.WriteLine("Using port number '{0}'", _port);

                // Fire up IIS Express.
                var fileName = GetIisExpressFileName();
                var startInfo = new ProcessStartInfo
                    {
                        FileName = fileName,
                        Arguments = string.Format("/path:\"{0}\" /port:{1}", instanceDirectory, _port),
                        WindowStyle = ProcessWindowStyle.Hidden
                    };

                // TODO: How to detect a port number clash?
                _webServerProcess = Process.Start(startInfo);
                Debug.WriteLine("IIS Express started ({0} {1}). Process ID = {2}. Main Window = {3}.",
                                startInfo.FileName, startInfo.Arguments,
                                _webServerProcess.Id, _webServerProcess.MainWindowHandle);
            }

            // Open the web browser.
            // TODO: How do we (should we?) bring it to the front?
            // TODO: If we don't bring it to the front, how do we bring it to the user's attention?
            string url = string.Format("http://localhost:{0}/", _port);
            Debug.WriteLine("Navigating to '{0}'", url);
            dte.ItemOperations.Navigate(url);
        }

        private static void ConfigureWebAppInstance(string instanceDirectory, string solutionFolder)
        {
            try
            {
                const string xpath = "//configuration/appSettings/add[@key='RootDirectory']/@value";

                var configurationFileName = Path.Combine(instanceDirectory, "Web.config");
                Debug.WriteLine(
                    "Updating configuration file '{0}'. Setting RootDirectory to '{1}'.",
                    configurationFileName, solutionFolder);

                var configuration = new XmlDocument();

                configuration.Load(configurationFileName);
                var nsmgr = new XmlNamespaceManager(configuration.NameTable);
                var rootDirectoryNode = configuration.SelectSingleNode(xpath, nsmgr);
                if (rootDirectoryNode == null)
                {
                    throw new ConfigurationErrorsException(
                        string.Format("Cannot find node '{0}' in configuration file '{1}'.", xpath,
                                      configurationFileName));
                }

                rootDirectoryNode.Value = solutionFolder;
                configuration.Save(configurationFileName);
            }
            catch (Exception ex)
            {
                throw new WebAppInstanceConfigurationException(instanceDirectory, ex);
            }
        }

        private static string GetIisExpressFileName()
        {
            string installPath;
            using (var registryKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\IISExpress\8.0"))
            {
                if (registryKey == null)
                    throw new Exception("Cannot find IIS Express installation path.");

                installPath = (string) registryKey.GetValue("InstallPath");
                Debug.WriteLine("IIS Express InstallPath = '{0}'", installPath);
            }

            var fileName = Path.Combine(installPath, "iisexpress.exe");
            return fileName;
        }

        private static void CopyWebAppFiles(string webApplicationSourceRoot, string temporaryPath)
        {
            var webApplicationFiles = Directory.EnumerateFiles(webApplicationSourceRoot, "*", SearchOption.AllDirectories);
            foreach (var webApplicationFile in webApplicationFiles)
            {
                var sourceFileName = webApplicationFile;
                var relativeFileName = webApplicationFile.Substring(webApplicationSourceRoot.Length + 1);
                var destinationFileName = Path.Combine(temporaryPath, relativeFileName);
                var destinationDirectory = Path.GetDirectoryName(destinationFileName);
                if (!Directory.Exists(destinationDirectory))
                    Directory.CreateDirectory(destinationDirectory);

                Debug.WriteLine("Copying '{0}' to '{1}'...", sourceFileName, destinationFileName);
                // TODO: Symlink, rather than copy?
                File.Copy(sourceFileName, destinationFileName, overwrite: true);
            }
        }

        /// <summary>
        /// Figure out where to put the web application instance for this solution.
        /// </summary>
        private static string GetWebAppInstanceDirectory(string solutionFileName)
        {
            // Come up with a directory of the form "%TEMP%\StartPage_hash" where "hash" is
            // generated from the solution file name.
            // This allows us to:
            // 1. keep the files outside the solution folder.
            // 2. have a separate instance for each solution -- because they need to know where the files live.
            // 3. keep the files around between runs, so that we start up more quickly the next time.

            var tempPath = Path.GetTempPath();
            var instanceKey = GetInstanceKey(solutionFileName);

            var instanceDirectory = Path.Combine(tempPath, instanceKey);
            Debug.WriteLine("Using instance directory = '{0}'", instanceDirectory);
            return instanceDirectory;
        }

        private static string GetInstanceKey(string solutionFileName)
        {
            // MD5 is fine; we're not doing crypto, and we want something fairly short.
            string suffix;
            using (var alg = MD5.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(solutionFileName);
                var hash = alg.ComputeHash(bytes);

                // We need to use Base36, because Base64 uses upper and lower case letters,
                // and Windows filenames are case-insensitive.
                suffix = ToBase36String(hash);
            }

            var instanceKey = string.Format("StartPage_{0}_{1}",
                                            Path.GetFileNameWithoutExtension(solutionFileName), suffix);
            return instanceKey;
        }

        private static string ToBase36String(byte[] bytes)
        {
            const string alphabet = "0123456789abcdefghijklmnopqrstuvwxyz";

            // log2(36) is slightly more than 6: so for every 6 bits in the source, we need 8.
            int capacity = 8*bytes.Length/6;
            var result = new StringBuilder(capacity);

            var dividend = new BigInteger(bytes);
            while (!dividend.IsZero)
            {
                BigInteger remainder;
                dividend = BigInteger.DivRem(dividend, 36, out remainder);
                int index = Math.Abs((int) remainder);
                result.Append(alphabet[index]);
            }
         
            return result.ToString();
        }

        private static string GetSolutionFileName(DTE dte)
        {
            if (dte.Solution == null)
                return null;

            var solutionFileName = dte.Solution.FullName;
            if (string.IsNullOrWhiteSpace(solutionFileName))
                return null;

            Debug.WriteLine("solutionFileName = '{0}'.", solutionFileName);
            return solutionFileName;
        }

        public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeCloseSolution(object pUnkReserved)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterCloseSolution(object pUnkReserved)
        {
            if (_webServerProcess != null)
            {
                Debug.WriteLine("IIS Express: Process ID = {0}. Main Window = {1}.",
                                _webServerProcess.Id, _webServerProcess.MainWindowHandle);

                // BUG: This doesn't actually work. MainWindowHandle is zero.
                _webServerProcess.CloseMainWindow();
                _webServerProcess.Kill();

                _webServerProcess.Dispose();
            }

            // TODO: If the web browser is open, and pointing at the readme, close the window.

            return VSConstants.S_OK;
        }
    }

    internal class WebAppInstanceConfigurationException : Exception
    {
        public WebAppInstanceConfigurationException(string instanceDirectory, Exception innerException)
            : base(string.Format("Failed to configure web app instance at '{0}'.", instanceDirectory), innerException)
        {
        }
    }
}
