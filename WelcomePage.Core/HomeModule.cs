using Nancy;
using Nancy.Responses.Negotiation;

namespace WelcomePage.Core
{
    public class HomeModule : NancyModule
    {
        private readonly IDocumentRenderer _renderer;

        public HomeModule(IDocumentRenderer renderer)
        {
            _renderer = renderer;

            Get["/"] = x =>
                {
                    var document = _renderer.GetDefaultDocument();
                    return ViewDocument(document);
                };

            Get["/{path*}"] = x =>
                {
                    string path = x.Path;
                    var document = _renderer.GetDocument(path);
                    return ViewDocument(document);
                };
        }

        private Negotiator ViewDocument(RenderedDocument document)
        {
            var model = new
                {
                    document.Title,
                    document.Content
                };
            return View["Index", model];
        }
    }
}