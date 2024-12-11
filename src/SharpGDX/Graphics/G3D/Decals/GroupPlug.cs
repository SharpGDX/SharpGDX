using SharpGDX.Utils;

namespace SharpGDX.Graphics.G3D.Decals;

/** Handles a single group's pre and post render arrangements. Can be plugged into {@link PluggableGroupStrategy} to build modular
 * {@link GroupStrategy GroupStrategies}. */
public interface GroupPlug {
	public void beforeGroup (Array<Decal> contents);

	public void afterGroup ();
}
