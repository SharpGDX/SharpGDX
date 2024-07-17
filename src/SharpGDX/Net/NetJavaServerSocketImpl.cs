using SharpGDX.Shims;
using SharpGDX.Net;
using SharpGDX.Utils;

namespace SharpGDX;

/** Server socket implementation using java.net.ServerSocket.
 * 
 * @author noblemaster */
public class NetJavaServerSocketImpl : ServerSocket {

	private INet.Protocol protocol;

	/** Our server or null for disposed, aka closed. */
	// TODO: private  java.net.ServerSocket server;

	public NetJavaServerSocketImpl (INet.Protocol protocol, int port, ServerSocketHints hints) 
	: this(protocol, null, port, hints)
	{
		
	}

	public NetJavaServerSocketImpl (INet.Protocol protocol, String hostname, int port, ServerSocketHints hints) {
		throw new NotImplementedException();
		//this.protocol = protocol;
		
		// // create the server socket
		//try {
		//	// initialize
		//	server = new java.net.ServerSocket();
		//	if (hints != null) {
		//		server.setPerformancePreferences(hints.performancePrefConnectionTime, hints.performancePrefLatency,
		//			hints.performancePrefBandwidth);
		//		server.setReuseAddress(hints.reuseAddress);
		//		server.setSoTimeout(hints.acceptTimeout);
		//		server.setReceiveBufferSize(hints.receiveBufferSize);
		//	}

		//	// and bind the server...
		//	InetSocketAddress address;
		//	if (hostname != null) {
		//		address = new InetSocketAddress(hostname, port);
		//	} else {
		//		address = new InetSocketAddress(port);
		//	}

		//	if (hints != null) {
		//		server.bind(address, hints.backlog);
		//	} else {
		//		server.bind(address);
		//	}
		//} catch (Exception e) {
		//	throw new GdxRuntimeException("Cannot create a server socket at port " + port + ".", e);
		//}
	}

	
	public INet.Protocol getProtocol () {
		return protocol;
	}

	
	public Socket accept (SocketHints hints)
	{
		throw new NotImplementedException();
		//try {
		//	return new NetJavaSocketImpl(server.accept(), hints);
		//} catch (Exception e) {
		//	throw new GdxRuntimeException("Error accepting socket.", e);
		//}
	}

	
	public void dispose () {
		throw new NotImplementedException();
		//if (server != null) {
		//	try {
		//		server.close();
		//		server = null;
		//	} catch (Exception e) {
		//		throw new GdxRuntimeException("Error closing server.", e);
		//	}
		//}
	}
}
