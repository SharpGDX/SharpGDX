using SharpGDX.Shims;

namespace SharpGDX.Shims
{
	/*** The base class for all writers. A writer is a means of writing data to a target in a character-wise manner. Most output
	 * streams expect the {@link #flush()} method to be called before closing the stream, to ensure all data is actually written out.
	 * <p>
	 * This abstract class does not provide a fully working implementation, so it needs to be subclassed, and at least the
	 * {@link #write(char[], int, int)}, {@link #close()} and {@link #flush()} methods needs to be overridden. Overriding some of the
	 * non-abstract methods is also often advised, since it might result in higher efficiency.
	 * <p>
	 * Many specialized readers for purposes like reading from a file already exist in this package.
	 *
	 * @see Reader */
	public abstract class Writer : ICloseable
	{

		static readonly String TOKEN_NULL = "null"; //$NON-NLS-1$

		/*** The object used to synchronize access to the writer. */
		protected Object _lock;

		/*** Constructs a new {@code Writer} with {@code this} as the object used to synchronize critical sections. */
		protected Writer()
		{
			_lock = this;
		}

		/*** Constructs a new {@code Writer} with {@code lock} used to synchronize critical sections.
		 *
		 * @param lock the {@code Object} used to synchronize critical sections.
		 * @throws NullPointerException if {@code lock} is {@code null}. */
		protected Writer(Object @lock)
		{
			if (@lock == null)
			{
				throw new NullPointerException();
			}

			this._lock = @lock;
		}

		/*** Closes this writer. Implementations of this method should free any resources associated with the writer.
		 *
		 * @throws IOException if an error occurs while closing this writer. */
		public abstract void close(); // TODO: throws IOException;

		/*** Flushes this writer. Implementations of this method should ensure that all buffered characters are written to the target.
		 *
		 * @throws IOException if an error occurs while flushing this writer. */
		public abstract void flush(); // TODO: throws IOException;

		/*** Writes the entire character buffer {@code buf} to the target.
		 *
		 * @param buf the non-null array containing characters to write.
		 * @throws IOException if this writer is closed or another I/O error occurs. */
		public void write(char[] buf) // TODO: throws IOException
		{
			write(buf, 0, buf.Length);
		}

		/*** Writes {@code count} characters starting at {@code offset} in {@code buf} to the target.
		 *
		 * @param buf the non-null character array to write.
		 * @param offset the index of the first character in {@code buf} to write.
		 * @param count the maximum number of characters to write.
		 * @throws IndexOutOfBoundsException if {@code offset < 0} or {@code count < 0}, or if {@code offset + count} is greater than
		 *            the size of {@code buf}.
		 * @throws IOException if this writer is closed or another I/O error occurs. */
		public abstract void write(char[] buf, int offset, int count); // TODO:  throws IOException;

		/*** Writes one character to the target. Only the two least significant bytes of the integer {@code oneChar} are written.
		 *
		 * @param oneChar the character to write to the target.
		 * @throws IOException if this writer is closed or another I/O error occurs. */
		public void write(int oneChar) // TODO: throws IOException
		{
			lock (_lock)
			{
				char[] oneCharArray = new char[1];
				oneCharArray[0] = (char)oneChar;
				write(oneCharArray);
			}
		}

		/*** Writes the characters from the specified string to the target.
		 *
		 * @param str the non-null string containing the characters to write.
		 * @throws IOException if this writer is closed or another I/O error occurs. */
		public void write(String str) // TODO: throws IOException
		{
			write(str, 0, str.Length);
		}

		/*** Writes {@code count} characters from {@code str} starting at {@code offset} to the target.
		 *
		 * @param str the non-null string containing the characters to write.
		 * @param offset the index of the first character in {@code str} to write.
		 * @param count the number of characters from {@code str} to write.
		 * @throws IOException if this writer is closed or another I/O error occurs.
		 * @throws IndexOutOfBoundsException if {@code offset < 0} or {@code count < 0}, or if {@code offset + count} is greater than
		 *            the length of {@code str}. */
		public void write(String str, int offset, int count) // TODO: throws IOException
		{
			if (count < 0)
			{
				// other cases tested by getChars()
				throw new StringIndexOutOfBoundsException();
			}

			char[] buf = new char[count];
			str.CopyTo(offset, buf, 0, offset + count);

			lock (_lock)
			{
				write(buf, 0, buf.Length);
			}
		}

/*** Appends the character {@code c} to the target. This method works the same way as {@link #write(int)}.
 *
 * @param c the character to append to the target stream.
 * @return this writer.
 * @throws IOException if this writer is closed or another I/O error occurs. */
		public Writer append(char c) // TODO: throws IOException
		{
			write(c);
			return this;
		}

/*** Appends the character sequence {@code csq} to the target. This method works the same way as
 * {@code Writer.write(csq.toString())}. If {@code csq} is {@code null}, then the string "null" is written to the target
 * stream.
 *
 * @param csq the character sequence appended to the target.
 * @return this writer.
 * @throws IOException if this writer is closed or another I/O error occurs. */
		public Writer append(string? csq) // TODO:throws IOException
		{
			if (null == csq)
			{
				write(TOKEN_NULL);
			}
			else
			{
				write(csq.ToString());
			}

			return this;
		}

/*** Appends a subsequence of the character sequence {@code csq} to the target. This method works the same way as
 * {@code Writer.writer(csq.subsequence(start, end).toString())}. If {@code csq} is {@code null}, then the specified
 * subsequence of the string "null" will be written to the target.
 *
 * @param csq the character sequence appended to the target.
 * @param start the index of the first char in the character sequence appended to the target.
 * @param end the index of the character following the last character of the subsequence appended to the target.
 * @return this writer.
 * @throws IOException if this writer is closed or another I/O error occurs.
 * @throws IndexOutOfBoundsException if {@code start > end}, {@code start < 0}, {@code end < 0} or either {@code start} or
 *            {@code end} are greater or equal than the length of {@code csq}. */
		public Writer append(string? csq, int start, int end) // TODO:throws IOException
		{
			if (null == csq)
			{
				write(TOKEN_NULL.Substring(start, end));
			}
			else
			{
				write(csq.Substring(start, end).ToString());
			}

			return this;
		}

/*** Returns true if this writer has encountered and suppressed an error. Used by PrintWriters as an alternative to checked
 * exceptions. */
		bool checkError()
		{
			return false;
		}
	}
}