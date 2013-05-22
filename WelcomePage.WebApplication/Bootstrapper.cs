using Nancy;

namespace WelcomePage.WebApplication
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        /// <summary>
        /// Disable default favicon by returning null.
        /// </summary>
        protected override byte[] FavIcon
        {
            get { return null; }
        }
    }
}