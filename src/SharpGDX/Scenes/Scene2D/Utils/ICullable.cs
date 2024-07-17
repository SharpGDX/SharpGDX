using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Mathematics;

namespace SharpGDX.Scenes.Scene2D.Utils
{
	/** Allows a parent to set the area that is visible on a child actor to allow the child to cull when drawing itself. This must
 * only be used for actors that are not rotated or scaled.
 * @author Nathan Sweet */
	public interface ICullable
	{
		/** @param cullingArea The culling area in the child actor's coordinates. */
		public void setCullingArea(Rectangle? cullingArea);
	}
}
