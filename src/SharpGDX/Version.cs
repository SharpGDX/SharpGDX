namespace SharpGDX;

/// <summary>
///     The version of SharpGDX.
/// </summary>
public class Version
{
    /// <summary>
    ///     The current major version of SharpGDX.
    /// </summary>
    public static readonly int Major = 1;

    /// <summary>
    ///     The current minor version of SharpGDX.
    /// </summary>
    public static readonly int Minor = 13;

    /// <summary>
    ///     The current revision version of SharpGDX.
    /// </summary>
    public static readonly int Revision = 0;

    public static bool IsHigher(int major, int minor, int revision)
    {
        return IsHigherEqual(major, minor, revision + 1);
    }

    public static bool IsHigherEqual(int major, int minor, int revision)
    {
        if (Major != major)
        {
            return Major > major;
        }

        if (Minor != minor)
        {
            return Minor > minor;
        }

        return Revision >= revision;
    }

    public static bool IsLower(int major, int minor, int revision)
    {
        return IsLowerEqual(major, minor, revision - 1);
    }

    public static bool IsLowerEqual(int major, int minor, int revision)
    {
        if (Major != major)
        {
            return Major < major;
        }

        if (Minor != minor)
        {
            return Minor < minor;
        }

        return Revision <= revision;
    }

    /// <summary>
    ///     The current version of SharpGDX as a string in the major.minor.revision format.
    /// </summary>
    /// <returns>The current version of SharpGDX as a string in the major.minor.revision format.</returns>
    public override string ToString()
    {
        return $"{Major}.{Minor}.{Revision}";
    }
}