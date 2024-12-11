using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX.Mathematics;
using Buffer = SharpGDX.Shims.Buffer;
using static SharpGDX.Utils.Timer;
using System.Numerics;

namespace SharpGDX.Graphics.G3D;

/** Extend this class to implement a material attribute. Register the attribute type by statically calling the
 * {@link #register(String)} method, whose return value should be used to instantiate the attribute. A class can implement
 * multiple types
 * @author Xoppa */
public abstract class Attribute : IComparable<Attribute> {
	/** The registered type aliases */
	private readonly static Array<String> types = new Array<String>();

	/** The long bitmask is limited to 64 bits **/
	private readonly static int MAX_ATTRIBUTE_COUNT = 64;

	/** @return The ID of the specified attribute type, or zero if not available */
	public static long getAttributeType (String alias) {
		for (int i = 0; i < types.size; i++)
			if (types.Get(i).CompareTo(alias) == 0) return 1L << i;
		return 0;
	}

	/** @return The alias of the specified attribute type, or null if not available. */
	public static String getAttributeAlias (long type) {
		int idx = -1;
		while (type != 0 && ++idx < 63 && (((type >> idx) & 1) == 0))
			;
		return (idx >= 0 && idx < types.size) ? types.Get(idx) : null;
	}

	/** Call this method to register a custom attribute type, see the wiki for an example. If the alias already exists, then that
	 * ID will be reused. The alias should be unambiguously and will by default be returned by the call to {@link #toString()}. A
	 * maximum of 64 attributes can be registered as a long bitmask can only hold 64 bits.
	 * @param alias The alias of the type to register, must be different for each direct type, will be used for debugging
	 * @return the ID of the newly registered type, or the ID of the existing type if the alias was already registered
	 * @throws GdxRuntimeException if maximum attribute count reached */
	protected static long register (String alias) {
		long result = getAttributeType(alias);
		if (result > 0) return result;
		if (types.size >= MAX_ATTRIBUTE_COUNT) {
			throw new GdxRuntimeException("Cannot register " + alias + ", maximum registered attribute count reached.");
		}
		types.Add(alias);
		return 1L << (types.size - 1);
	}

	/** The type of this attribute */
	public readonly long type;

	private readonly int typeBit;
    protected Attribute (long type) {
        this.type = type;
        // TODO: Verify parity of Integer.numberOfTrailingZeros -RP
this.typeBit = BitOperations.TrailingZeroCount((ulong)type);
	}

	/** @return An exact copy of this attribute */
	public abstract Attribute copy ();

	protected bool Equals (Attribute other) {
		return other.GetHashCode() == GetHashCode();
	}

    public abstract int CompareTo(Attribute? other);

    public override bool Equals (Object? obj) {
		if (obj == null) return false;
		if (obj == this) return true;
		if (!(obj is Attribute)) return false;
		Attribute other = (Attribute)obj;
		if (this.type != other.type) return false;
		return Equals(other);
	}

	public override String ToString () {
		return getAttributeAlias(type);
	}

	public override int GetHashCode () {
		return 7489 * typeBit;
	}
}
