using System;
using SharpGDX.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Desktop
{
	public class DesktopNet : INet
	{
		public DesktopNet(DesktopApplicationConfiguration config) { }
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
}
