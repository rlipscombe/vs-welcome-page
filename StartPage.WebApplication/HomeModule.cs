using System.Configuration;
using Kiwi.Markdown;
using Kiwi.Markdown.ContentProviders;
using Nancy;

namespace StartPage.WebApplication
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            Get["/"] = context =>
                {
                    var rootDirectory = GetRootDirectory();
                    var contentProvider = new FileContentProvider(rootDirectory);
                    var converter = new MarkdownService(contentProvider);
                    var document = converter.GetDocument("README");
                    return View["Index", document];
                };
         
            Get["/about"] = context =>
                {
                    var rootDirectory = GetRootDirectory();
                    return View["About", rootDirectory];
                };
        }

        private static string GetRootDirectory()
        {
            var rootDirectory = ConfigurationManager.AppSettings["RootDirectory"];
            if (string.IsNullOrWhiteSpace(rootDirectory))
                throw new ConfigurationErrorsException("RootDirectory is not configured.");
            return rootDirectory;
        }
    }
}