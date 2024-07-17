using SharpGDX.Shims;

namespace SharpGDX.Utils
{
	//@off
/**
 * Builder style API for emitting XML. <pre>
 * StringWriter writer = new StringWriter();
 * XmlWriter xml = new XmlWriter(writer);
 * xml.element("meow")
 *	.attribute("moo", "cow")
 *	.element("child")
 *		.attribute("moo", "cow")
 *		.element("child")
 *			.attribute("moo", "cow")
 *			.text("XML is like violence. If it doesn't solve your problem, you're not using enough of it.")
 *		.pop()
 *	.pop()
 * .pop();
 * System.out.println(writer);
 * </pre>
 * @author Nathan Sweet
 */
//@on
public class XmlWriter : Writer {
	private readonly Writer writer;
	private readonly Array<String> stack = new ();
	private String currentElement;
	private bool indentNextClose;

	public int _indent;

	public XmlWriter (Writer writer) {
		this.writer = writer;
	}

	private void indent () // TODO: throws IOException 
	{
		int count = _indent;
		if (currentElement != null) count++;
		for (int i = 0; i < count; i++)
			writer.write('\t');
	}

	public XmlWriter element (String name) // TODO: throws IOException 
	{
		if (startElementContent()) writer.write('\n');
		indent();
		writer.write('<');
		writer.write(name);
		currentElement = name;
		return this;
	}

	public XmlWriter element (String name, Object text) // TODO: throws IOException
	{
		return element(name).text(text).pop();
	}

	private bool startElementContent () // TODO: throws IOException 
	{
		if (currentElement == null) return false;
		_indent++;
		stack.add(currentElement);
		currentElement = null;
		writer.write(">");
		return true;
	}

	public XmlWriter attribute (String name, Object value) // TODO: throws IOException 
		{
		if (currentElement == null) throw new IllegalStateException();
		writer.write(' ');
		writer.write(name);
		writer.write("=\"");
		writer.write(value == null ? "null" : value.ToString());
		writer.write('"');
		return this;
	}

	public XmlWriter text (Object text) // TODO: throws IOException 
		{
		startElementContent();
		String str = text == null ? "null" : text.ToString();
		indentNextClose = str.Length > 64;
		if (indentNextClose) {
			writer.write('\n');
			indent();
		}
		writer.write(str);
		if (indentNextClose) writer.write('\n');
		return this;
	}

	public XmlWriter pop () // TODO: throws IOException 
		{
		if (currentElement != null) {
			writer.write("/>\n");
			currentElement = null;
		} else {
			_indent = Math.Max(_indent - 1, 0);
			if (indentNextClose) indent();
			writer.write("</");
			writer.write(stack.pop());
			writer.write(">\n");
		}
		indentNextClose = true;
		return this;
	}

	/** Calls {@link #pop()} for each remaining open element, if any, and closes the stream. */
	public override void close () // TODO: throws IOException 
		{
		while (stack.size != 0)
			pop();
		writer.close();
	}

		public override void write (char[] cbuf, int off, int len) // TODO: throws IOException
		{
		startElementContent();
		writer.write(cbuf, off, len);
	}

		public override void flush () // TODO: throws IOException 
		{
		writer.flush();
	}
}
}
