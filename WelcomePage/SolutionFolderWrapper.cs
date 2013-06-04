using System;
using System.IO;
using EnvDTE;

namespace RogerLipscombe.WelcomePage
{
    public class SolutionFolderWrapper : ISolutionFolder
    {
        private readonly DTE _dte;

        public SolutionFolderWrapper(DTE dte)
        {
            _dte = dte;
        }

        public string GetDirectoryName()
        {
            var solution = _dte.Solution;
            if (solution == null)
                return null;

            var fullName = solution.FullName;
            if (string.IsNullOrWhiteSpace(fullName))
                return null;

            return Path.GetDirectoryName(fullName);
        }
    }
}