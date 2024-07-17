using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpGDX.Shims;
using System.Threading.Tasks;

namespace SharpGDX.Utils.Compression.LZ
{
	public class OutWindow {
	byte[] _buffer;
	int _pos;
	int _windowSize = 0;
	int _streamPos;
	OutputStream _stream;

	public void Create (int windowSize) {
		if (_buffer == null || _windowSize != windowSize) _buffer = new byte[windowSize];
		_windowSize = windowSize;
		_pos = 0;
		_streamPos = 0;
	}

	public void SetStream (OutputStream stream) // TODO: throws IOException 
	{
		ReleaseStream();
		_stream = stream;
	}

	public void ReleaseStream () // TODO: throws IOException 
	{
		Flush();
		_stream = null;
	}

	public void Init (bool solid) {
		if (!solid) {
			_streamPos = 0;
			_pos = 0;
		}
	}

	public void Flush () // TODO: throws IOException 
	{
		int size = _pos - _streamPos;
		if (size == 0) return;
		_stream.write(_buffer, _streamPos, size);
		if (_pos >= _windowSize) _pos = 0;
		_streamPos = _pos;
	}

	public void CopyBlock (int distance, int len) // TODO: throws IOException  throws IOException 
		{
		int pos = _pos - distance - 1;
		if (pos < 0) pos += _windowSize;
		for (; len != 0; len--) {
			if (pos >= _windowSize) pos = 0;
			_buffer[_pos++] = _buffer[pos++];
			if (_pos >= _windowSize) Flush();
		}
	}

	public void PutByte (byte b) // TODO: throws IOException  throws IOException 
		{
		_buffer[_pos++] = b;
		if (_pos >= _windowSize) Flush();
	}

	public byte GetByte (int distance) {
		int pos = _pos - distance - 1;
		if (pos < 0) pos += _windowSize;
		return _buffer[pos];
	}
}
}
