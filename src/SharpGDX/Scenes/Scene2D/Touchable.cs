using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Scenes.Scene2D
{
	/** Determines how touch input events are distributed to an actor and any children.
 * @author Nathan Sweet */
	public enum Touchable
	{
		/** All touch input events will be received by the actor and any children. */
		enabled,
		/** No touch input events will be received by the actor or any children. */
		disabled,
		/** No touch input events will be received by the actor, but children will still receive events. Note that events on the
		 * children will still bubble to the parent. */
		childrenOnly
	}
}
