using SharpGDX.Graphics;
using SharpGDX.Mathematics;

namespace SharpGDX.Graphics.Profiling
{
	/** When enabled, collects statistics about GL calls and checks for GL errors. Enabling will wrap Gdx.gl* instances with delegate
 * classes which provide described functionality and route GL calls to the actual GL instances.
 * 
 * @see GL20Interceptor
 * @see GL30Interceptor
 * 
 * @author Daniel Holderbaum
 * @author Jan Polák */
public class GLProfiler {

	private IGraphics graphics;
	private GLInterceptor glInterceptor;
	private IGLErrorListener listener;
	private bool enabled = false;

		/** Create a new instance of GLProfiler to monitor a {@link com.badlogic.gdx.Graphics} instance's gl calls
		 * @param graphics instance to monitor with this instance, With Lwjgl 2.x you can pass in Gdx.graphics, with Desktop use
		 *           DesktopWindow.getGraphics() */
		public GLProfiler (IGraphics graphics) {
		this.graphics = graphics;
		GL32 gl32 = graphics.getGL32();
		GL31 gl31 = graphics.getGL31();
		GL30 gl30 = graphics.getGL30();
		if (gl32 != null) {
			glInterceptor = new GL32Interceptor(this, gl32);
		} else if (gl31 != null) {
			glInterceptor = new GL31Interceptor(this, gl31);
		} else if (gl30 != null) {
			glInterceptor = new GL30Interceptor(this, gl30);
		} else {
			glInterceptor = new GL20Interceptor(this, graphics.getGL20());
		}
		listener = IGLErrorListener.LOGGING_LISTENER;
	}

	/** Enables profiling by replacing the {@code GL20} and {@code GL30} instances with profiling ones. */
	public void enable () {
		if (enabled) return;

		if (glInterceptor is GL32) {
			graphics.setGL32((GL32)glInterceptor);
		}
		if (glInterceptor is GL31) {
			graphics.setGL31((GL31)glInterceptor);
		}
		if (glInterceptor is GL30) {
			graphics.setGL30((GL30)glInterceptor);
		}
		graphics.setGL20(glInterceptor);

		Gdx.gl32 = graphics.getGL32();
		Gdx.gl31 = graphics.getGL31();
		Gdx.gl30 = graphics.getGL30();
		Gdx.gl20 = graphics.getGL20();
		Gdx.gl = graphics.getGL20();

		enabled = true;
	}

	/** Disables profiling by resetting the {@code GL20} and {@code GL30} instances with the original ones. */
	public void disable () {
		if (!enabled) return;

		if (glInterceptor is GL32Interceptor) {
			graphics.setGL32(((GL32Interceptor)glInterceptor).gl32);
		}
		if (glInterceptor is GL31Interceptor) {
			graphics.setGL31(((GL31Interceptor)glInterceptor).gl31);
		}
		if (glInterceptor is GL30Interceptor) {
			graphics.setGL30(((GL30Interceptor)glInterceptor).gl30);
		}
		if (glInterceptor is GL20Interceptor) {
			graphics.setGL20(((GL20Interceptor)graphics.getGL20()).gl20);
		}

		Gdx.gl32 = graphics.getGL32();
		Gdx.gl31 = graphics.getGL31();
		Gdx.gl30 = graphics.getGL30();
		Gdx.gl20 = graphics.getGL20();
		Gdx.gl = graphics.getGL20();

		enabled = false;
	}

	/** Set the current listener for the {@link GLProfiler} to {@code errorListener} */
	public void setListener (IGLErrorListener errorListener) {
		this.listener = errorListener;
	}

	/** @return the current {@link GLErrorListener} */
	public IGLErrorListener getListener () {
		return listener;
	}

	/** @return true if the GLProfiler is currently profiling */
	public bool isEnabled () {
		return enabled;
	}

	/** @return the total gl calls made since the last reset */
	public int getCalls () {
		return glInterceptor.getCalls();
	}

	/** @return the total amount of texture bindings made since the last reset */
	public int getTextureBindings () {
		return glInterceptor.getTextureBindings();
	}

	/** @return the total amount of draw calls made since the last reset */
	public int getDrawCalls () {
		return glInterceptor.getDrawCalls();
	}

	/** @return the total amount of shader switches made since the last reset */
	public int getShaderSwitches () {
		return glInterceptor.getShaderSwitches();
	}

	/** @return {@link FloatCounter} containing information about rendered vertices since the last reset */
	public FloatCounter getVertexCount () {
		return glInterceptor.getVertexCount();
	}

	/** Will reset the statistical information which has been collected so far. This should be called after every frame. Error
	 * listener is kept as it is. */
	public void reset () {
		glInterceptor.reset();
	}

}
}
