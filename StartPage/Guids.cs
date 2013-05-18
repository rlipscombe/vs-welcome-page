// Guids.cs
// MUST match guids.h
using System;

namespace RogerLipscombe.StartPage
{
    static class GuidList
    {
        public const string guidStartPagePkgString = "947a85ca-9a1e-45e4-86bd-e0c4ef3eda66";
        public const string guidStartPageCmdSetString = "24b73a04-f79c-4692-98dd-0816021fe451";

        public static readonly Guid guidStartPageCmdSet = new Guid(guidStartPageCmdSetString);
    };
}