using System.IO;

namespace WelcomePage.Core
{
    public class DefaultDocumentPolicy : IDefaultDocumentPolicy
    {
        private readonly string[] _options = new[]
            {
                "Index", // because
                "Home", // github wiki
                "README" // github project
            };

        public bool ContainsDefaultDocument(string rootDirectory)
        {
            return FindDefaultDocument(rootDirectory) != null;
        }

        public string GetDefaultDocument(string rootDirectory)
        {
            string result = FindDefaultDocument(rootDirectory);
            if (string.IsNullOrWhiteSpace(result))
                throw new DefaultDocumentNotFoundException(rootDirectory, _options);

            return result;
        }

        private string FindDefaultDocument(string rootDirectory)
        {
            foreach (var option in _options)
            {
                var path = Path.Combine(rootDirectory, string.Format("{0}.md", option));
                if (File.Exists(path))
                    return option;
            }

            return null;
        }
    }
}