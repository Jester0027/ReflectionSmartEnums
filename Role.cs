namespace EnumsTest;

[SmartEnum<DataAttribute>]
public enum Role
{
    [Data(name: "Administrator", claim: RoleClaims.Administrator)]
    Admin = 0,

    [Data(name: "User", claim: RoleClaims.User)]
    User = 1,

    [Data(name: "Guest", claim: RoleClaims.Guest)]
    Guest = 2
}

public static class RoleClaims
{
    public const string Administrator = "com.example.roles.administrator";
    public const string User = "com.example.roles.user";
    public const string Guest = "com.example.roles.guest";
}

[AttributeUsage(AttributeTargets.Field)]
public sealed class DataAttribute : Attribute
{
    public string Name { get; }
    public string Claim { get; }

    public DataAttribute(string name, string claim)
    {
        Name = name;
        Claim = claim;
    }
}

/// <summary>
/// Marks an enum as a smart enum with the given data attribute for each value.
/// TODO Used for compile-time checking. Verifying that each value has a data attribute with the correct type, and that the the enum is not an enum with flags.
/// </summary>
/// <typeparam name="TData"></typeparam>
[AttributeUsage(AttributeTargets.Enum)]
public sealed class SmartEnumAttribute<TData> : Attribute where TData : Attribute
{
    /// <summary>
    /// The type of the data attribute that each value must have.
    /// </summary>
    public Type Type { get; set; } = typeof(TData);
}

public static class RoleUtils
{
    /// <summary>
    /// Converts a claim string to a Role enum value
    /// </summary>
    /// <param name="claim"></param>
    /// <returns></returns>
    public static Role FromClaim(string claim) =>
        Enum.GetValues<Role>()
            .Single(role => role.GetClaim() == claim);
}

public static class RoleExtensions
{
    public static string GetName(this Role role)
        => role.GetAttribute<DataAttribute>().Name;

    public static string GetClaim(this Role role)
        => role.GetAttribute<DataAttribute>().Claim;
}

public static class EnumExtensions
{
    public static TAttribute GetAttribute<TAttribute>(this Enum value)
        where TAttribute : Attribute
    {
        var type = value.GetType();
        var name = Enum.GetName(type, value);
        if (name is null)
        {
            throw new ArgumentOutOfRangeException(nameof(value), value, null);
        }

        var field = type.GetField(name);
        if (field is null)
        {
            throw new ArgumentOutOfRangeException(nameof(value), value, null);
        }

        if (Attribute.GetCustomAttribute(field, typeof(TAttribute)) is not TAttribute attribute)
        {
            throw new ArgumentOutOfRangeException(nameof(value), value, null);
        }

        return attribute;
    }
}