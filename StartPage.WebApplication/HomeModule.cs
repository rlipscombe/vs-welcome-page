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
                    var contentProvider = new FileContentProvider(@"C:\Users\roger\Source\vs-start-page");
                    var converter = new MarkdownService(contentProvider);
                    var document = converter.GetDocument("README");
                    return document.Content;
                };
        }
    }
}