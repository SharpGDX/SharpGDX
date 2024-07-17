namespace SharpGDX.Shims;

public static class SharedLibraryLoader
{
	public static bool isMac = OperatingSystem.IsMacOS() || OperatingSystem.IsMacOS();
	public static bool isAndroid = OperatingSystem.IsAndroid();
	public static bool isWindows = OperatingSystem.IsWindows();
	public static bool isLinux = OperatingSystem.IsLinux();
	public static bool isIos = OperatingSystem.IsIOS();
}