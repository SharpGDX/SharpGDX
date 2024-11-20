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
		IGL32 gl32 = graphics.GetGL32();
		IGL31 gl31 = graphics.GetGL31();
		IGL30 gl30 = graphics.GetGL30();
		if (gl32 != null) {
			glInterceptor = new GL32Interceptor(this, gl32);
		} else if (gl31 != null) {
			glInterceptor = new GL31Interceptor(this, gl31);
		} else if (gl30 != null) {
			glInterceptor = new GL30Interceptor(this, gl30);
		} else {
			glInterceptor = new GL20Interceptor(this, graphics.GetGL20());
		}
		listener = IGLErrorListener.LOGGING_LISTENER;
	}

	/** Enables profiling by replacing the {@code GL20} and {@code GL30} instances with profiling ones. */
	public void enable () {
		if (enabled) return;

		if (glInterceptor is IGL32) {
			graphics.SetGL32((IGL32)glInterceptor);
		}
		if (glInterceptor is IGL31) {
			graphics.SetGL31((IGL31)glInterceptor);
		}
		if (glInterceptor is IGL30) {
			graphics.SetGL30((IGL30)glInterceptor);
		}
		graphics.SetGL20(glInterceptor);

		Gdx.GL32 = graphics.GetGL32();
		Gdx.GL31 = graphics.GetGL31();
		Gdx.GL30 = graphics.GetGL30();
		Gdx.GL20 = graphics.GetGL20();
		Gdx.GL = graphics.GetGL20();

		enabled = true;
	}

	/** Disables profiling by resetting the {@code GL20} and {@code GL30} instances with the original ones. */
	public void disable () {
		if (!enabled) return;

		if (glInterceptor is GL32Interceptor) {
			graphics.SetGL32(((GL32Interceptor)glInterceptor).gl32);
		}
		if (glInterceptor is GL31Interceptor) {
			graphics.SetGL31(((GL31Interceptor)glInterceptor).gl31);
		}
		if (glInterceptor is GL30Interceptor) {
			graphics.SetGL30(((GL30Interceptor)glInterceptor).gl30);
		}
		if (glInterceptor is GL20Interceptor) {
			graphics.SetGL20(((GL20Interceptor)graphics.GetGL20()).gl20);
		}

		Gdx.GL32 = graphics.GetGL32();
		Gdx.GL31 = graphics.GetGL31();
		Gdx.GL30 = graphics.GetGL30();
		Gdx.GL20 = graphics.GetGL20();
		Gdx.GL = graphics.GetGL20();

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
