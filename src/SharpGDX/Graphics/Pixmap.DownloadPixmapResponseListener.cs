namespace SharpGDX.Graphics;

public partial class Pixmap
{
    /// <summary>
    ///     Response listener for <see cref="DownloadFromUrl(string, IDownloadPixmapResponseListener)" />.
    /// </summary>
    public interface IDownloadPixmapResponseListener
    {
        /// <summary>
        ///     Called on the render thread when image was downloaded successfully.
        /// </summary>
        /// <param name="pixmap"></param>
        void DownloadComplete(Pixmap pixmap);

        /// <summary>
        ///     Called when image download failed. This might get called on a background thread.
        /// </summary>
        /// <param name="t"></param>
        void DownloadFailed(Exception t);
    }
}