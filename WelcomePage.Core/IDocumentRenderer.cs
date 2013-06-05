namespace WelcomePage.Core
{
    public interface IDocumentRenderer
    {
        RenderedDocument GetDefaultDocument();
        RenderedDocument GetDocument(string name);
    }
}