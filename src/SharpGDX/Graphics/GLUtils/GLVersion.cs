using System.Text.RegularExpressions;

namespace SharpGDX.Graphics.GLUtils;

public class GLVersion
{
	public enum GLType
	{
		OpenGL,
		GLES,
		WebGL,
		None
	}

	private const string Tag = "GLVersion";
	
	private readonly string _rendererString;
	private readonly GLType _type;
	private readonly string _vendorString;

	private int _majorVersion;
	private int _minorVersion;
	private int _releaseVersion;

	public GLVersion(IApplication.ApplicationType appType, string versionString, string vendorString, string rendererString)
	{
		switch (appType)
		{
			case IApplication.ApplicationType.Android:
			case IApplication.ApplicationType.iOS:
				_type = GLType.GLES;
				break;
			case IApplication.ApplicationType.Desktop:
			case IApplication.ApplicationType.Applet:
				_type = GLType.OpenGL;
				break;
			case IApplication.ApplicationType.WebGL:
				_type = GLType.WebGL;
				break;
			default:
				_type = GLType.None;
				break;
		}

		switch (_type)
		{
			case GLType.GLES:
				ExtractVersion("OpenGL ES (\\d(\\.\\d){0,2})", versionString);
				break;
			case GLType.WebGL:
				ExtractVersion("WebGL (\\d(\\.\\d){0,2})", versionString);
				break;
			case GLType.OpenGL:
				ExtractVersion("(\\d(\\.\\d){0,2})", versionString);
				break;
			case GLType.None:
			default:
				_majorVersion = -1;
				_minorVersion = -1;
				_releaseVersion = -1;
				vendorString = "";
				rendererString = "";
				break;
		}

		_vendorString = vendorString;
		_rendererString = rendererString;
	}

	/**
	 * @return a string with the current GL connection data
	 */
	public string GetDebugVersionString()
	{
		return "Type: " + _type + "\n" + "Version: " + _majorVersion + ":" + _minorVersion + ":" + _releaseVersion +
		       "\n" + "Vendor: "
		       + _vendorString + "\n" + "Renderer: " + _rendererString;
	}

	/**
	 * @return the major version of current GL connection. -1 if running headless
	 */
	public int GetMajorVersion()
	{
		return _majorVersion;
	}

	/**
	 * @return the minor version of the current GL connection. -1 if running headless
	 */
	public int GetMinorVersion()
	{
		return _minorVersion;
	}

	/**
	 * @return the release version of the current GL connection. -1 if running headless
	 */
	public int GetReleaseVersion()
	{
		return _releaseVersion;
	}

	/**
	 * @return the name of the renderer associated with the current GL connection. This name is typically specific to a particular
	 * configuration of a hardware platform.
	 */
	public string GetRendererString()
	{
		return _rendererString;
	}

	/**
	 * @return what {@link Type} of GL implementation this application has access to, e.g. {@link Type#OpenGL} or
	 * {@link Type#GLES}
	 */
	public GLType GetType()
	{
		return _type;
	}

	/**
	 * @return the vendor string associated with the current GL connection
	 */
	public string GetVendorString()
	{
		return _vendorString;
	}

	/**
	 * Checks to see if the current GL connection version is higher, or equal to the provided test versions.
	 * 
	 * @param testMajorVersion the major version to test against
	 * @param testMinorVersion the minor version to test against
	 * @return true if the current version is higher or equal to the test version
	 */
	public bool IsVersionEqualToOrHigher(int testMajorVersion, int testMinorVersion)
	{
		return _majorVersion > testMajorVersion ||
		       (_majorVersion == testMajorVersion && _minorVersion >= testMinorVersion);
	}

	/** Forgiving parsing of gl major, minor and release versions as some manufacturers don't adhere to spec **/
	private static int ParseInt(string v, int defaultValue)
	{
		try
		{
			return int.Parse(v);
		}
		catch (FormatException)
		{
			Gdx.app.error("libGDX GL", "Error parsing number: " + v + ", assuming: " + defaultValue);

			return defaultValue;
		}
	}

	private void ExtractVersion(string patternString, string versionString)
	{
		var regex = new Regex(patternString);
		var matcher = regex.Match(versionString);
		var found = matcher.Success;

		if (found)
		{
			var result = matcher.Groups[0].Value;
			var resultSplit = result.Split('.');
			_majorVersion = ParseInt(resultSplit[0], 2);
			_minorVersion = resultSplit.Length < 2 ? 0 : ParseInt(resultSplit[1], 0);
			_releaseVersion = resultSplit.Length < 3 ? 0 : ParseInt(resultSplit[2], 0);
		}
		else
		{
			Gdx.app.log(Tag, "Invalid version string: " + versionString);
			_majorVersion = 2;
			_minorVersion = 0;
			_releaseVersion = 0;
		}
	}
}