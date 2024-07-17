namespace SharpGDX;

/// <summary>
///     An IInputProcessor is used to receive input events from the keyboard and the touch screen (mouse on the desktop).
/// </summary>
/// <remarks>
///     <para>
///         Must be registered with the <see cref="IInput.setInputProcessor(IInputProcessor)" /> method.
///     </para>
///     <para>
///         It will be called each frame before the call to <see cref="IApplicationListener.Render" />.
///     </para>
///     <para>
///         Each method returns a boolean in case you want to use this with the <see cref="InputMultiplexer" /> to chain
///         input processors.
///     </para>
/// </remarks>
public interface IInputProcessor
{
	/// <summary>
	///     Called when a key was pressed.
	/// </summary>
	/// <param name="keycode">One of the constants in <see cref="IInput.Keys" />.</param>
	/// <returns>Whether the input was processed.</returns>
	public bool KeyDown(int keycode);

	/// <summary>
	///     Called when a key was typed.
	/// </summary>
	/// <param name="character">The character.</param>
	/// <returns>Whether the input was processed.</returns>
	public bool KeyTyped(char character);

	/// <summary>
	///     Called when a key was released.
	/// </summary>
	/// <param name="keycode">One of the constants in <see cref="IInput.Keys" />.</param>
	/// <returns>Whether the input was processed.</returns>
	public bool KeyUp(int keycode);

	/// <summary>
	///     Called when the mouse was moved without any buttons being pressed.
	/// </summary>
	/// <remarks>
	///     Will not be called on iOS.
	/// </remarks>
	/// <param name="screenX">The x coordinate, origin is in the upper left corner.</param>
	/// <param name="screenY">The y coordinate, origin is in the upper left corner.</param>
	/// <returns>Whether the input was processed.</returns>
	public bool MouseMoved(int screenX, int screenY);

	/// <summary>
	///     Called when the mouse wheel was scrolled.
	/// </summary>
	/// <remarks>
	///     Will not be called on iOS.
	/// </remarks>
	/// <param name="amountX">
	///     The horizontal scroll amount, negative or positive depending on the direction the wheel was
	///     scrolled.
	/// </param>
	/// <param name="amountY">
	///     The vertical scroll amount, negative or positive depending on the direction the wheel was
	///     scrolled.
	/// </param>
	/// <returns>The input was processed.</returns>
	public bool Scrolled(float amountX, float amountY);

	/// <summary>
	///     Called when the touch gesture is cancelled.
	/// </summary>
	/// <remarks>
	///     Relevant on Android and iOS only.
	/// </remarks>
	/// <param name="screenX">The x coordinate, origin is in the upper left corner.</param>
	/// <param name="screenY">The y coordinate, origin is in the upper left corner.</param>
	/// <param name="pointer">The pointer for the event.</param>
	/// <param name="button">The button.</param>
	/// <returns>Whether the input was processed.</returns>
	public bool TouchCancelled(int screenX, int screenY, int pointer, int button);

	/// <summary>
	///     Called when the screen was touched or a mouse button was pressed.
	/// </summary>
	/// <param name="screenX">The x coordinate, origin is in the upper left corner.</param>
	/// <param name="screenY">The y coordinate, origin is in the upper left corner.</param>
	/// <param name="pointer">The pointer for the event.</param>
	/// <param name="button">The button.</param>
	/// <returns>Whether the input was processed.</returns>
	public bool TouchDown(int screenX, int screenY, int pointer, int button);

	/// <summary>
	///     Called when a finger or the mouse was dragged.
	/// </summary>
	/// <param name="screenX">The x coordinate, origin is in the upper left corner.</param>
	/// <param name="screenY">The y coordinate, origin is in the upper left corner.</param>
	/// <param name="pointer">The pointer for the event.</param>
	/// <returns>Whether the input was processed.</returns>
	public bool TouchDragged(int screenX, int screenY, int pointer);

	/// <summary>
	///     Called when a finger was lifted or a mouse button was released.
	/// </summary>
	/// <param name="screenX">The x coordinate, origin is in the upper left corner.</param>
	/// <param name="screenY">The y coordinate, origin is in the upper left corner.</param>
	/// <param name="pointer">The pointer for the event.</param>
	/// <param name="button">The button.</param>
	/// <returns>Whether the input was processed.</returns>
	public bool TouchUp(int screenX, int screenY, int pointer, int button);
}