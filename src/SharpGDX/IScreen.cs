using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX
{
	/**
 * <p>
 * Represents one of many application screens, such as a main menu, a settings menu, the game screen and so on.
 * </p>
 * <p>
 * Note that {@link #dispose()} is not called automatically.
 * </p>
 * @see Game */
	public interface IScreen
	{

		/// <summary>
		/// Called when this screen becomes the current screen for a <see cref="Game"/>.
		/// </summary>
		public void Show();

		/// <summary>
		/// Called when the screen should render itself.
		/// </summary>
		/// <param name="delta">The time in seconds since the last render.</param>
		public void Render(float delta);

		/// <inheritdoc cref="IApplicationListener.Resize"/>>
		public void Resize(int width, int height);

		/// <inheritdoc cref="IApplicationListener.Pause"/>>
		public void Pause();

		/// <inheritdoc cref="IApplicationListener.Resume"/>
		public void Resume();

		/// <summary>
		/// Called when this screen is no longer the current screen for a <see cref="Game"/>.
		/// </summary>
		public void Hide();

		/// <summary>
		/// Called when this screen should release all resources.
		/// </summary>
		public void Dispose();
	}
}
