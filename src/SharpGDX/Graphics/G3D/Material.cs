using SharpGDX.Utils;

namespace SharpGDX.Graphics.G3D;

public class Material : Attributes {
	private static int counter = 0;

	public String id;

	/** Create an empty material */
	public Material () 
    : this("mtl" + (++counter))
    {
		
	}

	/** Create an empty material */
	public Material (String id) {
		this.id = id;
	}

	/** Create a material with the specified attributes */
	public Material (params Attribute[] attributes) 
    : this()
    {
		
		set(attributes);
	}

	/** Create a material with the specified attributes */
	public Material ( String id,  Attribute[] attributes) 
    : this(id)
    {
		
		set(attributes);
	}

	/** Create a material with the specified attributes */
	public Material ( Array<Attribute> attributes) 
    : this()
    {
		
		set(attributes);
	}

	/** Create a material with the specified attributes */
	public Material ( String id,  Array<Attribute> attributes) 
    : this(id)
    {
		
		set(attributes);
	}

	/** Create a material which is an exact copy of the specified material */
	public Material ( Material copyFrom) 
    : this(copyFrom.id, copyFrom)
    {
		
	}

	/** Create a material which is an exact copy of the specified material */
	public Material ( String id,  Material copyFrom) 
    : this(id)
    {
		
		foreach (Attribute attr in copyFrom)
			set(attr.copy());
	}

	/** Create a copy of this material */
	public Material copy () {
		return new Material(this);
	}

	public override int GetHashCode () {
		return base.GetHashCode() + 3 * id.GetHashCode();
	}

	public override bool Equals (Object? other) {
		return (other is Material) && ((other == this) || ((((Material)other).id.Equals(id)) && base.Equals(other)));
	}
}
