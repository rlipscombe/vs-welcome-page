using Kiwi.Markdown;

namespace WelcomePage.Core
{
    public interface IDocumentFolder
    {
        IContentProvider ContentProvider { get; }
        string RootDirectory { get; }
        string FindDefaultDocumentId();
    }
}