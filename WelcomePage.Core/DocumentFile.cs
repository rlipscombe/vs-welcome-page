using System.IO;

namespace WelcomePage.Core
{
    public class DocumentFile : IDocumentFile
    {
        private readonly string _path;

        public DocumentFile(string path)
        {
            _path = path;
        }

        public string ReadAllText()
        {
            return File.ReadAllText(_path);
        }
    }
}