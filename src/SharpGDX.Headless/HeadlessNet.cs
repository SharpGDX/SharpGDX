using SharpGDX.Net;

namespace SharpGDX.Headless;

public class HeadlessNet : INet
{
	public HeadlessNet(HeadlessApplicationConfiguration configuration){}
	public void sendHttpRequest(INet.HttpRequest httpRequest, INet.HttpResponseListener? httpResponseListener)
	{
		throw new NotImplementedException();
	}

	public void cancelHttpRequest(INet.HttpRequest httpRequest)
	{
		throw new NotImplementedException();
	}

	public ServerSocket newServerSocket(INet.Protocol protocol, string hostname, int port, ServerSocketHints hints)
	{
		throw new NotImplementedException();
	}

	public ServerSocket newServerSocket(INet.Protocol protocol, int port, ServerSocketHints hints)
	{
		throw new NotImplementedException();
	}

	public Socket newClientSocket(INet.Protocol protocol, string host, int port, SocketHints hints)
	{
		throw new NotImplementedException();
	}

	public bool openURI(string URI)
	{
		throw new NotImplementedException();
	}
}