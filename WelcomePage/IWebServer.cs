using System;

namespace RogerLipscombe.WelcomePage
{
    public interface IWebServer
    {
        void Start(Uri url, string rootFolder);
        void Stop();
    }
}