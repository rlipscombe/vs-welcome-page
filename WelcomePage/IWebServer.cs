using System;

namespace RogerLipscombe.WelcomePage
{
    public interface IWebServer : IDisposable
    {
        void Start(Uri url, string rootFolder);
        void Stop();
    }
}