using System;
using System.IO;
using Nancy;
using Nancy.ErrorHandling;
using Nancy.ViewEngines;

namespace WelcomePage.Core
{
    public class ErrorHandler : IStatusCodeHandler
    {
        private readonly IViewFactory _viewFactory;

        public ErrorHandler(IViewFactory viewFactory)
        {
            _viewFactory = viewFactory;
        }

        public bool HandlesStatusCode(HttpStatusCode statusCode, NancyContext context)
        {
            if (statusCode == HttpStatusCode.InternalServerError)
                return true;

            return false;
        }

        public void Handle(HttpStatusCode statusCode, NancyContext context)
        {
            object errorObject;
            context.Items.TryGetValue(NancyEngine.ERROR_EXCEPTION, out errorObject);

            var exception = errorObject as Exception;
            if (exception != null)
            {
                if (exception is RequestExecutionException)
                    exception = exception.InnerException;

                Handle(exception, context);
            }
        }

        private void Handle(Exception exception, NancyContext context)
        {
            // TODO: Also DirectoryNotFoundException.
            var fileNotFoundException = exception as FileNotFoundException;
            if (fileNotFoundException != null)
            {
                RenderView(context, "Errors/404", HttpStatusCode.NotFound, new {fileNotFoundException.Message});
                return;
            }

            var directoryNotFoundException = exception as DirectoryNotFoundException;
            if (directoryNotFoundException != null)
            {
                RenderView(context, "Errors/404", HttpStatusCode.NotFound, new {directoryNotFoundException.Message});
                return;
            }

            RenderView(context, "Errors/500", HttpStatusCode.InternalServerError, new {exception.Message});
        }

        private void RenderView(NancyContext context, string viewName, HttpStatusCode statusCode, object model)
        {
            var renderer = new DefaultViewRenderer(_viewFactory);
            var response = renderer.RenderView(context, viewName, model);

            context.Response = response;
            context.Response.StatusCode = statusCode;
        }
    }
}