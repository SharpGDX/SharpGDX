using SharpGDX.Graphics;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Mathematics;
using SharpGDX.Mathematics.Collision;
using SharpGDX.Scenes.Scene2D.Utils;

namespace SharpGDX.Utils.Viewports;

/// <summary>
///     Manages a <see cref="Camera" /> and determines how world coordinates are mapped to and from the screen.
/// </summary>
public abstract class Viewport
{
    private readonly Vector3 _tmp = new();
    private Camera _camera = null!;
    private int _screenX, _screenY, _screenWidth, _screenHeight;
    private float _worldWidth, _worldHeight;

    /// <summary>
    ///     Calls <see cref="Apply(bool)" /> with false.
    /// </summary>
    public void Apply()
    {
        Apply(false);
    }

    /// <summary>
    ///     Applies the viewport to the camera and sets the glViewport.
    /// </summary>
    /// <param name="centerCamera">If true, the camera position is set to the center of the world.</param>
    public void Apply(bool centerCamera)
    {
        HdpiUtils.glViewport(_screenX, _screenY, _screenWidth, _screenHeight);
        _camera.viewportWidth = _worldWidth;
        _camera.viewportHeight = _worldHeight;
        if (centerCamera)
        {
            _camera.position.set(_worldWidth / 2, _worldHeight / 2, 0);
        }

        _camera.update();
    }

    /// <inheritdoc cref="ScissorStack.calculateScissors(Camera, float, float, float, float, Matrix4, Rectangle, Rectangle)" />
    public void CalculateScissors(Matrix4 batchTransform, Rectangle area, Rectangle scissor)
    {
        ScissorStack.calculateScissors(_camera, _screenX, _screenY, _screenWidth, _screenHeight, batchTransform,
            area, scissor);
    }

    /// <summary>
    ///     Returns the bottom gutter (black bar) height in screen coordinates.
    /// </summary>
    /// <returns></returns>
    public int GetBottomGutterHeight()
    {
        return _screenY;
    }

    public Camera GetCamera()
    {
        return _camera;
    }

    /// <summary>
    ///     Returns the left gutter (black bar) width in screen coordinates.
    /// </summary>
    /// <returns></returns>
    public int GetLeftGutterWidth()
    {
        return _screenX;
    }

    // TODO: Should this inherit doc?
    /// <inheritdoc cref="Camera.getPickRay(float, float, float, float, float, float)" />
    public Ray GetPickRay(float screenX, float screenY)
    {
        return _camera.getPickRay(screenX, screenY, _screenX, _screenY, _screenWidth, _screenHeight);
    }

    /// <summary>
    ///     Returns the right gutter (black bar) width in screen coordinates.
    /// </summary>
    /// <returns></returns>
    public int GetRightGutterWidth()
    {
        return Gdx.Graphics.getWidth() - (_screenX + _screenWidth);
    }

    /// <summary>
    ///     Returns the right gutter (black bar) x in screen coordinates.
    /// </summary>
    /// <returns></returns>
    public int GetRightGutterX()
    {
        return _screenX + _screenWidth;
    }

    public int GetScreenHeight()
    {
        return _screenHeight;
    }

    public int GetScreenWidth()
    {
        return _screenWidth;
    }

    public int GetScreenX()
    {
        return _screenX;
    }

    public int GetScreenY()
    {
        return _screenY;
    }

    /// <summary>
    ///     Returns the top gutter (black bar) height in screen coordinates.
    /// </summary>
    /// <returns></returns>
    public int GetTopGutterHeight()
    {
        return Gdx.Graphics.getHeight() - (_screenY + _screenHeight);
    }

    /// <summary>
    ///     Returns the top gutter (black bar) y in screen coordinates.
    /// </summary>
    /// <returns></returns>
    public int GetTopGutterY()
    {
        return _screenY + _screenHeight;
    }

    public float GetWorldHeight()
    {
        return _worldHeight;
    }

    public float GetWorldWidth()
    {
        return _worldWidth;
    }

    /// <summary>
    ///     Transforms the specified world coordinate to screen coordinates.
    /// </summary>
    /// <remarks>
    ///     <see cref="Camera.project(Vector3)" />
    /// </remarks>
    /// <param name="worldCoords"></param>
    /// <returns>The vector that was passed in, transformed to screen coordinates.</returns>
    public Vector2 Project(Vector2 worldCoords)
    {
        _tmp.set(worldCoords.x, worldCoords.y, 1);
        _camera.project(_tmp, _screenX, _screenY, _screenWidth, _screenHeight);
        worldCoords.Set(_tmp.x, _tmp.y);
        return worldCoords;
    }

    /// <summary>
    ///     Transforms the specified world coordinate to screen coordinates.
    /// </summary>
    /// <see cref="Camera.project(Vector3)" />
    /// <param name="worldCoords"></param>
    /// <returns>The vector that was passed in, transformed to screen coordinates.</returns>
    public Vector3 Project(Vector3 worldCoords)
    {
        _camera.project(worldCoords, _screenX, _screenY, _screenWidth, _screenHeight);
        return worldCoords;
    }

    public void SetCamera(Camera camera)
    {
        _camera = camera;
    }

    /// <summary>
    ///     Sets the viewport's bounds in screen coordinates.
    /// </summary>
    /// <remarks>
    ///     This is typically set by <see cref="Update(int, int, bool)" />.
    /// </remarks>
    /// <param name="screenX"></param>
    /// <param name="screenY"></param>
    /// <param name="screenWidth"></param>
    /// <param name="screenHeight"></param>
    public void SetScreenBounds(int screenX, int screenY, int screenWidth, int screenHeight)
    {
        _screenX = screenX;
        _screenY = screenY;
        _screenWidth = screenWidth;
        _screenHeight = screenHeight;
    }

    /// <summary>
    ///     Sets the viewport's height in screen coordinates.
    /// </summary>
    /// <remarks>
    ///     This is typically set by <see cref="Update(int, int, bool)" />.
    /// </remarks>
    /// <param name="screenHeight"></param>
    public void SetScreenHeight(int screenHeight)
    {
        _screenHeight = screenHeight;
    }

    /// <summary>
    ///     Sets the viewport's position in screen coordinates.
    /// </summary>
    /// <remarks>
    ///     This is typically set by <see cref="Update(int, int, bool)" />.
    /// </remarks>
    /// <param name="screenX"></param>
    /// <param name="screenY"></param>
    public void SetScreenPosition(int screenX, int screenY)
    {
        _screenX = screenX;
        _screenY = screenY;
    }

    /// <summary>
    ///     Sets the viewport's size in screen coordinates.
    /// </summary>
    /// <remarks>
    ///     This is typically set by <see cref="Update(int, int, bool)" />.
    /// </remarks>
    /// <param name="screenWidth"></param>
    /// <param name="screenHeight"></param>
    public void SetScreenSize(int screenWidth, int screenHeight)
    {
        _screenWidth = screenWidth;
        _screenHeight = screenHeight;
    }

    /// <summary>
    ///     Sets the viewport's width in screen coordinates.
    /// </summary>
    /// <remarks>
    ///     This is typically set by <see cref="Update(int, int, bool)" />.
    /// </remarks>
    /// <param name="screenWidth"></param>
    public void SetScreenWidth(int screenWidth)
    {
        _screenWidth = screenWidth;
    }

    /// <summary>
    ///     Sets the viewport's offset from the left edge of the screen.
    /// </summary>
    /// <remarks>
    ///     This is typically set by <see cref="Update(int, int, bool)" />.
    /// </remarks>
    /// <param name="screenX"></param>
    public void SetScreenX(int screenX)
    {
        _screenX = screenX;
    }

    /// <summary>
    ///     Sets the viewport's offset from the bottom edge of the screen.
    /// </summary>
    /// <remarks>
    ///     This is typically set by <see cref="Update(int, int, bool)" />.
    /// </remarks>
    /// <param name="screenY"></param>
    public void SetScreenY(int screenY)
    {
        _screenY = screenY;
    }

    /// <summary>
    ///     The virtual height of this viewport in world coordinates.
    /// </summary>
    /// <remarks>
    ///     This height is scaled to the viewport's screen height.
    /// </remarks>
    /// <param name="worldHeight"></param>
    public void SetWorldHeight(float worldHeight)
    {
        _worldHeight = worldHeight;
    }

    public void SetWorldSize(float worldWidth, float worldHeight)
    {
        _worldWidth = worldWidth;
        _worldHeight = worldHeight;
    }

    /// <summary>
    ///     The virtual width of this viewport in world coordinates.
    /// </summary>
    /// <remarks>
    ///     This width is scaled to the viewport's screen width.
    /// </remarks>
    /// <param name="worldWidth"></param>
    public void SetWorldWidth(float worldWidth)
    {
        _worldWidth = worldWidth;
    }

    /// <summary>
    ///     Transforms a point to real screen coordinates (as opposed to OpenGL ES window coordinates), where the origin is in
    ///     the top left and the y-axis is pointing downwards.
    /// </summary>
    /// <param name="worldCoords"></param>
    /// <param name="transformMatrix"></param>
    /// <returns></returns>
    public Vector2 ToScreenCoordinates(Vector2 worldCoords, Matrix4 transformMatrix)
    {
        _tmp.set(worldCoords.x, worldCoords.y, 0);
        _tmp.mul(transformMatrix);
        _camera.project(_tmp, _screenX, _screenY, _screenWidth, _screenHeight);
        _tmp.y = Gdx.Graphics.getHeight() - _tmp.y;
        worldCoords.x = _tmp.x;
        worldCoords.y = _tmp.y;
        return worldCoords;
    }

    /// <summary>
    ///     Transforms the specified screen coordinate to world coordinates.
    /// </summary>
    /// <remarks>
    ///     <see cref="Camera.unproject(Vector3)" />
    /// </remarks>
    /// <param name="screenCoords"></param>
    /// <returns>The vector that was passed in, transformed to world coordinates.</returns>
    public Vector2 Unproject(Vector2 screenCoords)
    {
        _tmp.set(screenCoords.x, screenCoords.y, 1);
        _camera.unproject(_tmp, _screenX, _screenY, _screenWidth, _screenHeight);
        screenCoords.Set(_tmp.x, _tmp.y);
        return screenCoords;
    }

    /// <summary>
    ///     Transforms the specified screen coordinate to world coordinates.
    /// </summary>
    /// <remarks>
    ///     <see cref="Camera.unproject(Vector3)" />
    /// </remarks>
    /// <param name="screenCoords"></param>
    /// <returns>The vector that was passed in, transformed to world coordinates.</returns>
    public Vector3 Unproject(Vector3 screenCoords)
    {
        _camera.unproject(screenCoords, _screenX, _screenY, _screenWidth, _screenHeight);
        return screenCoords;
    }

    /// <summary>
    ///     Calls <see cref="Update(int, int, bool)" /> with false.
    /// </summary>
    /// <param name="screenWidth"></param>
    /// <param name="screenHeight"></param>
    public void Update(int screenWidth, int screenHeight)
    {
        Update(screenWidth, screenHeight, false);
    }

    /// <summary>
    ///     Configures this viewport's screen bounds using the specified screen size and calls <see cref="Apply(bool)" />.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Typically called from <see cref="IApplicationListener.Resize(int, int)" /> or
    ///         <see cref="IScreen.Resize(int, int)" />.
    ///     </para>
    ///     <para>
    ///         The default implementation only calls <see cref="Apply(bool)" />.
    ///     </para>
    /// </remarks>
    /// <param name="screenWidth"></param>
    /// <param name="screenHeight"></param>
    /// <param name="centerCamera"></param>
    public virtual void Update(int screenWidth, int screenHeight, bool centerCamera)
    {
        Apply(centerCamera);
    }
}