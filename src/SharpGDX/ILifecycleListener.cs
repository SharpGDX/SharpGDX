namespace SharpGDX;

/// <summary>
///     An <see cref="ILifecycleListener" /> can be added to an <see cref="IApplication" /> via
///     <see cref="IApplication.addLifecycleListener(ILifecycleListener)" />. It will receive notification of pause, resume
///     and dispose events. This is mainly meant to be used by extensions that need to manage resources based on the
///     life-cycle. Normal, application level development should rely on the <see cref="IApplicationListener" /> interface.
/// </summary>
/// <remarks>
///     <para>
///         The methods will be invoked on the rendering thread.
///     </para>
///     <para>
///         The methods will be executed before the <see cref="IApplicationListener" /> methods are executed.
///     </para>
/// </remarks>
public interface ILifecycleListener
{
	/// <summary>
	///     Called when the <see cref="IApplication" /> is about to be disposed.
	/// </summary>
	public void Dispose();

	/// <summary>
	///     Called when the <see cref="IApplication" /> is about to pause.
	/// </summary>
	public void Pause();

	/// <summary>
	///     Called when the <see cref="IApplication" /> is about to be resumed.
	/// </summary>
	public void Resume();
}