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

        public bool Exists(string name)
        {
            throw new System.NotImplementedException();
        }

        public IDocumentFile Open(string name)
        {
            var path = Path.Combine(_rootDirectory, name + ".md");
            return new DocumentFile(path);
        }
    }
}