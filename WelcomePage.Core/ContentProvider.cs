using Kiwi.Markdown;

namespace WelcomePage.Core
{
    public class ContentProvider : IContentProvider
    {
        private readonly IContentProvider _provider;
        private readonly IDefaultDocumentPolicy _defaultDocumentPolicy;
        private readonly string _rootDirectory;

        public ContentProvider(IContentProvider provider, IDefaultDocumentPolicy defaultDocumentPolicy,
                               string rootDirectory)
        {
            _provider = provider;
            _defaultDocumentPolicy = defaultDocumentPolicy;
            _rootDirectory = rootDirectory;
        }

        public string GetContent(string docId)
        {
            if (string.IsNullOrWhiteSpace(docId))
                docId = _defaultDocumentPolicy.GetDefaultDocument(_rootDirectory);

            return _provider.GetContent(docId);
        }
    }
}