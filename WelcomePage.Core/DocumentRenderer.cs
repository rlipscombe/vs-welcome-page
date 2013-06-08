using System.Text.RegularExpressions;
using Kiwi.Markdown;

namespace WelcomePage.Core
{
    public class DocumentRenderer : IDocumentRenderer
    {
        private readonly IMarkdownService _converter;
        
        public DocumentRenderer(IMarkdownService converter)
        {
            _converter = converter;
        }

        public RenderedDocument GetDefaultDocument()
        {
            return GetDocument(string.Empty);
        }

        public RenderedDocument GetDocument(string name)
        {
            var document = _converter.GetDocument(name);

            // BUG: Default document doesn't get a name displayed.

            // Kiwi.Markdown (or MarkdownSharp) doesn't appear to support [[Links]], so we'll do that here:
            var content = Regex.Replace(document.Content, @"\[\[(.*?)\]\]",
                                        m => string.Format("<a href=\"/{0}\">{0}</a>", m.Groups[1].Value));
            return new RenderedDocument { Title = document.Title, Content = content };
        }
    }
}