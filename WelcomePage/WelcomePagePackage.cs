using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
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
using Microsoft.VisualStudio.Shell;
using Nancy.Hosting.Self;
using WelcomePage.WebApplication;

namespace RogerLipscombe.WelcomePage
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    // We need to load early enough to be able to hook solution events.
    // VSConstants.UICONTEXT_NoSolution
    [ProvideAutoLoad("ADFC4E64-0397-11D1-9F4E-00A0C911004F")]
    [Guid(GuidList.guidWelcomePagePkgString)]
    public sealed class WelcomePagePackage : Package, IVsSolutionEvents
    {
        // Event subscription.
        private IVsSolution _solution;
        private uint _dwCookie;

        // Web server.
        private Uri _url;
        private NancyHost _host;

        private static void Log(string message)
        {
            Debug.WriteLine(message);
        }

        private static void Log(string format, object arg0)
        {
            var message = string.Format(format, arg0);
            Log(message);
        }

        private static void Log(string format, object arg0, object arg1)
        {
            var message = string.Format(format, arg0, arg1);
            Log(message);
        }

        private static void Log(string format, params object[] args)
        {
            var message = string.Format(format, args);
            Log(message);
        }

        protected override void Initialize()
        {
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
            var menuCommandId = new CommandID(GuidList.guidWelcomePageCmdSet, (int) PkgCmdIDList.cmdidViewWelcomePage);
            var menuItem = new MenuCommand((sender, e) => ViewWelcomePage(), menuCommandId);
            mcs.AddCommand(menuItem);
        }

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

            if (_host == null)
            {
                // Figure out where the .SLN file lives.
                var solutionFileName = GetSolutionFileName(dte);
                if (solutionFileName == null)
                    return;

                var solutionFolder = Path.GetDirectoryName(solutionFileName);

                // Figure out a port number.
                var random = new Random();
                var port = random.Next(10000, short.MaxValue);
                _url = new Uri(string.Format("http://localhost:{0}/", port));
                Log("Using URL '{0}'", _url);

                // Fire up the web server.
                var bootstrapper = new Bootstrapper(solutionFolder);
                var configuration = new HostConfiguration { RewriteLocalhost = false };
                _host = new NancyHost(bootstrapper, configuration, _url);
                _host.Start();
            }

            // Open the web browser.
            // TODO: How do we (should we?) bring it to the front?
            // TODO: If we don't bring it to the front, how do we bring it to the user's attention?
            Log("Navigating to '{0}'", _url);
            dte.ItemOperations.Navigate(_url.ToString());
        }

        private static string GetSolutionFileName(DTE dte)
        {
            if (dte.Solution == null)
                return null;

            var solutionFileName = dte.Solution.FullName;
            if (string.IsNullOrWhiteSpace(solutionFileName))
                return null;

            Log("solutionFileName = '{0}'.", solutionFileName);
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
            if (_host != null)
            {
                _host.Stop();
                _host = null;
            }

            // TODO: If the web browser is open, and pointing at the readme, close the window.
            return VSConstants.S_OK;
        }
    }
}
