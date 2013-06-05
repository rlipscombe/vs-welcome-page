namespace WelcomePage.Core
{
    public interface IDefaultDocumentPolicy
    {
        bool ContainsDefaultDocument(string rootDirectory);
        string GetDefaultDocument(string rootDirectory);
    }
}