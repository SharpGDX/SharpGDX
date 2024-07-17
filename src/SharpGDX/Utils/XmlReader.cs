using SharpGDX.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpGDX.Shims;
using SharpGDX.Utils;

namespace SharpGDX.Utils;

/** Lightweight XML parser. Supports a subset of XML features: elements, attributes, text, predefined entities, CDATA, mixed
 * content. Namespaces are parsed as part of the element or attribute name. Prologs and doctypes are ignored. Only 8-bit character
 * encodings are supported. Input is assumed to be well formed.<br>
 * <br>
 * The default behavior is to parse the XML into a DOM. Extends this class and override methods to perform event driven parsing.
 * When this is done, the parse methods will return null.
 * @author Nathan Sweet */
public class XmlReader {
	private readonly Array<Element> elements = new (8);
	private Element root, current;
	private readonly StringBuilder textBuffer = new StringBuilder(64);
	private String entitiesText;

	public Element parse (String xml) {
		char[] data = xml.ToCharArray();
		return parse(data, 0, data.Length);
	}

	public Element parse (Reader reader) {
		try {
			char[] data = new char[1024];
			int offset = 0;
			while (true) {
				int length = reader.read(data, offset, data.Length - offset);
				if (length == -1) break;
				if (length == 0) {
					char[] newData = new char[data.Length * 2];
					Array.Copy(data, 0, newData, 0, data.Length);
					data = newData;
				} else
					offset += length;
			}
			return parse(data, 0, offset);
		} catch (IOException ex) {
			throw new SerializationException(ex);
		} finally {
			StreamUtils.closeQuietly(reader);
		}
	}

	public Element parse (InputStream input) {
		try {
			return parse(new InputStreamReader(input, "UTF-8"));
		} catch (IOException ex) {
			throw new SerializationException(ex);
		} finally {
			StreamUtils.closeQuietly(input);
		}
	}

	public Element parse (FileHandle file) {
		try {
			return parse(file.reader("UTF-8"));
		} catch (Exception ex) {
			throw new SerializationException("Error parsing file: " + file, ex);
		}
	}

	public Element parse (char[] data, int offset, int length) {
		int cs, p = offset, pe = length;

		int s = 0;
		String attributeName = null;
		bool hasBody = false;

		// line 3 "XmlReader.java"
		{
			cs = xml_start;
		}

		// line 7 "XmlReader.java"
		{
			int _klen;
			int _trans = 0;
			int _acts;
			int _nacts;
			int _keys;
			int _goto_targ = 0;


			// TODO: This loop is wild as hell. -RP
			// TODO: Not sure if any of these should be 'falling through'. -RP
		_goto:
		while (true) {
				
				switch (_goto_targ) {
				case 0:
					if (p == pe) {
						_goto_targ = 4;
						goto _goto;
					}
					if (cs == 0) {
						_goto_targ = 5;
						goto _goto;
					}

					break;
				case 1:
					_match:
					do {
						_keys = _xml_key_offsets[cs];
						_trans = _xml_index_offsets[cs];
						_klen = _xml_single_lengths[cs];
						if (_klen > 0) {
							int _lower = _keys;
							int _mid;
							int _upper = _keys + _klen - 1;
							while (true) {
								if (_upper < _lower) break;

								_mid = _lower + ((_upper - _lower) >> 1);
								if (data[p] < _xml_trans_keys[_mid])
									_upper = _mid - 1;
								else if (data[p] > _xml_trans_keys[_mid])
									_lower = _mid + 1;
								else {
									_trans += (_mid - _keys);
									goto _match;
								}
							}
							_keys += _klen;
							_trans += _klen;
						}

						_klen = _xml_range_lengths[cs];
						if (_klen > 0) {
							int _lower = _keys;
							int _mid;
							int _upper = _keys + (_klen << 1) - 2;
							while (true) {
								if (_upper < _lower) break;

								_mid = _lower + (((_upper - _lower) >> 1) & ~1);
								if (data[p] < _xml_trans_keys[_mid])
									_upper = _mid - 2;
								else if (data[p] > _xml_trans_keys[_mid + 1])
									_lower = _mid + 2;
								else {
									_trans += ((_mid - _keys) >> 1);
									goto _match;
								}
							}
							_trans += _klen;
						}
					} while (false);

					_trans = _xml_indicies[_trans];
					cs = _xml_trans_targs[_trans];

					if (_xml_trans_actions[_trans] != 0) {
						_acts = _xml_trans_actions[_trans];
						_nacts = (int)_xml_actions[_acts++];
						while (_nacts-- > 0) {
							switch (_xml_actions[_acts++]) {
							case 0:
							// line 97 "XmlReader.rl"
							{
								s = p;
							}
								break;
							case 1:
							// line 98 "XmlReader.rl"
							{
								char c = data[s];
								if (c == '?' || c == '!') {
									if (data[s + 1] == '[' && //
										data[s + 2] == 'C' && //
										data[s + 3] == 'D' && //
										data[s + 4] == 'A' && //
										data[s + 5] == 'T' && //
										data[s + 6] == 'A' && //
										data[s + 7] == '[') {
										s += 8;
										p = s + 2;
										while (data[p - 2] != ']' || data[p - 1] != ']' || data[p] != '>')
											p++;
										text(new String(data, s, p - s - 2));
									} else if (c == '!' && data[s + 1] == '-' && data[s + 2] == '-') {
										p = s + 3;
										while (data[p] != '-' || data[p + 1] != '-' || data[p + 2] != '>')
											p++;
										p += 2;
									} else
										while (data[p] != '>')
											p++;
									{
										cs = 15;
										_goto_targ = 2;
										if (true) goto _goto;
									}
								}
								hasBody = true;
								open(new String(data, s, p - s));
							}
								break;
							case 2:
							// line 127 "XmlReader.rl"
							{
								hasBody = false;
								close();
								{
									cs = 15;
									_goto_targ = 2;
									if (true) goto _goto;
								}
							}
								break;
							case 3:
							// line 132 "XmlReader.rl"
							{
								close();
								{
									cs = 15;
									_goto_targ = 2;
									if (true) goto _goto;
								}
							}
								break;
							case 4:
							// line 136 "XmlReader.rl"
							{
								if (hasBody) {
									cs = 15;
									_goto_targ = 2;
									if (true) goto _goto;
								}
							}
								break;
							case 5:
							// line 139 "XmlReader.rl"
							{
								attributeName = new String(data, s, p - s);
							}
								break;
							case 6:
							// line 142 "XmlReader.rl"
							{
								int end = p;
								while (end != s) {
									switch (data[end - 1]) {
									case ' ':
									case '\t':
									case '\n':
									case '\r':
										end--;
										continue;
									}
									break;
								}
								int current = s;
								bool entityFound = false;
								while (current != end) {
									if (data[current++] != '&') continue;
									int entityStart = current;
									while (current != end) {
										if (data[current++] != ';') continue;
										textBuffer.Append(data, s, entityStart - s - 1);
										String name = new String(data, entityStart, current - entityStart - 1);
										String value = entity(name);
										textBuffer.Append(value != null ? value : name);
										s = current;
										entityFound = true;
										break;
									}
								}
								if (entityFound) {
									if (s < end) textBuffer.Append(data, s, end - s);
									entitiesText = textBuffer.ToString();
									textBuffer.Length =(0);
								} else
									entitiesText = new String(data, s, end - s);
							}
								break;
							case 7:
							// line 178 "XmlReader.rl"
							{
								attribute(attributeName, entitiesText);
							}
								break;
							case 8:
							// line 181 "XmlReader.rl"
							{
								text(entitiesText);
							}
								break;
							// line 201 "XmlReader.java"
							}
						}
					}

					break;
				case 2:
					if (cs == 0) {
						_goto_targ = 5;
						goto _goto;
					}
					if (++p != pe) {
						_goto_targ = 1;
						goto _goto;
					}

					break;
				case 4:
				case 5:
					break;
				}
				break;
			}
		}

		// line 195 "XmlReader.rl"

		entitiesText = null;

		if (p < pe) {
			int lineNumber = 1;
			for (int i = 0; i < p; i++)
				if (data[i] == '\n') lineNumber++;
			throw new SerializationException(
				"Error parsing XML on line " + lineNumber + " near: " + new String(data, p, Math.Min(32, pe - p)));
		} else if (elements.size != 0) {
			Element element = elements.peek();
			elements.clear();
			throw new SerializationException("Error parsing XML, unclosed element: " + element.getName());
		}
		Element root = this.root;
		this.root = null;
		return root;
	}

	// line 221 "XmlReader.java"
	private static byte[] init__xml_actions_0 () {
		return new byte[] {0, 1, 0, 1, 1, 1, 2, 1, 3, 1, 4, 1, 5, 2, 1, 4, 2, 2, 4, 2, 6, 7, 2, 6, 8, 3, 0, 6, 7};
	}

	private static readonly byte[] _xml_actions = init__xml_actions_0();

	private static byte[] init__xml_key_offsets_0 () {
		return new byte[] {0, 0, 4, 9, 14, 20, 26, 30, 35, 36, 37, 42, 46, 50, 51, 52, 56, 57, 62, 67, 73, 79, 83, 88, 89, 90, 95,
			99, 103, 104, 108, 109, 110, 111, 112, 115};
	}

	private static readonly byte[] _xml_key_offsets = init__xml_key_offsets_0();

	// TODO: Holy crap, what is this? -RP
	private static char[] init__xml_trans_keys_0 () {
		return new char[] {(char)32,(char) 60, (char)9,(char) 13, (char)32,(char) 47, (char)62, (char)9,(char) 13,(char) 32, (char)47, (char)62, (char)9, (char)13, (char)32,(char) 47,(char) 61,(char) 62, (char)9,(char) 13,(char) 32, (char)47,(char) 61,(char) 62, (char)9, (char)13, (char)32,
			(char)61, (char)9, (char)13,(char) 32, (char)34, (char)39,(char) 9,(char) 13, (char)34, (char)34, (char)32, (char)47, (char)62,(char) 9,(char) 13, (char)32, (char)62, (char)9, (char)13,(char) 32,(char) 62,(char) 9, (char)13,(char) 39,(char) 39, (char)32, (char)60, (char)9,(char) 13,(char) 60, (char)32,(char) 47,
			(char)62,(char) 9,(char) 13,(char) 32,(char) 47, (char)62, (char)9, (char)13,(char) 32,(char) 47, (char)61,(char) 62, (char)9, (char)13,(char) 32,(char) 47,(char) 61,(char) 62,(char) 9, (char)13,(char) 32, (char)61,(char) 9, (char)13, (char)32, (char)34,(char) 39,(char) 9,(char) 13,(char) 34, (char)34,(char) 32,
			(char)47, (char)62,(char) 9,(char) 13,(char) 32, (char)62,(char) 9,(char) 13,(char) 32,(char) 62, (char)9, (char)13,(char) 60,(char) 32,(char) 47,(char) 9,(char) 13,(char) 62,(char) 62, (char)39,(char) 39, (char)32,(char) 9,(char) 13,(char) 0};
	}

	private static readonly char[] _xml_trans_keys = init__xml_trans_keys_0();

	private static byte[] init__xml_single_lengths_0 () {
		return new byte[] {0, 2, 3, 3, 4, 4, 2, 3, 1, 1, 3, 2, 2, 1, 1, 2, 1, 3, 3, 4, 4, 2, 3, 1, 1, 3, 2, 2, 1, 2, 1, 1, 1, 1, 1,
			0};
	}

	private static readonly byte[] _xml_single_lengths = init__xml_single_lengths_0();

	private static byte[] init__xml_range_lengths_0 () {
		return new byte[] {0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 0, 0, 1, 0, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 0, 1, 0, 0, 0, 0, 1,
			0};
	}

	private static readonly byte[] _xml_range_lengths = init__xml_range_lengths_0();

	private static short[] init__xml_index_offsets_0 () {
		return new short[] {0, 0, 4, 9, 14, 20, 26, 30, 35, 37, 39, 44, 48, 52, 54, 56, 60, 62, 67, 72, 78, 84, 88, 93, 95, 97, 102,
			106, 110, 112, 116, 118, 120, 122, 124, 127};
	}

	private static readonly short[] _xml_index_offsets = init__xml_index_offsets_0();

	private static byte[] init__xml_indicies_0 () {
		return new byte[] {0, 2, 0, 1, 2, 1, 1, 2, 3, 5, 6, 7, 5, 4, 9, 10, 1, 11, 9, 8, 13, 1, 14, 1, 13, 12, 15, 16, 15, 1, 16,
			17, 18, 16, 1, 20, 19, 22, 21, 9, 10, 11, 9, 1, 23, 24, 23, 1, 25, 11, 25, 1, 20, 26, 22, 27, 29, 30, 29, 28, 32, 31, 30,
			34, 1, 30, 33, 36, 37, 38, 36, 35, 40, 41, 1, 42, 40, 39, 44, 1, 45, 1, 44, 43, 46, 47, 46, 1, 47, 48, 49, 47, 1, 51, 50,
			53, 52, 40, 41, 42, 40, 1, 54, 55, 54, 1, 56, 42, 56, 1, 57, 1, 57, 34, 57, 1, 1, 58, 59, 58, 51, 60, 53, 61, 62, 62, 1,
			1, 0};
	}

	private static readonly byte[] _xml_indicies = init__xml_indicies_0();

	private static byte[] init__xml_trans_targs_0 () {
		return new byte[] {1, 0, 2, 3, 3, 4, 11, 34, 5, 4, 11, 34, 5, 6, 7, 6, 7, 8, 13, 9, 10, 9, 10, 12, 34, 12, 14, 14, 16, 15,
			17, 16, 17, 18, 30, 18, 19, 26, 28, 20, 19, 26, 28, 20, 21, 22, 21, 22, 23, 32, 24, 25, 24, 25, 27, 28, 27, 29, 31, 35,
			33, 33, 34};
	}

	private static readonly byte[] _xml_trans_targs = init__xml_trans_targs_0();

	private static byte[] init__xml_trans_actions_0 () {
		return new byte[] {0, 0, 0, 1, 0, 3, 3, 13, 1, 0, 0, 9, 0, 11, 11, 0, 0, 0, 0, 1, 25, 0, 19, 5, 16, 0, 1, 0, 1, 0, 0, 0, 22,
			1, 0, 0, 3, 3, 13, 1, 0, 0, 9, 0, 11, 11, 0, 0, 0, 0, 1, 25, 0, 19, 5, 16, 0, 0, 0, 7, 1, 0, 0};
	}

	private static readonly byte[] _xml_trans_actions = init__xml_trans_actions_0();

	static readonly int xml_start = 1;
	static readonly int xml_first_final = 34;
	static readonly int xml_error = 0;

	static readonly int xml_en_elementBody = 15;
	static readonly int xml_en_main = 1;

	// line 215 "XmlReader.rl"

	protected void open (String name) {
		Element child = new Element(name, current);
		Element parent = current;
		if (parent != null) parent.addChild(child);
		elements.add(child);
		current = child;
	}

	protected void attribute (String name, String value) {
		current.setAttribute(name, value);
	}

	protected String? entity (String name) {
		if (name.Equals("lt")) return "<";
		if (name.Equals("gt")) return ">";
		if (name.Equals("amp")) return "&";
		if (name.Equals("apos")) return "'";
		if (name.Equals("quot")) return "\"";
		if (name.StartsWith("#x")) return ((char)Convert.ToInt32(name.Substring(2), 16)).ToString();
		return null;
	}

	protected void text (String text) {
		String existing = current.getText();
		current.setText(existing != null ? existing + text : text);
	}

	protected void close () {
		root = elements.pop();
		current = elements.size > 0 ? elements.peek() : null;
	}

	 public class Element {
		private readonly String _name;
		private ObjectMap<String, String> attributes;
		private Array<Element> children;
		private String text;
		private Element parent;

		public Element (String name, Element parent) {
			this._name = name;
			this.parent = parent;
		}

		public String getName () {
			return _name;
		}

		public ObjectMap<String, String> getAttributes () {
			return attributes;
		}

		/** @throws GdxRuntimeException if the attribute was not found. */
		public String getAttribute (String name) {
			if (attributes == null) throw new GdxRuntimeException("Element " + this._name + " doesn't have attribute: " + name);
			String value = attributes.get(name);
			if (value == null) throw new GdxRuntimeException("Element " + this._name + " doesn't have attribute: " + name);
			return value;
		}

		public String getAttribute (String name, String defaultValue) {
			if (attributes == null) return defaultValue;
			String value = attributes.get(name);
			if (value == null) return defaultValue;
			return value;
		}

		public bool hasAttribute (String name) {
			if (attributes == null) return false;
			return attributes.containsKey(name);
		}

		public void setAttribute (String name, String value) {
			if (attributes == null) attributes = new (8);
			attributes.put(name, value);
		}

		public int getChildCount () {
			if (children == null) return 0;
			return children.size;
		}

		/** @throws GdxRuntimeException if the element has no children. */
		public Element getChild (int index) {
			if (children == null) throw new GdxRuntimeException("Element has no children: " + _name);
			return children.get(index);
		}

		public void addChild (Element element) {
			if (children == null) children = new (8);
			children.add(element);
		}

		public String getText () {
			return text;
		}

		public void setText (String text) {
			this.text = text;
		}

		public void removeChild (int index) {
			if (children != null) children.removeIndex(index);
		}

		public void removeChild (Element child) {
			if (children != null) children.removeValue(child, true);
		}

		public void remove () {
			parent.removeChild(this);
		}

		public Element getParent () {
			return parent;
		}

		public override String ToString () {
			return toString("");
		}

		public String toString (String indent) {
			StringBuilder buffer = new StringBuilder(128);
			buffer.Append(indent);
			buffer.Append('<');
			buffer.Append(_name);
			if (attributes != null) {
				foreach (var entry in attributes.entries()) {
					buffer.Append(' ');
					buffer.Append(entry.key);
					buffer.Append("=\"");
					buffer.Append(entry.value);
					buffer.Append('\"');
				}
			}
			if (children == null && (text == null || text.Length == 0))
				buffer.Append("/>");
			else {
				buffer.Append(">\n");
				String childIndent = indent + '\t';
				if (text != null && text.Length > 0) {
					buffer.Append(childIndent);
					buffer.Append(text);
					buffer.Append('\n');
				}
				if (children != null) {
					foreach (Element child in children) {
						buffer.Append(child.toString(childIndent));
						buffer.Append('\n');
					}
				}
				buffer.Append(indent);
				buffer.Append("</");
				buffer.Append(_name);
				buffer.Append('>');
			}
			return buffer.ToString();
		}

		/** @param name the name of the child {@link Element}
		 * @return the first child having the given name or null, does not recurse */
		public Element? getChildByName (String name) {
			if (children == null) return null;
			for (int i = 0; i < children.size; i++) {
				Element element = children.get(i);
				if (element._name.Equals(name)) return element;
			}
			return null;
		}

		public bool hasChild (String name) {
			if (children == null) return false;
			return getChildByName(name) != null;
		}

		/** @param name the name of the child {@link Element}
		 * @return the first child having the given name or null, recurses */
		public Element? getChildByNameRecursive (String name) {
			if (children == null) return null;
			for (int i = 0; i < children.size; i++) {
				Element element = children.get(i);
				if (element._name.Equals(name)) return element;
				Element found = element.getChildByNameRecursive(name);
				if (found != null) return found;
			}
			return null;
		}

		public bool hasChildRecursive (String name) {
			if (children == null) return false;
			return getChildByNameRecursive(name) != null;
		}

		/** @param name the name of the children
		 * @return the children with the given name or an empty {@link Array} */
		public Array<Element> getChildrenByName (String name) {
			Array<Element> result = new Array<Element>();
			if (children == null) return result;
			for (int i = 0; i < children.size; i++) {
				Element child = children.get(i);
				if (child._name.Equals(name)) result.add(child);
			}
			return result;
		}

		/** @param name the name of the children
		 * @return the children with the given name or an empty {@link Array} */
		public Array<Element> getChildrenByNameRecursively (String name) {
			Array<Element> result = new Array<Element>();
			getChildrenByNameRecursively(name, result);
			return result;
		}

		private void getChildrenByNameRecursively (String name, Array<Element> result) {
			if (children == null) return;
			for (int i = 0; i < children.size; i++) {
				Element child = children.get(i);
				if (child._name.Equals(name)) result.add(child);
				child.getChildrenByNameRecursively(name, result);
			}
		}

		/** @throws GdxRuntimeException if the attribute was not found. */
		public float getFloatAttribute (String name) {
			return float.Parse(getAttribute(name));
		}

		public float getFloatAttribute (String name, float defaultValue) {
			String value = getAttribute(name, null);
			if (value == null) return defaultValue;
			return float.Parse(value);
		}

		/** @throws GdxRuntimeException if the attribute was not found. */
		public int getIntAttribute (String name) {
			return int.Parse(getAttribute(name));
		}

		public int getIntAttribute (String name, int defaultValue) {
			String value = getAttribute(name, null);
			if (value == null) return defaultValue;
			return int.Parse(value);
		}

		/** @throws GdxRuntimeException if the attribute was not found. */
		public bool getBooleanAttribute (String name) {
			return bool.Parse(getAttribute(name));
		}

		public bool getBooleanAttribute (String name, bool defaultValue) {
			String value = getAttribute(name, null);
			if (value == null) return defaultValue;
			return bool.Parse(value);
		}

		/** Returns the attribute value with the specified name, or if no attribute is found, the text of a child with the name.
		 * @throws GdxRuntimeException if no attribute or child was not found. */
		public String get (String name) {
			String value = get(name, null);
			if (value == null) throw new GdxRuntimeException("Element " + this._name + " doesn't have attribute or child: " + name);
			return value;
		}

		/** Returns the attribute value with the specified name, or if no attribute is found, the text of a child with the name.
		 * @throws GdxRuntimeException if no attribute or child was not found. */
		public String get (String name, String defaultValue) {
			if (attributes != null) {
				String value = attributes.get(name);
				if (value != null) return value;
			}

			{
				Element child = getChildByName(name);
				if (child == null) return defaultValue;
				String value = child.getText();
				if (value == null) return defaultValue;
				return value;
			}
		}

		/** Returns the attribute value with the specified name, or if no attribute is found, the text of a child with the name.
		 * @throws GdxRuntimeException if no attribute or child was not found. */
		public int getInt (String name) {
			String value = get(name, null);
			if (value == null) throw new GdxRuntimeException("Element " + this._name + " doesn't have attribute or child: " + name);
			return int.Parse(value);
		}

		/** Returns the attribute value with the specified name, or if no attribute is found, the text of a child with the name.
		 * @throws GdxRuntimeException if no attribute or child was not found. */
		public int getInt (String name, int defaultValue) {
			String value = get(name, null);
			if (value == null) return defaultValue;
			return int.Parse(value);
		}

		/** Returns the attribute value with the specified name, or if no attribute is found, the text of a child with the name.
		 * @throws GdxRuntimeException if no attribute or child was not found. */
		public float getFloat (String name) {
			String value = get(name, null);
			if (value == null) throw new GdxRuntimeException("Element " + this._name + " doesn't have attribute or child: " + name);
			return float.Parse(value);
		}

		/** Returns the attribute value with the specified name, or if no attribute is found, the text of a child with the name.
		 * @throws GdxRuntimeException if no attribute or child was not found. */
		public float getFloat (String name, float defaultValue) {
			String value = get(name, null);
			if (value == null) return defaultValue;
			return float.Parse(value);
		}

		/** Returns the attribute value with the specified name, or if no attribute is found, the text of a child with the name.
		 * @throws GdxRuntimeException if no attribute or child was not found. */
		public bool getBoolean (String name) {
			String value = get(name, null);
			if (value == null) throw new GdxRuntimeException("Element " + this._name + " doesn't have attribute or child: " + name);
			return bool.Parse(value);
		}

		/** Returns the attribute value with the specified name, or if no attribute is found, the text of a child with the name.
		 * @throws GdxRuntimeException if no attribute or child was not found. */
		public bool getBoolean (String name, bool defaultValue) {
			String value = get(name, null);
			if (value == null) return defaultValue;
			return bool.Parse(value);
		}
	}
}