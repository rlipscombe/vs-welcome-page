// Guids.cs
// MUST match guids.h
using System;

namespace RogerLipscombe.WelcomePage
{
    static class GuidList
    {
        public const string guidWelcomePagePkgString = "947a85ca-9a1e-45e4-86bd-e0c4ef3eda66";
        public const string guidWelcomePageCmdSetString = "24b73a04-f79c-4692-98dd-0816021fe451";

        public static readonly Guid guidWelcomePageCmdSet = new Guid(guidWelcomePageCmdSetString);
    };
}