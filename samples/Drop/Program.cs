using SharpGDX.Desktop;
using SharpGDX.Shims;

namespace Drop;

internal class Program
{
	private static void Main(string[] args)
	{
		var config = new DesktopApplicationConfiguration();
		config.setTitle("Drop");
		config.setWindowedMode(800, 480);
		config.useVsync(true);
		config.setForegroundFPS(60);
		config.enableGLDebugOutput(true, new PrintStream());
		new DesktopApplication(new Drop(), config);
	}
}