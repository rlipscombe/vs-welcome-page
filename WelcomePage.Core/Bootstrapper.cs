using System.Configuration;
using System.Diagnostics;
using System.IO;
using Kiwi.Markdown;
using Kiwi.Markdown.ContentProviders;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.Diagnostics;
using Nancy.Embedded.Conventions;
using Nancy.TinyIoc;
using Nancy.ViewEngines;

namespace WelcomePage.Core
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        private static readonly TraceSource Trace = new TraceSource("WelcomePage.Core");

        private readonly IContentProvider _contentProvider;
        private byte[] _favIcon;

        public Bootstrapper()
            : this(CreateContentProvider(ConfigurationManager.AppSettings["RootFolder"]))
        {
        }

        private static IContentProvider CreateContentProvider(string rootDirectory)
        {
            return new ContentProvider(new FileContentProvider(rootDirectory), new DefaultDocumentPolicy(),
                                       rootDirectory);
        }

        public Bootstrapper(IContentProvider contentProvider)
        {
            _contentProvider = contentProvider;
        }

        /// <summary>
        /// Called when the application starts up. We use it to register the embedded-resource views.
        /// TODO: Consider *not* embedding the views/content, so that they're more easily configurable.
        /// </summary>
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            Trace.TraceInformation("Bootstrapper.ApplicationStartup");
            base.ApplicationStartup(container, pipelines);

            ResourceViewLocationProvider.RootNamespaces.Add(GetType().Assembly, "WelcomePage.Core.Views");
//            DiagnosticsHook.Disable(pipelines);
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            Trace.TraceInformation("Bootstrapper.ConfigureApplicationContainer");
            base.ConfigureApplicationContainer(container);

            container.Register<IContentProvider>((c, p) => _contentProvider);
            container.Register<IMarkdownService, MarkdownService>();
            container.Register<IDocumentRenderer, DocumentRenderer>();
        }

        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            Trace.TraceInformation("Bootstrapper.ConfigureConventions");
            base.ConfigureConventions(nancyConventions);

            nancyConventions.StaticContentsConventions.Add(
                EmbeddedStaticContentConventionBuilder.AddDirectory("Content", GetType().Assembly));
        }

        protected override DiagnosticsConfiguration DiagnosticsConfiguration
        {
            get { return new DiagnosticsConfiguration { Password = "password" }; }
        }

        protected override byte[] FavIcon
        {
            get { return _favIcon ?? (_favIcon = LoadFavIcon()); }
        }

        /// <summary>
        /// Load favicon.ico from embedded resources.
        /// </summary>
        private byte[] LoadFavIcon()
        {
            var assembly = typeof (Bootstrapper).Assembly;
            using (var stream = assembly.GetManifestResourceStream("WelcomePage.Core.Content.favicon.ico"))
            {
                var memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// Configure Nancy to search for views in embedded resources.
        /// </summary>
        protected override NancyInternalConfiguration InternalConfiguration
        {
            get
            {
                return
                    NancyInternalConfiguration.WithOverrides(
                        x => x.ViewLocationProvider = typeof (ResourceViewLocationProvider));
            }
        }

        protected override IRootPathProvider RootPathProvider
        {
            get { return new DefaultRootPathProvider(); }
        }
    }
}
