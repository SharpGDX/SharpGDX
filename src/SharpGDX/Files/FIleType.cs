namespace SharpGDX.Files;

/// <summary>
///     Indicates how to resolve a path to a file.
/// </summary>
public enum FileType
{
    /// <summary>
    ///     Path relative to the root of the classpath.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Classpath files are always readonly.
    ///     </para>
    ///     <para>
    ///         Note that classpath files are not compatible with some functionality on Android, such as
    ///         <see cref="IAudio.NewSound(FileHandle)" /> and <see cref="IAudio.NewMusic(FileHandle)" />.
    ///     </para>
    /// </remarks>
    Classpath,

    /// <summary>
    ///     Path relative to the asset directory on Android and to the application's root directory on the desktop.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         On the desktop, if the file is not found, then the classpath is checked. This enables files to be found when
    ///         using JWS or applets.
    ///     </para>
    ///     <para>
    ///         Internal files are always readonly.
    ///     </para>
    /// </remarks>
    Internal,

    /// <summary>
    ///     Path relative to the root of the app external storage on Android and to the home directory of the current user on
    ///     the desktop.
    /// </summary>
    External,

    /// <summary>
    ///     Path that is a fully qualified, absolute filesystem path. To ensure portability across platforms use absolute files
    ///     only when absolutely necessary.
    /// </summary>
    Absolute,

    /// <summary>
    ///     Path relative to the private files directory on Android and to the application's root directory on the desktop.
    /// </summary>
    Local
}