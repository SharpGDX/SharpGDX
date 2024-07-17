namespace SharpGDX.Desktop;

/// <summary>
///     Convenience implementation of {@link DesktopWindowListener}.
/// </summary>
/// <remarks>
///     Derive from this class and only overwrite the methods you are interested in.
/// </remarks>
public class DesktopAdapter : IDesktopWindowListener
{
	public virtual bool CloseRequested()
	{
		return true;
	}

	public virtual void Created(DesktopWindow window)
	{
	}

	public virtual void FilesDropped(string[] files)
	{
	}

	public virtual void FocusGained()
	{
	}

	public virtual void FocusLost()
	{
	}

	public virtual void Iconified(bool isIconified)
	{
	}

	public virtual void Maximized(bool isMaximized)
	{
	}

	public virtual void RefreshRequested()
	{
	}
}