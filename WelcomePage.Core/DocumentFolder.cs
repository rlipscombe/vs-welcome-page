using System;
using System.IO;
using Kiwi.Markdown;
using Kiwi.Markdown.ContentProviders;

namespace WelcomePage.Core
{
    public class DocumentFolder : IDocumentFolder
    {
        public DocumentFolder(string rootDirectory)
        {
            if (rootDirectory == null)
                throw new ArgumentNullException("rootDirectory");

            RootDirectory = rootDirectory;
            ContentProvider = new FileContentProvider(rootDirectory);
        }

        public IContentProvider ContentProvider { get; private set; }

        public string RootDirectory { get; private set; }
    
        public string FindDefaultDocumentId()
        {
            var options = new[]
                {
                    "Index", // because
                    "Home", // github wiki
                    "README" // github project
                };

            foreach (var option in options)
            {
                var path = Path.Combine(RootDirectory, string.Format("{0}.md", option));
                if (File.Exists(path))
                    return option;
            }

            throw new DefaultDocumentNotFoundException(RootDirectory, options);
        }

        public static IDocumentFolder Create(string rootDirectory)
        {
            return new DocumentFolder(rootDirectory);
        }
    }
}