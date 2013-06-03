using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WelcomePage.Core
{
    internal class DefaultDocumentNotFoundException : FileNotFoundException
    {
        public DefaultDocumentNotFoundException(string rootDirectory, IEnumerable<string> options)
            : base(CreateMessage(rootDirectory, options))
        {
        }

        private static string CreateMessage(string rootDirectory, IEnumerable<string> options)
        {
            return
                string.Format("Cannot find default document in root directory '{0}'. Considered {1}.",
                              rootDirectory, string.Join(", ", options.Select(o => "'" + o + "'")));
        }
    }
}