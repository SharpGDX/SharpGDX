using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Shims;

namespace SharpGDX.Utils
{
	/** Indicates an error during serialization due to misconfiguration or during deserialization due to invalid input data.
 * @author Nathan Sweet */
public class SerializationException : Exception {
	private StringBuilder trace;

	public SerializationException () {
	}

	public SerializationException (String message, Exception cause) 
	: base(message, cause)
	{
		
	}

	public SerializationException (String message) 
	: base(message)
	{
		
	}

	public SerializationException (Exception cause) 
	: base("", cause)
	{
		
	}

	/** Returns true if any of the exceptions that caused this exception are of the specified type. */
	public bool causedBy (Type type) {
		return causedBy(this, type);
	}

	private bool causedBy (Exception ex, Type type) {
		// TODO: this was originally ex.getCause()
		Exception cause = ex.InnerException;
		if (cause == null || cause == ex) return false;
		if (type.IsAssignableFrom(cause.GetType())) return true;
		return causedBy(cause, type);
	}

	public String getMessage () {
		if (trace == null) return base.Message;
		StringBuilder sb = new StringBuilder(512);
		sb.Append(base.Message);
		if (sb.Length > 0) sb.Append('\n');
		sb.Append("Serialization trace:");
		sb.Append(trace);
		return sb.ToString();
	}

	/** Adds information to the exception message about where in the the object graph serialization failure occurred. Serializers
	 * can catch {@link SerializationException}, add trace information, and rethrow the exception. */
	public void addTrace (String info) {
		if (info == null) throw new IllegalArgumentException("info cannot be null.");
		if (trace == null) trace = new StringBuilder(512);
		trace.Append('\n');
		trace.Append(info);
	}
}
}
