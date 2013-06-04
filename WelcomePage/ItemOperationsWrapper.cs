using EnvDTE;

namespace RogerLipscombe.WelcomePage
{
    public class ItemOperationsWrapper : IItemOperations
    {
        private readonly DTE _dte;

        public ItemOperationsWrapper(DTE dte)
        {
            _dte = dte;
        }

        public void Navigate(string url)
        {
            _dte.ItemOperations.Navigate(url);
        }
    }
}