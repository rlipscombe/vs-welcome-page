using System;
using System.Reflection;

namespace WelcomePage.WebApplication
{
    internal static class AttributeExtensions
    {
        public static T GetCustomAttribute<T>(this Assembly assembly)
        {
            var attributes = assembly.GetCustomAttributes(typeof (T), inherit: true);
            if (attributes.Length == 0)
            {
                throw new InvalidOperationException(
                    string.Format("Custom attribute of type '{0}' was not found on assembly '{1}'.",
                                  typeof (T), assembly));
            }

            if (attributes.Length > 1)
            {
                throw new InvalidOperationException(
                    string.Format("Multiple instances of custom attribute of type '{0}' were found on assembly '{1}'.",
                                  typeof (T), assembly));
            }

            return (T) attributes[0];
        }
    }
}