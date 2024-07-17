using SharpGDX.Shims;
using SharpGDX.Utils;

namespace SharpGDX;

/**
 * The version of libGDX
 * 
 * @author mzechner
 */
public class Version
{
	/** The current major version of libGDX **/
	public static readonly int MAJOR;

	/** The current minor version of libGDX **/
	public static readonly int MINOR;

	/** The current revision version of libGDX **/
	public static readonly int REVISION;

	/** The current version of libGDX as a String in the major.minor.revision format **/
	public static readonly string VERSION = "1.12.1";

	static Version()
	{
		try
		{
			var v = VERSION.Split("\\.");
			MAJOR = v.Length < 1 ? 0 : Integer.ValueOf(v[0]);
			MINOR = v.Length < 2 ? 0 : Integer.ValueOf(v[1]);
			REVISION = v.Length < 3 ? 0 : Integer.ValueOf(v[2]);
		}
		catch (Exception t)
		{
			// Should never happen
			throw new GdxRuntimeException("Invalid version " + VERSION, t);
		}
	}

	public static bool isHigher(int major, int minor, int revision)
	{
		return isHigherEqual(major, minor, revision + 1);
	}

	public static bool isHigherEqual(int major, int minor, int revision)
	{
		if (MAJOR != major)
		{
			return MAJOR > major;
		}

		if (MINOR != minor)
		{
			return MINOR > minor;
		}

		return REVISION >= revision;
	}

	public static bool isLower(int major, int minor, int revision)
	{
		return isLowerEqual(major, minor, revision - 1);
	}

	public static bool isLowerEqual(int major, int minor, int revision)
	{
		if (MAJOR != major)
		{
			return MAJOR < major;
		}

		if (MINOR != minor)
		{
			return MINOR < minor;
		}

		return REVISION <= revision;
	}
}