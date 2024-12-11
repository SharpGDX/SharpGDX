using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX.Mathematics;
using Buffer = SharpGDX.Shims.Buffer;

namespace SharpGDX.Graphics.G3D;

// TODO: Rename to AttributeCollection
public class Attributes : IEnumerable<Attribute>, IComparer<Attribute>, IComparable<Attributes> {
	protected long mask;
	protected readonly Array<Attribute> attributes = new Array<Attribute>();

	protected bool sorted = true;

	/** Sort the attributes by their ID */
	public void sort () {
		if (!sorted) {
			attributes.sort(this);
			sorted = true;
		}
	}

	/** @return Bitwise mask of the ID's of all the containing attributes */
	public long getMask () {
		return mask;
	}

	/** Example usage: ((BlendingAttribute)material.get(BlendingAttribute.ID)).sourceFunction;
	 * @return The attribute (which can safely be cast) if any, otherwise null */
	public Attribute get (long type) {
		if (has(type)) for (int i = 0; i < attributes.size; i++)
			if (attributes.Get(i).type == type) return attributes.Get(i);
		return null;
	}

	/** Example usage: ((BlendingAttribute)material.get(BlendingAttribute.ID)).sourceFunction;
	 * @return The attribute if any, otherwise null */
	public T get<T> (Type clazz, long type)
	where T: Attribute{
		return (T)get(type);
	}

	/** Get multiple attributes at once. Example: material.get(out, ColorAttribute.Diffuse | ColorAttribute.Specular |
	 * TextureAttribute.Diffuse); */
	public Array<Attribute> get (Array<Attribute> @out, long type) {
		for (int i = 0; i < attributes.size; i++)
			if ((attributes.Get(i).type & type) != 0) @out.Add(attributes.Get(i));
		return @out;
	}

	/** Removes all attributes */
	public void clear () {
		mask = 0;
		attributes.clear();
	}

	/** @return The amount of attributes this material contains. */
	public int size () {
		return attributes.size;
	}

	private void enable (long mask) {
		this.mask |= mask;
	}

	private void disable (long mask) {
		this.mask &= ~mask;
	}

	/** Add a attribute to this material. If the material already contains an attribute of the same type it is overwritten. */
	public void set (Attribute attribute) {
		int idx = indexOf(attribute.type);
		if (idx < 0) {
			enable(attribute.type);
			attributes.Add(attribute);
			sorted = false;
		} else {
			attributes.set(idx, attribute);
		}
		sort(); // FIXME: See #4186
	}

	/** Add multiple attributes to this material. If the material already contains an attribute of the same type it is
	 * overwritten. */
	public void set (Attribute attribute1, Attribute attribute2) {
		set(attribute1);
		set(attribute2);
	}

	/** Add multiple attributes to this material. If the material already contains an attribute of the same type it is
	 * overwritten. */
	public void set (Attribute attribute1, Attribute attribute2, Attribute attribute3) {
		set(attribute1);
		set(attribute2);
		set(attribute3);
	}

	/** Add multiple attributes to this material. If the material already contains an attribute of the same type it is
	 * overwritten. */
	public void set (Attribute attribute1, Attribute attribute2, Attribute attribute3,
		Attribute attribute4) {
		set(attribute1);
		set(attribute2);
		set(attribute3);
		set(attribute4);
	}

	/** Add an array of attributes to this material. If the material already contains an attribute of the same type it is
	 * overwritten. */
	public void set (Attribute[] attributes) {
		foreach (Attribute attr in attributes)
			set(attr);
	}

	/** Add an array of attributes to this material. If the material already contains an attribute of the same type it is
	 * overwritten. */
	public void set (IEnumerable<Attribute> attributes) {
		foreach (Attribute attr in attributes)
			set(attr);
	}

	/** Removes the attribute from the material, i.e.: material.remove(BlendingAttribute.ID); Can also be used to remove multiple
	 * attributes also, i.e. remove(AttributeA.ID | AttributeB.ID); */
	public void remove (long mask) {
		for (int i = attributes.size - 1; i >= 0; i--) {
			long type = attributes.Get(i).type;
			if ((mask & type) == type) {
				attributes.RemoveIndex(i);
				disable(type);
				sorted = false;
			}
		}
		sort(); // FIXME: See #4186
	}

	/** @return True if this collection has the specified attribute, i.e. attributes.has(ColorAttribute.Diffuse); Or when multiple
	 *         attribute types are specified, true if this collection has all specified attributes, i.e. attributes.has(out,
	 *         ColorAttribute.Diffuse | ColorAttribute.Specular | TextureAttribute.Diffuse); */
	public bool has (long type) {
		return type != 0 && (this.mask & type) == type;
	}

	/** @return the index of the attribute with the specified type or negative if not available. */
	protected int indexOf (long type) {
		if (has(type)) for (int i = 0; i < attributes.size; i++)
			if (attributes.Get(i).type == type) return i;
		return -1;
	}

	/** Check if this collection has the same attributes as the other collection. If compareValues is true, it also compares the
	 * values of each attribute.
	 * @param compareValues True to compare attribute values, false to only compare attribute types
	 * @return True if this collection contains the same attributes (and optionally attribute values) as the other. */
	public bool same (Attributes other, bool compareValues) {
		if (other == this) return true;
		if ((other == null) || (mask != other.mask)) return false;
		if (!compareValues) return true;
		sort();
		other.sort();
		for (int i = 0; i < attributes.size; i++)
			if (!attributes.Get(i).Equals(other.attributes.Get(i))) return false;
		return true;
	}

	/** See {@link #same(Attributes, boolean)}
	 * @return True if this collection contains the same attributes (but not values) as the other. */
	public bool same (Attributes other) {
		return same(other, false);
	}

	/** Used for sorting attributes by type (not by value) */
	public int Compare (Attribute arg0, Attribute arg1) {
		return (int)(arg0.type - arg1.type);
	}

	/** Used for iterating through the attributes */
	public IEnumerator<Attribute> GetEnumerator() {
		return attributes.GetEnumerator();
	}

	/** @return A hash code based on only the attribute values, which might be different compared to {@link #hashCode()} because
	 *         the latter might include other properties as well, i.e. the material id. */
	public int attributesHash () {
		sort();
		int n = attributes.size;
		long result = 71 + mask;
		int m = 1;
		for (int i = 0; i < n; i++)
			result += mask * attributes.Get(i).GetHashCode() * (m = (m * 7) & 0xFFFF);
		return (int)(result ^ (result >> 32));
	}

	public override int GetHashCode () {
		return attributesHash();
	}

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public override bool Equals (Object? other) {
		if (!(other is Attributes)) return false;
		if (other == this) return true;
		return same((Attributes)other, true);
	}

	public int CompareTo (Attributes? other) {
		if (other == this) return 0;
		if (mask != other.mask) return mask < other.mask ? -1 : 1;
		sort();
		other.sort();
		for (int i = 0; i < attributes.size; i++) {
			int c = attributes.Get(i).CompareTo(other.attributes.Get(i));
			if (c != 0) return c < 0 ? -1 : (c > 0 ? 1 : 0);
		}
		return 0;
	}
}
