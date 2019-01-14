using System;
using System.Resources;
using System.Reflection;

// Information
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("CodeChief")]
[assembly: AssemblyDescription("Core components, portable across all currently supported framework versions.")]
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Retail")]
#endif
[assembly: AssemblyCompany("Code Chief")]
[assembly: AssemblyProduct("Components")]
[assembly: AssemblyCopyright("Copyright Anthony Brian Wall")]
[assembly: AssemblyTrademark("All Rights Reserved")]
[assembly: AssemblyCulture("")]

// Version
[assembly: AssemblyVersion("4.51.1502.28008")]
[assembly: AssemblyFileVersion("4.51.1502.28008")]

// Globalization
[assembly: NeutralResourcesLanguage("en-US")]

// Assembly is CLS compliant (unless specified otherwise on individual members)
[assembly: CLSCompliant(true)]
