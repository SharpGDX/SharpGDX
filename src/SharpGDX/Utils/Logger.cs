using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Utils
{
	/** Simple logger that uses the {@link Application} logging facilities to output messages. The log level set with
 * {@link Application#setLogLevel(int)} overrides the log level set here.
 * @author mzechner
 * @author Nathan Sweet */
	public class Logger
	{
		static public readonly int NONE = 0;
		static public readonly int ERROR = 1;
		static public readonly int INFO = 2;
		static public readonly int DEBUG = 3;

		private readonly String tag;
		private int level;

		public Logger(String tag)
		: this(tag, ERROR)
		{
			
		}

		public Logger(String tag, int level)
		{
			this.tag = tag;
			this.level = level;
		}

		public void debug(String message)
		{
			if (level >= DEBUG) Gdx.app.debug(tag, message);
		}

		public void debug(String message, Exception exception)
		{
			if (level >= DEBUG) Gdx.app.debug(tag, message, exception);
		}

		public void info(String message)
		{
			if (level >= INFO) Gdx.app.log(tag, message);
		}

		public void info(String message, Exception exception)
		{
			if (level >= INFO) Gdx.app.log(tag, message, exception);
		}

		public void error(String message)
		{
			if (level >= ERROR) Gdx.app.error(tag, message);
		}

		public void error(String message, Exception exception)
		{
			if (level >= ERROR) Gdx.app.error(tag, message, exception);
		}

		/** Sets the log level. {@link #NONE} will mute all log output. {@link #ERROR} will only let error messages through.
		 * {@link #INFO} will let all non-debug messages through, and {@link #DEBUG} will let all messages through.
		 * @param level {@link #NONE}, {@link #ERROR}, {@link #INFO}, {@link #DEBUG}. */
		public void setLevel(int level)
		{
			this.level = level;
		}

		public int getLevel()
		{
			return level;
		}
	}
}
