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
            var rootDirectory = ConfigurationManager.AppSettings["RootDirectory"];
            if (string.IsNullOrWhiteSpace(rootDirectory))
                throw new ConfigurationErrorsException("RootDirectory is not configured.");

            Get["/"] = context =>
                {
                    var contentProvider = new FileContentProvider(rootDirectory);
                    var converter = new MarkdownService(contentProvider);
                    var document = converter.GetDocument("README");
                    return document.Content;
                };
            Get["/about"] = context =>
                {
                    return rootDirectory;
                };
        }
    }
}