using System;
using System.IO;
using System.Text.RegularExpressions;
using Kiwi.Markdown;
using Kiwi.Markdown.ContentProviders;

namespace WelcomePage.WebApplication
{
    public class DocumentRenderer : IDocumentRenderer
    {
        private readonly IMarkdownService _converter;
        private readonly string _rootDirectory;

        public DocumentRenderer(string rootDirectory)
        {
            if (rootDirectory == null) 
                throw new ArgumentNullException("rootDirectory");

            var contentProvider = new FileContentProvider(rootDirectory);
            _converter = new MarkdownService(contentProvider);
            _rootDirectory = rootDirectory;
        }

        public string RootDirectory
        {
            get { return _rootDirectory; }
        }

        public RenderedDocument GetDefaultDocument()
        {
            var name = FindDefaultDocumentId(_rootDirectory);
            return GetDocument(name);
        }

        public RenderedDocument GetDocument(string name)
        {
            var document = _converter.GetDocument(name);

            // Kiwi.Markdown (or MarkdownSharp) doesn't appear to support [[Links]], so we'll do that here:
            var content = Regex.Replace(document.Content, @"\[\[(.*?)\]\]",
                                        m => string.Format("<a href=\"/{0}\">{0}</a>", m.Groups[1].Value));
            return new RenderedDocument { Title = document.Title, Content = content };
        }

        private string FindDefaultDocumentId(string rootDirectory)
        {
            if (rootDirectory == null)
                throw new ArgumentNullException("rootDirectory");

            var options = new[]
                {
                    "Index",    // because
                    "Home",     // github wiki
                    "README"    // github project
                };

            foreach (var option in options)
            {
                var path = Path.Combine(rootDirectory, string.Format("{0}.md", option));
                if (File.Exists(path))
                    return option;
            }

            throw new DefaultDocumentNotFoundException(rootDirectory, options);
        }
    }
}