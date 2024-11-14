namespace SharpGDX;

public partial interface IApplication
{
    /// <summary>
    ///     Enumeration of possible <see cref="IApplication" /> types.
    /// </summary>
    public enum ApplicationType
    {
        Android,
        Desktop,
        HeadlessDesktop,
        Applet,
        WebGL,
        IOS
    }
}