using HtmlAgilityPack;
using Nancy.Testing;

namespace WelcomePage_UnitTests
{
    internal static class ResponseAsHtml
    {
        public static HtmlDocument AsHtmlDocument(this BrowserResponseBodyWrapper body)
        {
            var stream = body.AsStream();
            var doc = new HtmlDocument();
            doc.Load(stream);
            return doc;
        }
    }
}