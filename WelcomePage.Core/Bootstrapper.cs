using System;
using System.Configuration;
using Autofac;
using Kiwi.Markdown;
using Kiwi.Markdown.ContentProviders;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Bootstrappers.Autofac;
using Nancy.Conventions;
using Nancy.Embedded.Conventions;
using Nancy.ViewEngines;

namespace WelcomePage.Core
{
    public class Bootstrapper : AutofacNancyBootstrapper
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
        protected override void ApplicationStartup(ILifetimeScope container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            ResourceViewLocationProvider.RootNamespaces.Add(GetType().Assembly, "WelcomePage.Core.Views");
        }

        protected override ILifetimeScope GetApplicationContainer()
        {
            var builder = new ContainerBuilder();

            builder.Register<IContentProvider>(_ => _contentProvider);
            builder.RegisterType<MarkdownService>().AsImplementedInterfaces();
            builder.RegisterType<DocumentRenderer>().AsImplementedInterfaces();
            return builder.Build();
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
