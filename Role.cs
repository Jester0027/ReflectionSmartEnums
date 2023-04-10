namespace EnumsTest;

public enum Role
{
    [Data(name: "Administrator", claim: RoleClaims.Administrator)]
    Admin = 0,

    [Data(name: "User", claim: RoleClaims.User)]
    User = 1,

    [Data(name: "Guest", claim: RoleClaims.Guest)]
    Guest = 2,
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
    public string Name { get; set; }
    public string Claim { get; init; }

    public DataAttribute(string name, string claim)
    {
        Name = name;
        Claim = claim;
    }
}

public static class RoleUtils
{
    public static Role FromClaim(string claim)
    {
        return Enum
            .GetValues(typeof(Role))
            .Cast<Role>()
            .Single(role => role.GetClaim() == claim);
    }
}

public static class RoleExtensions
{
    public static string GetName(this Role role)
    {
        var attribute = role.GetAttribute<DataAttribute>();

        return attribute.Name;
    }

    public static string GetClaim(this Role role)
    {
        var attribute = role.GetAttribute<DataAttribute>();

        return attribute.Claim;
    }
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