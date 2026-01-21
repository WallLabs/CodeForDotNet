using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// Code Analysis.
[assembly: SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "Assembly is Windows specific.")]

// Testing.
[assembly: Parallelize(Scope = ExecutionScope.ClassLevel)]
