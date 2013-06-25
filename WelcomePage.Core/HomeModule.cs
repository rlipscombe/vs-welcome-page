using System.Diagnostics;
using System.Reflection;
using MarkdownDeep;
using Nancy;

namespace WelcomePage.Core
{
    public class HomeModule : NancyModule
    {
        public HomeModule(IDocumentFolder documentFolder)
        {
            Get["/"] = x =>
                {
                    var converter = new Markdown();
                    var markdown = documentFolder.ReadAllText("README");
                    var html = converter.Transform(markdown);
                    return View["Index", new { Title = "README", Content = html }];
                };

            Get["/{path*}"] = x =>
                {
                    string path = x.Path;
                    var converter = new Markdown();
                    var markdown = documentFolder.ReadAllText(path);
                    var html = converter.Transform(markdown);
                    return View["Index", new { Title = path, Content = html }];
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
    }
}