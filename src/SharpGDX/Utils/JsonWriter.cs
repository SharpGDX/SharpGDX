using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpGDX.Shims;

namespace SharpGDX.Utils
{
	/** Builder style API for emitting JSON.
 * @author Nathan Sweet */
public class JsonWriter : Writer {
	readonly Writer writer;
	private readonly Array<JsonObject> stack = new ();
	private JsonObject current;
	private bool named;
	private OutputType outputType = OutputType.json;
	private bool quoteLongValues = false;

	public JsonWriter (Writer writer) {
		this.writer = writer;
	}

	public Writer getWriter () {
		return writer;
	}

	/** Sets the type of JSON output. Default is {@link OutputType#minimal}. */
	public void setOutputType (OutputType outputType) {
		this.outputType = outputType;
	}

	/** When true, quotes long, double, BigInteger, BigDecimal types to prevent truncation in languages like JavaScript and PHP.
	 * This is not necessary when using libgdx, which handles these types without truncation. Default is false. */
	public void setQuoteLongValues (bool quoteLongValues) {
		this.quoteLongValues = quoteLongValues;
	}

	public JsonWriter name (String name)// TODO: throws IOException
	{
		if (current == null || current.array) throw new IllegalStateException("Current item must be an object.");
		if (!current.needsComma)
			current.needsComma = true;
		else
			writer.write(',');
		writer.write(OutputTypeUtils.quoteName(name));
		writer.write(':');
		named = true;
		return this;
	}

	public JsonWriter @object ()// TODO: throws IOException 
	{
		requireCommaOrName();
		stack.add(current = new JsonObject(this,false));
		return this;
	}

	public JsonWriter array ()// TODO: throws IOException 
	{
		requireCommaOrName();
		stack.add(current = new JsonObject(this,true));
		return this;
	}

	public JsonWriter value (Object value)// TODO: throws IOException 
	{
		throw new NotImplementedException();
		//if (quoteLongValues
		//	&& (value is long || value is Double || value is BigDecimal || value is BigInteger)) {
		//	value = value.ToString();
		//} else if (value is Number) {
		//	Number number = (Number)value;
		//	long longValue = number.longValue();
		//	if (number.doubleValue() == longValue) value = longValue;
		//}
		//requireCommaOrName();
		//writer.write(OutputTypeUtils.quoteValue(value));
		//return this;
	}

	/** Writes the specified JSON value, without quoting or escaping. */
	public JsonWriter json (String json) // TODO: throws IOException 
	{
		requireCommaOrName();
		writer.write(json);
		return this;
	}

	private void requireCommaOrName () // TODO: throws IOException 
	{
		if (current == null) return;
		if (current.array) {
			if (!current.needsComma)
				current.needsComma = true;
			else
				writer.write(',');
		} else {
			if (!named) throw new IllegalStateException("Name must be set.");
			named = false;
		}
	}

	public JsonWriter @object (String name)// TODO: throws IOException 
	{
		return this.name(name).@object();
	}

	public JsonWriter array (String name)// TODO: throws IOException 
{
		return this.name(name).array();
	}

	public JsonWriter set (String name, Object value)// TODO: throws IOException 
{
		return this.name(name).value(value);
	}

	/** Writes the specified JSON value, without quoting or escaping. */
	public JsonWriter json (String name, String json)// TODO: throws IOException
{
		return this.name(name).json(json);
	}

	public JsonWriter pop ()// TODO: throws IOException 
{
		if (named) throw new IllegalStateException("Expected an object, array, or value since a name was set.");
		stack.pop().close();
		current = stack.size == 0 ? null : stack.peek();
		return this;
	}

	public override void write (char[] cbuf, int off, int len)// TODO: throws IOException 
{
		writer.write(cbuf, off, len);
	}

	public override void flush () // TODO: throws IOException 
{
		writer.flush();
	}

	public override void close () // TODO: throws IOException 
{
		while (stack.size > 0)
			pop();
		writer.close();
	}

	private class JsonObject {
		internal readonly bool array;
		internal bool needsComma;
		private JsonWriter writer;

		internal JsonObject (JsonWriter writer, bool array)// TODO: throws IOException
		{
			this.writer = writer;
			this.array = array;
			writer.write(array ? '[' : '{');
		}

		internal void close ()// TODO: throws IOException
	{
			writer.write(array ? ']' : '}');
		}
	}

	public enum OutputType {
		/** Normal JSON, with all its double quotes. */
		json,
		/** Like JSON, but names are only double quoted if necessary. */
		javascript,
		/** Like JSON, but:
		 * <ul>
		 * <li>Names only require double quotes if they start with <code>space</code> or any of <code>":,}/</code> or they contain
		 * <code>//</code> or <code>/*</code> or <code>:</code>.
		 * <li>Values only require double quotes if they start with <code>space</code> or any of <code>":,{[]/</code> or they
		 * contain <code>//</code> or <code>/*</code> or any of <code>}],</code> or they are equal to <code>true</code>,
		 * <code>false</code> , or <code>null</code>.
		 * <li>Newlines are treated as commas, making commas optional in many cases.
		 * <li>C style comments may be used: <code>//...</code> or <code>/*...*<b></b>/</code>
		 * </ul>
		 */
		minimal

		
	}

	public class OutputTypeUtils
	{
		// TODO: static private Pattern javascriptPattern = Pattern.compile("^[a-zA-Z_$][a-zA-Z_$0-9]*$");
			// TODO: static private Pattern minimalNamePattern = Pattern.compile("^[^\":,}/ ][^:]*$");
			// TODO: static private Pattern minimalValuePattern = Pattern.compile("^[^\":,{\\[\\]/ ][^}\\],]*$");

			static public String quoteValue(Object value)
		{
			throw new NotImplementedException();
			//if (value == null) return "null";
			//String str = value.toString();
			//if (value is Number || value is Boolean) return str;
			//StringBuilder buffer = new StringBuilder(str);
			//buffer.replace('\\', "\\\\").replace('\r', "\\r").replace('\n', "\\n").replace('\t', "\\t");
			//if (this == OutputType.minimal && !str.equals("true") && !str.equals("false") && !str.equals("null")
			//    && !str.contains("//") && !str.contains("/*"))
			//{
			//	int length = buffer.length();
			//	if (length > 0 && buffer.charAt(length - 1) != ' ' && minimalValuePattern.matcher(buffer).matches())
			//		return buffer.toString();
			//}
			//return '"' + buffer.replace('"', "\\\"").toString() + '"';
		}

		public static String quoteName(String value)
		{
			throw new NotImplementedException();
			//	StringBuilder buffer = new StringBuilder(value);
			//buffer.replace('\\', "\\\\").replace('\r', "\\r").replace('\n', "\\n").replace('\t', "\\t");
			//switch (this)
			//{
			//	case minimal:
			//		if (!value.contains("//") && !value.contains("/*") && minimalNamePattern.matcher(buffer).matches())
			//			return buffer.toString();
			//	case javascript:
			//		if (javascriptPattern.matcher(buffer).matches()) return buffer.toString();
			//}
			//return '"' + buffer.replace('"', "\\\"").toString() + '"';
		}
		}
}
}
