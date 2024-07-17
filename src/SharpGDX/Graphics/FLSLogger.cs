using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Utils;

namespace SharpGDX.Graphics
{
	/** A simple helper class to log the frames per seconds achieved. Just invoke the {@link #log()} method in your rendering method.
 * The output will be logged once per second.
 *
 * @author mzechner */
	public class FPSLogger
	{
		long startTime;
		int bound;

		public FPSLogger()
		: this(int.MaxValue)
		{
			
		}

		/** @param bound only logs when they frames per second are less than the bound */
		public FPSLogger(int bound)
		{
			this.bound = bound;
			startTime = TimeUtils.nanoTime();
		}

		/** Logs the current frames per second to the console. */
		public void log()
		{
			 long nanoTime = TimeUtils.nanoTime();
			if (nanoTime - startTime > 1000000000) /* 1,000,000,000ns == one second */
			{
				 int fps = Gdx.graphics.getFramesPerSecond();
				if (fps < bound)
				{
					Gdx.app.log("FPSLogger", "fps: " + fps);
					startTime = nanoTime;
				}
			}
		}
	}
}
