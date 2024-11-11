using SharpGDX.Files;

namespace SharpGDX;

/// <summary>
///     Provides standard access to the filesystem, classpath, Android app storage (internal and external), and Android
///     assets directory.
/// </summary>
public interface IFiles
{
    /// <summary>
    ///     Convenience method that returns a <see cref="FileType.Absolute" /> file handle.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public FileHandle Absolute(string path);

    /// <summary>
    ///     Convenience method that returns a <see cref="FileType.Classpath" /> file handle.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public FileHandle Classpath(string path);

    /// <summary>
    ///     Convenience method that returns a <see cref="FileType.External" /> file handle.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public FileHandle External(string path);

    /// <summary>
    ///     Returns the external storage path directory. This is the app external storage on Android and the home directory of
    ///     the current user on the desktop.
    /// </summary>
    /// <returns></returns>
    public string GetExternalStoragePath();

    /// <summary>
    ///     Returns a handle representing a file or directory.
    /// </summary>
    /// <param name="path">Determines how the path is resolved.</param>
    /// <param name="type"></param>
    /// <returns></returns>
    /// <exception cref="Utils.GdxRuntimeException">If the type is classpath or internal and the file does not exist.</exception>
    public FileHandle GetFileHandle(string path, FileType type);

    /// <summary>
    ///     Returns the local storage path directory. This is the private files directory on Android and the directory of the
    ///     jar on the desktop.
    /// </summary>
    /// <returns></returns>
    public string GetLocalStoragePath();

    /// <summary>
    ///     Convenience method that returns a <see cref="FileType.Internal" /> file handle.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public FileHandle Internal(string path);

    /// <summary>
    ///     Returns true if the external storage is ready for file IO.
    /// </summary>
    /// <returns></returns>
    public bool IsExternalStorageAvailable();

    /// <summary>
    ///     Returns true if the local storage is ready for file IO.
    /// </summary>
    /// <returns></returns>
    public bool IsLocalStorageAvailable();

    /// <summary>
    ///     Convenience method that returns a <see cref="FileType.Local" /> file handle.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public FileHandle Local(string path);
}