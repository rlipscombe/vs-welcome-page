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

            Get["/"] = context =>
                {
                    var document = _renderer.GetDefaultDocument();
                    return ViewDocument(document);
                };

            Get["/{id*}"] = context =>
                {
                    string id = context.Id;
                    var document = _renderer.GetDocument(id);
                    return ViewDocument(document);
                };

            Get["/_About"] = context =>
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
                                _renderer.RootDirectory,
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