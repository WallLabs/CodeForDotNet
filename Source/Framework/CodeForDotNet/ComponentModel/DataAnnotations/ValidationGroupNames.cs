namespace CodeForDotNet.ComponentModel.DataAnnotations;

/// <summary>
/// Defines common validation group names as useful constants for <see cref="ValidationGroupAttribute"/>.
/// </summary>
public static class ValidationGroupNames
{
    /// <summary>
    /// Common validation group name used when creating an entity.
    /// </summary>
    public const string Create = nameof(Create);

    /// <summary>
    /// Common validation group name used when updating an entity.
    /// </summary>
    public const string Update = nameof(Update);
}
