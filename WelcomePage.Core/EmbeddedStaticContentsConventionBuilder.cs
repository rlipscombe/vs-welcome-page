using System;
using System.Reflection;
using Nancy;
using Nancy.Responses;

namespace WelcomePage.WebApplication
{
    static class EmbeddedStaticContentsConventionBuilder
    {
        // TODO: Replace this with the one from mainline, once 0.18 is out.
        public static Func<NancyContext, string, Response> Add(string prefix, Assembly assembly, string resourcePath)
        {
            return
                (context, applicationFolder) =>
                    {
                        var requestPath = context.Request.Path;
                        if (!requestPath.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase))
                            return null;

                        var name = requestPath.Substring(prefix.Length);
                        return new EmbeddedFileResponse(assembly, resourcePath, name);
                    };
        }
    }
}