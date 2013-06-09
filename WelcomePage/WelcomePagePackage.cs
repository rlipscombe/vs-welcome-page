using System;
using System.IO;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;

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
        private WelcomePageImpl _impl;

        protected override void Initialize()
        {
            base.Initialize();

            AddMenuCommands();

            _solution = GetService(typeof (SVsSolution)) as IVsSolution;
            if (_solution != null)
                _solution.AdviseSolutionEvents(this, out _dwCookie);

            var dte = (DTE)GetService(typeof(DTE));

            var itemOperations = new ItemOperationsWrapper(dte);
            var solutionFolderWrapper = new SolutionFolderWrapper(dte);
            var defaultDocumentPolicy = new DefaultDocumentPolicy();
            var server = new WebServer();
            _impl = new WelcomePageImpl(solutionFolderWrapper, defaultDocumentPolicy, itemOperations, server);
        }

        protected override void Dispose(bool disposing)
        {
            _impl.Dispose();

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
            var menuItem = new MenuCommand((sender, e) => OnViewWelcomePage(), menuCommandId);
            mcs.AddCommand(menuItem);
        }

        private void OnViewWelcomePage()
        {
            try
            {
                _impl.OnViewWelcomePage();
            }
            catch (Exception ex)
            {
                ShowMessageBox("WelcomePage", ex.Message);
            }
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
            try
            {
                _impl.OnAfterOpenSolution();
            }
            catch (Exception ex)
            {
                ShowMessageBox("WelcomePage", ex.Message);
            }
            return VSConstants.S_OK;
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
            _impl.OnAfterCloseSolution();

            // TODO: If the web browser is open, and pointing at the readme, close the window.
            return VSConstants.S_OK;
        }
    }

    public class DefaultDocumentPolicy : IDefaultDocumentPolicy
    {
        public bool ContainsDefaultDocument(string solutionDirectoryName)
        {
            return File.Exists(Path.Combine(solutionDirectoryName, "README.md"));
        }
    }
}
