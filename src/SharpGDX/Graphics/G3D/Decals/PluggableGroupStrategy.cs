using SharpGDX.Graphics.GLUtils;
using SharpGDX.Utils;

namespace SharpGDX.Graphics.G3D.Decals;

/** This class in combination with the {@link GroupPlug GroupPlugs} allows you to build a modular {@link GroupStrategy} out of
 * routines you already implemented. */
public abstract class PluggableGroupStrategy : GroupStrategy {
	private IntMap<GroupPlug> plugs = new IntMap<GroupPlug>();

    public abstract ShaderProgram getGroupShader(int group);
    public abstract int decideGroup(Decal decal);

    public void beforeGroup (int group, Array<Decal> contents) {
		plugs.get(group).beforeGroup(contents);
	}

	public void afterGroup (int group) {
		plugs.get(group).afterGroup();
	}

    public abstract void beforeGroups();
    public abstract void afterGroups();

    /** Set the plug used for a specific group. The plug will automatically be invoked.
	 * @param plug Plug to use
	 * @param group Group the plug is for */
	public void plugIn (GroupPlug plug, int group) {
		plugs.put(group, plug);
	}

	/** Remove a plug from the strategy
	 * @param group Group to remove the plug from
	 * @return removed plug, null if there was none for that group */
	public GroupPlug unPlug (int group) {
		return plugs.remove(group);
	}
}
