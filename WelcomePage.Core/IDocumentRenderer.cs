namespace WelcomePage.WebApplication
{
    public interface IDocumentRenderer
    {
        string RootDirectory { get; }
        RenderedDocument GetDefaultDocument();
        RenderedDocument GetDocument(string name);
    }
}