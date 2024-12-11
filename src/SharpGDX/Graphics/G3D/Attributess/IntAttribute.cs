namespace SharpGDX.Graphics.G3D.Attributess;

public class IntAttribute : Attribute {
	public static readonly String CullFaceAlias = "cullface";
	public static readonly long CullFace = register(CullFaceAlias);

	/** create a cull face attribute to be used in a material
	 * @param value cull face value, possible values are 0 (render both faces), GL_FRONT_AND_BACK (render nothing), GL_BACK (render
	 *           front faces only), GL_FRONT (render back faces only), or -1 to inherit default
	 * @return an attribute */
	public static IntAttribute createCullFace (int value) {
		return new IntAttribute(CullFace, value);
	}

	public int value;

	public IntAttribute (long type) 
    : base(type)
    {
		
	}

	public IntAttribute (long type, int value) 
    : base(type)
    {
		
		this.value = value;
	}

	public override Attribute copy () {
		return new IntAttribute(type, value);
	}

	public override int GetHashCode () {
		int result = base.GetHashCode();
		result = 983 * result + value;
		return result;
	}

	public override int CompareTo (Attribute o) {
		if (type != o.type) return (int)(type - o.type);
		return value - ((IntAttribute)o).value;
	}
}
