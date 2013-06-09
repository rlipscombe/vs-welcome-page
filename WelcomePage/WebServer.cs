using System;
using System.Diagnostics;

namespace RogerLipscombe.WelcomePage
{
    public sealed class WebServer : MarshalByRefObject, IWebServer
    {
        private Process _process;

        public void Start(Uri url, string rootFolder)
        {
            // TODO: Extract server from assets.
            var fileName = @"D:\Source\vs-welcome-page\WelcomePage.WebServer\bin\Debug\WelcomePage.WebServer.exe";
            
            if (_process != null && !_process.HasExited)
                return;

            var startInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = string.Format("{0} \"{1}\"", url, rootFolder),
                    WindowStyle = ProcessWindowStyle.Hidden
                };

            _process = Process.Start(startInfo);
            Log.Message("Web Server started ({0} {1}). Process ID = {2}.",
                        startInfo.FileName, startInfo.Arguments, _process.Id);
        }

        public void Stop()
        {
            if (_process != null)
            {
                _process.Kill();
                _process.WaitForExit(milliseconds: 1000);
                _process = null;
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}