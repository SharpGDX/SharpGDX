using SharpGDX.Net;
using SharpGDX.Utils;
using SharpGDX.Shims;

namespace SharpGDX;

/** Socket implementation using java.net.Socket.
 * 
 * @author noblemaster */
public class NetJavaSocketImpl : Socket {

	/** Our socket or null for disposed, aka closed. */
	// TODO: private java.net.Socket socket;

	public NetJavaSocketImpl (INet.Protocol protocol, String host, int port, SocketHints hints)
	{
		throw new NotImplementedException();
		//try {
		//	// create the socket
		//	socket = new java.net.Socket();
		//	applyHints(hints); // better to call BEFORE socket is connected!

		//	// and connect...
		//	InetSocketAddress address = new InetSocketAddress(host, port);
		//	if (hints != null) {
		//		socket.connect(address, hints.connectTimeout);
		//	} else {
		//		socket.connect(address);
		//	}
		//} catch (Exception e) {
		//	throw new GdxRuntimeException("Error making a socket connection to " + host + ":" + port, e);
		//}
	}

	public NetJavaSocketImpl (System.Net.Sockets.Socket socket, SocketHints hints) {
		throw new NotImplementedException();
		//this.socket = socket;
		//applyHints(hints);
	}

	private void applyHints (SocketHints hints)
	{
		throw new NotImplementedException();
		//if (hints != null) {
		//	try {
		//		socket.setPerformancePreferences(hints.performancePrefConnectionTime, hints.performancePrefLatency,
		//			hints.performancePrefBandwidth);
		//		socket.setTrafficClass(hints.trafficClass);
		//		socket.setTcpNoDelay(hints.tcpNoDelay);
		//		socket.setKeepAlive(hints.keepAlive);
		//		socket.setSendBufferSize(hints.sendBufferSize);
		//		socket.setReceiveBufferSize(hints.receiveBufferSize);
		//		socket.setSoLinger(hints.linger, hints.lingerDuration);
		//		socket.setSoTimeout(hints.socketTimeout);
		//	} catch (Exception e) {
		//		throw new GdxRuntimeException("Error setting socket hints.", e);
		//	}
		//}
	}

	public bool isConnected () {
		throw new NotImplementedException();
		//if (socket != null) {
		//	return socket.isConnected();
		//} else {
		//	return false;
		//}
	}

	public InputStream getInputStream () {
		throw new NotImplementedException();
		//try {
		//	return socket.getInputStream();
		//} catch (Exception e) {
		//	throw new GdxRuntimeException("Error getting input stream from socket.", e);
		//}
	}

	public OutputStream getOutputStream () {
		throw new NotImplementedException();
		//try {
		//	return socket.getOutputStream();
		//} catch (Exception e) {
		//	throw new GdxRuntimeException("Error getting output stream from socket.", e);
		//}
	}

	public String getRemoteAddress () {
		throw new NotImplementedException();
		//return socket.getRemoteSocketAddress().toString();
	}

	public void dispose () {
		throw new NotImplementedException();
		//if (socket != null) {
		//	try {
		//		socket.close();
		//		socket = null;
		//	} catch (Exception e) {
		//		throw new GdxRuntimeException("Error closing socket.", e);
		//	}
		//}
	}
}
