using System;

namespace WelcomePage.Core
{
    public interface IDefaultDocumentPolicy
    {
        bool ContainsDefaultDocument(string rootDirectory);
    }

    public class DefaultDocumentPolicy : IDefaultDocumentPolicy
    {
        public bool ContainsDefaultDocument(string rootDirectory)
        {
            throw new NotImplementedException();
        }
    }
}
