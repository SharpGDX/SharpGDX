using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Shims
{
	/**
 * Native bindings to the <a href="https://openal-soft.org/openal-extensions/SOFTX_hold_on_disconnect.txt">SOFTX_hold_on_disconnect</a> extension.
 *
 * <p>LWJGL: This extension is experimental.</p>
 */
	public sealed class SOFTXHoldOnDisconnect
	{

		/** Accepted by the {@code target} parameter of {@link AL10#alEnable Enable}, {@link AL10#alDisable Disable}, and {@link AL10#alIsEnabled IsEnabled}. */
		public static readonly int AL_STOP_SOURCES_ON_DISCONNECT_SOFT = 0x19AB;

		private SOFTXHoldOnDisconnect() { }

	}
}
