using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Resources;

// Code Analysis.
[assembly: SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "Assembly is Windows specific.")]

// Legacy settings used by projects which do not support SDK-style project properties via Directory.Build.props, e.g. Windows SDK.

// Identity.
[assembly: AssemblyVersion("10.0.2603.5003")]

// Resources.
[assembly: NeutralResourcesLanguage("en-US")]
