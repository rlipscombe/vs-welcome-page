using System.Configuration;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.TinyIoc;
using Nancy.ViewEngines;

namespace WelcomePage.Core
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        private IDocumentFolder _documentFolder;

        /// <summary>
        /// Used in the test web application; assumes that Web
        /// </summary>
        public Bootstrapper()
            : this(DocumentFolder.Create(ConfigurationManager.AppSettings["RootFolder"]))
        {
        }

        public Bootstrapper(IDocumentFolder documentFolder)
        {
            _documentFolder = documentFolder;
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);

            container.Register<IDocumentRenderer, DocumentRenderer>(new DocumentRenderer(_documentFolder));
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            ResourceViewLocationProvider.RootNamespaces.Add(GetType().Assembly, "WelcomePage.Core.Views");
        }

        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            base.ConfigureConventions(nancyConventions);

            nancyConventions.StaticContentsConventions.Add(
                EmbeddedStaticContentsConventionBuilder.Add("/Content/", GetType().Assembly, "WelcomePage.Core.Content"));
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
    }
}
