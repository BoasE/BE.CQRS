using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace BE.CQRS.Domain
{
    internal static class CurrentVersion
    {
        internal static readonly Lazy<string> FrameworkEventVersion = new Lazy<string>(ResolveVersion);

        internal static string ResolveVersion()
        {
            return "0.70.5";
        }
    }
}