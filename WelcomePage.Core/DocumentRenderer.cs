using System.IO;

namespace WelcomePage.Core
{
    public class DocumentFolder : IDocumentFolder
    {
        private readonly string _rootDirectory;

        public DocumentFolder(string rootDirectory)
        {
            _rootDirectory = rootDirectory;
        }

        public string ReadAllText(string name)
        {
            return File.ReadAllText(Path.Combine(_rootDirectory, name + ".md"));
        }
    }
}