using System.Configuration;
using Kiwi.Markdown;
using Kiwi.Markdown.ContentProviders;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.Embedded.Conventions;
using Nancy.TinyIoc;
using Nancy.ViewEngines;

namespace WelcomePage.Core
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        private readonly IContentProvider _contentProvider;

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
            base.ApplicationStartup(container, pipelines);

            ResourceViewLocationProvider.RootNamespaces.Add(GetType().Assembly, "WelcomePage.Core.Views");
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);

            container.Register<IContentProvider>((c, p) => _contentProvider);
            container.Register<IMarkdownService, MarkdownService>();
            container.Register<IDocumentRenderer, DocumentRenderer>();
        }

        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            base.ConfigureConventions(nancyConventions);

            nancyConventions.StaticContentsConventions.Add(
                EmbeddedStaticContentConventionBuilder.AddDirectory("/Content/", GetType().Assembly, "WelcomePage.Core.Content"));
        }

        /// <summary>
        /// Disable default favicon by returning null.
        /// </summary>
        protected override byte[] FavIcon
        {
            get { return null; }
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
