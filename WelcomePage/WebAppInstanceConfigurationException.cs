using System;
using System.Runtime.Serialization;

namespace RogerLipscombe.WelcomePage
{
    [Serializable]
    internal class WebAppInstanceConfigurationException : Exception
    {
        public WebAppInstanceConfigurationException()
        {
            // For serialization.
        }

        protected WebAppInstanceConfigurationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            // For serialization.
        }

        public WebAppInstanceConfigurationException(string instanceDirectory, Exception innerException)
            : base(string.Format("Failed to configure web app instance at '{0}'.", instanceDirectory), innerException)
        {
        }
    }
}