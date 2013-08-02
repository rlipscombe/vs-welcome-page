using System;
using System.Reflection;

namespace WelcomePage.Core
{
    public static class AppDomainExtensions
    {
        public static T CreateInstanceAndUnwrap<T>(this AppDomain domain,
                                                   string assemblyName, string typeName,
                                                   object[] args)
        {
            return (T)domain.CreateInstanceAndUnwrap(
                assemblyName, typeName, false,
                BindingFlags.CreateInstance, null,
                args,
                null, null);
        }
    }
}