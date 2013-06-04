namespace WelcomePage.Core
{
    public interface IDocumentFolder
    {
        string RootDirectory { get; }
        string FindDefaultDocumentId();
    }
}