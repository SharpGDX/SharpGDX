using System;
using SharpGDX.Utils;
using SharpGDX.Shims;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using  static SharpGDX.Graphics.Profiling.GLInterceptor;

namespace SharpGDX.Graphics.Profiling
{
	/** Listener for GL errors detected by {@link GLProfiler}.
 * 
 * @see GLProfiler
 * @author Jan Polák */
public interface IGLErrorListener {

	/** Put your error logging code here.
	 * @see GLInterceptor#resolveErrorNumber(int) */
	public void onError (int error);

	// Basic implementations

	/** Listener that will log using Gdx.app.error GL error name and GL function. */
	public static IGLErrorListener LOGGING_LISTENER = new LoggingListener() ;

	private class LoggingListener:IGLErrorListener
	{
		public void onError(int error)
		{
			String place = null;
			try
			{
				throw new NotImplementedException();
				//StackTraceElement[] stack = Thread.currentThread().getStackTrace();
				//for (int i = 0; i < stack.length; i++)
				//{
				//	if ("check".equals(stack[i].getMethodName()))
				//	{
				//		if (i + 1 < stack.length)
				//		{
				//			final StackTraceElement glMethod = stack[i + 1];
				//			place = glMethod.getMethodName();
				//		}
				//		break;
				//	}
				//}
			}
			catch (Exception ignored)
			{
			}

			if (place != null)
			{
				Gdx.app.error("GLProfiler", "Error " + resolveErrorNumber(error) + " from " + place);
			}
			else
			{
				Gdx.app.error("GLProfiler", "Error " + resolveErrorNumber(error) + " at: ", new Exception());
				// This will capture current stack trace for logging, if possible
			}
		}
	}

		/** Listener that will throw a GdxRuntimeException with error name. */
		public static  IGLErrorListener THROWING_LISTENER = new ThrowingListener() ;

		private class ThrowingListener:IGLErrorListener
		{
			public void onError(int error)
			{
				throw new GdxRuntimeException("GLProfiler: Got GL error " + resolveErrorNumber(error));
			}
		}
	}
}
