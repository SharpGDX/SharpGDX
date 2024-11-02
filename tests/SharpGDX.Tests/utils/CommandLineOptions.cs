using SharpGDX.Graphics;
using SharpGDX.Graphics.G2D;
using SharpGDX.Scenes.Scene2D;
using SharpGDX.Scenes.Scene2D.Utils;
using SharpGDX.Utils.Viewports;
using SharpGDX.Mathematics;
using SharpGDX.Tests.Utils;
using SharpGDX.Utils;

namespace SharpGDX.Tests.Utils;

/** Shared class for desktop launchers.
 * 
 * options: --gl30, --gl31, --gl32 enable GLES 3.x (default is GLES 2.0) --glErrors enable GLProfiler and log any GL errors.
 * (default is disabled) */
public class CommandLineOptions
{

	public String startupTestName = null;
	public bool gl30 = false;
	public bool gl31 = false;
	public bool gl32 = false;
	public bool angle = false;
	public bool logGLErrors = false;

	public CommandLineOptions(String[] argv)
	{
		Array<String> args = new Array<String>(argv);
		foreach (String arg in args)
		{
			if (arg.StartsWith("-"))
			{
				if (arg.Equals("--gl30"))
					gl30 = true;
				else if (arg.Equals("--gl31"))
					gl31 = true;
				else if (arg.Equals("--gl32"))
					gl32 = true;
				else if (arg.Equals("--glErrors"))
					logGLErrors = true;
				else if (arg.Equals("--angle"))
					angle = true;
				else
					Console.WriteLine("skip unrecognized option " + arg);
			}
			else
			{
				startupTestName = arg;
			}
		}
		if ((gl30 || gl31 || gl32) && angle)
		{
			throw new GdxRuntimeException("Both --gl3[0|1|2] and --angle set. Can not be combined.");
		}
	}

	public bool isTestCompatible(String testName)
	{
		Type clazz = GdxTests.forName(testName);
		GdxTestConfig? config = clazz.GetCustomAttributes(typeof(GdxTestConfig), true)?[0] as GdxTestConfig;
		if (config != null) {
			if (config.RequireGL32 && !gl32) return false;
			if (config.RequireGL31 && !(gl31 || gl32)) return false;
			if (config.RequireGL30 && !(gl30 || gl31 || gl32)) return false;
			if (config.OnlyGL20 && (gl30 || gl31 || gl32)) return false;
		}
		return true;
	}

	public Object[] getCompatibleTests()
{
	List<String> names = new();
	foreach (String name in GdxTests.getNames())
	{
		if (isTestCompatible(name)) names.Add(name);
	}
	return names.ToArray();
}
}
