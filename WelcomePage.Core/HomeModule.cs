using System.Diagnostics;
using System.Reflection;
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

            Get["/_About"] = x =>
                {
                    var processId = Process.GetCurrentProcess().Id;
                    var location = Assembly.GetExecutingAssembly().Location;
                    var informationalVersion =
                        Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>();
                    var version =
                        Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>();
                    var model =
                        new
                            {
                                ProcessId = processId,
                                Location = location,
                                Version =
                                    informationalVersion != null
                                        ? informationalVersion.InformationalVersion
                                        : version.Version
                            };
                    return View["About", model];
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