using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using CodeForDotNet.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeForDotNet.Tests.Facts;

/// <summary>
/// Tests the error reporting classes.
/// </summary>
[TestClass]
public class ErrorReportTests
{
    #region Public Methods

    /// <summary>
    /// Tests serialization of an <see cref="ErrorReportData"/>.
    /// </summary>
    [TestMethod]
    [RequiresUnreferencedCode("Serialization.")]
    [RequiresDynamicCode("Serializaton.")]
    public void ErrorReportTestSerialization()
    {
        // Create test object
        var report = CreateTestReport();
        report.EventDate = report.EventDate.TruncateToMilliseconds();        // Allow millisecond accuracy (e.g. JSON serialized)

        // Serialize as XML
        const string dataContractFileName = "ErrorReport.xml";
        var dataContract = new DataContractSerializer(typeof(ErrorReportData));
        using (var writer = File.Create(dataContractFileName))
            dataContract.WriteObject(writer, report);

        // De-serialize XML
        ErrorReportData dataContractReport;
        using (var reader = File.OpenRead(dataContractFileName))
            dataContractReport = (ErrorReportData)dataContract.ReadObject(reader)!;

        // Check XML serialized contents match
        Assert.AreEqual(report, dataContractReport);

        // Serialize as JSON
        const string jsonFileName = "ErrorReport.json";
        var json = new DataContractJsonSerializer(typeof(ErrorReportData[]));
        using (var file = File.Create(jsonFileName))
            json.WriteObject(file, new[] { report });

        // De-serialize JSON
        ErrorReportData jsonReport;
        using (var reader = File.OpenRead(jsonFileName))
            jsonReport = ((ErrorReportData[])json.ReadObject(reader)!)[0];

        // Check JSON serialized contents match
        Assert.AreEqual(report, jsonReport);
    }

    #endregion Public Methods

    #region Private Methods

    /// <summary>
    /// Creates an <see cref="ErrorReportData"/> test object.
    /// </summary>
    private static ErrorReportData CreateTestReport()
    {
        var report = new ErrorReportData {
            Id = Guid.NewGuid(),
            SourceId = Guid.NewGuid(),
            SourceAssemblyName = Assembly.GetExecutingAssembly().GetName().ToString(),
            EventDate = DateTime.UtcNow,
            Message = "Some argument was null, oh no!",
            ErrorTypeFullName = typeof(ArgumentNullException).FullName,
            StackTrace = new StackTrace().ToString()
        };
        return report;
    }

    #endregion Private Methods
}
