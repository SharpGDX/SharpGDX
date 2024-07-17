namespace SharpGDX.Headless;

public class HeadlessApplicationConfiguration
{
	/**
	 * The maximum number of threads to use for network requests. Default is {@link Integer#MAX_VALUE}.
	 */
	public int maxNetThreads = int.MaxValue;

	/**
	 * Preferences directory for headless. Default is ".prefs/".
	 */
	public string preferencesDirectory = ".prefs/";

	/**
	 * The amount of updates targeted per second. Use 0 to never sleep; negative to not call the render method at all. Default is
	 * 60.
	 */
	public int updatesPerSecond = 60;
}