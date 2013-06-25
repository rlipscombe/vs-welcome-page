using System.Diagnostics;
using Nancy;
using Nancy.TinyIoc;

namespace WelcomePage.Core
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        private static readonly TraceSource Trace = new TraceSource("WelcomePage.Core");

        private readonly string _rootDirectory;

        public Bootstrapper(string rootDirectory)
        {
            _rootDirectory = rootDirectory;
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            Trace.TraceInformation("Bootstrapper.ConfigureApplicationContainer");
            base.ConfigureApplicationContainer(container);

            container.Register<IDocumentFolder>((c,p)=> new DocumentFolder(_rootDirectory));
        }

        protected override IRootPathProvider RootPathProvider
        {
            // TODO: Do we need this? If so, put a comment on it.
            get { return new DefaultRootPathProvider(); }
        }
    }
}
