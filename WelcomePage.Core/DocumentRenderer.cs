using System;
using System.Text.RegularExpressions;
using Kiwi.Markdown;
using Kiwi.Markdown.ContentProviders;

namespace WelcomePage.Core
{
    public class DocumentRenderer : IDocumentRenderer
    {
        private readonly IDocumentFolder _documentFolder;

        public DocumentRenderer(IDocumentFolder documentFolder)
        {
            if (documentFolder == null)
                throw new ArgumentNullException("documentFolder");

            _documentFolder = documentFolder;
        }

        public string RootDirectory
        {
            get { return _documentFolder.RootDirectory; }
        }

        public RenderedDocument GetDefaultDocument()
        {
            var name = FindDefaultDocumentId();
            return GetDocument(name);
        }

        public RenderedDocument GetDocument(string name)
        {
            // TODO: Look up the converter based on the document type?
            var converter = new MarkdownService(new FileContentProvider(_documentFolder.RootDirectory));
            var document = converter.GetDocument(name);

            // Kiwi.Markdown (or MarkdownSharp) doesn't appear to support [[Links]], so we'll do that here:
            var content = Regex.Replace(document.Content, @"\[\[(.*?)\]\]",
                                        m => string.Format("<a href=\"/{0}\">{0}</a>", m.Groups[1].Value));
            return new RenderedDocument { Title = document.Title, Content = content };
        }

        private string FindDefaultDocumentId()
        {
            return _documentFolder.FindDefaultDocumentId();
        }
    }
}