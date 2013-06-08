using System.Diagnostics;
using Microsoft.VisualStudio.Shell;

namespace RogerLipscombe.WelcomePage
{
    internal static class Log
    {
        public static void Message(string message)
        {
            Debug.WriteLine(message);
            ActivityLog.LogInformation("WelcomePage", message);
        }

        public static void Message(string format, object arg0)
        {
            var message = string.Format(format, arg0);
            Message(message);
        }

        public static void Message(string format, object arg0, object arg1)
        {
            var message = string.Format(format, arg0, arg1);
            Message(message);
        }

        public static void Message(string format, params object[] args)
        {
            var message = string.Format(format, args);
            Message(message);
        }
    }
}