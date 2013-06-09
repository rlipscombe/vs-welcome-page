using System.Diagnostics;
using Kiwi.Markdown;
using Nancy;
using Nancy.TinyIoc;

namespace WelcomePage.Core
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        private static readonly TraceSource Trace = new TraceSource("WelcomePage.Core");

        private readonly IContentProvider _contentProvider;

        public Bootstrapper(IContentProvider contentProvider)
        {
            _contentProvider = contentProvider;
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            Trace.TraceInformation("Bootstrapper.ConfigureApplicationContainer");
            base.ConfigureApplicationContainer(container);

            container.Register<IContentProvider>((c, p) => _contentProvider);
            container.Register<IMarkdownService, MarkdownService>();
            container.Register<IDocumentRenderer, DocumentRenderer>();
        }
        
        protected override IRootPathProvider RootPathProvider
        {
            get { return new DefaultRootPathProvider(); }
        }
    }
}
