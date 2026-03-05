namespace CodeForDotNet.Tests.Mocks;

/// <summary>
/// Authorization Store permission names.
/// </summary>
public static class TestPermissions
{
    /// <summary>
    /// View the user interface.
    /// </summary>
    public const string ViewUserInterface = nameof(ViewUserInterface);

    /// <summary>
    /// Run a test.
    /// </summary>
    public const string RunTest = nameof(RunTest);

    /// <summary>
    /// Always denied, used to test authorization failures.
    /// </summary>
    public const string AccessDenied = nameof(AccessDenied);

    /// <summary>
    /// View test data.
    /// </summary>
    public const string TestDataView = nameof(TestDataView);

    /// <summary>
    /// Create test data.
    /// </summary>
    public const string TestDataCreate = nameof(TestDataCreate);

    /// <summary>
    /// Edit test data.
    /// </summary>
    public const string TestDataEdit = nameof(TestDataEdit);

    /// <summary>
    /// Delete test data.
    /// </summary>
    public const string TestDataDelete = nameof(TestDataDelete);
}
