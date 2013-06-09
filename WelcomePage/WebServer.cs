using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;

namespace RogerLipscombe.WelcomePage
{
    public sealed class WebServer : MarshalByRefObject, IWebServer
    {
        private Process _process;

        public void Start(Uri url, string rootFolder)
        {
            if (_process != null && !_process.HasExited)
                return;

            // Figure out where the web application binaries are.
            string directoryName = Path.GetDirectoryName(typeof(WelcomePagePackage).Assembly.Location);
            var webApplicationAssets = Path.Combine(directoryName, "WelcomePage.WebServer.zip");
            Log.Message("Web Application Assets = '{0}'", webApplicationAssets);

            // Copy the web application binaries to a new temporary location.
            var instanceDirectory = Path.Combine(Path.GetTempPath(), "WelcomePage.WebServer");
            ExtractWebAppFiles(webApplicationAssets, instanceDirectory);

            var fileName = Path.Combine(instanceDirectory, "WelcomePage.WebServer.exe");

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

        private void ExtractWebAppFiles(string zipFileName, string instanceDirectory)
        {
            Log.Message("Extracting Web Application from '{0}'...", zipFileName);

            using (var zip = new ZipArchive(File.OpenRead(zipFileName), ZipArchiveMode.Read))
            {
                foreach (var entry in zip.Entries)
                {
                    var destinationFileName = Path.Combine(instanceDirectory, entry.FullName);

                    if (!File.Exists(destinationFileName) ||
                        (entry.LastWriteTime > File.GetLastWriteTimeUtc(destinationFileName)))
                    {
                        Log.Message("Extracting '{0}' to '{1}'...", entry.FullName, destinationFileName);

                        var destinationDirectory = Path.GetDirectoryName(destinationFileName);
                        if (!Directory.Exists(destinationDirectory))
                            Directory.CreateDirectory(destinationDirectory);

                        using (var entryStream = entry.Open())
                        using (
                            var destinationStream = File.Open(destinationFileName, FileMode.Create, FileAccess.Write,
                                                              FileShare.Read))
                        {
                            entryStream.CopyTo(destinationStream);
                        }
                    }
                    else
                    {
                        Log.Message("Skipping '{0}' because it is up-to-date.", destinationFileName);
                    }
                }
            }
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