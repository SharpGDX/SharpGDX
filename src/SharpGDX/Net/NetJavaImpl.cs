using SharpGDX.Shims;
using SharpGDX.Utils;

namespace SharpGDX.Net;

/** Implements part of the {@link Net} API using {@link HttpURLConnection}, to be easily reused between the Android and Desktop
 * backends.
 * @author acoppes */
public class NetJavaImpl {

	 class HttpClientResponse : INet.HttpResponse {
		// TODO: private readonly HttpURLConnection connection;
		private HttpStatus status;

		public HttpClientResponse ()// TODO: HttpURLConnection connection) // TODO: throws IOException
		{
			// TODO: this.connection = connection;
			try
			{
				// TODO: this.status = new HttpStatus(connection.getResponseCode());
			}
			catch (IOException e) {
				// TODO: this.status = new HttpStatus(-1);
			}
		}

		public byte[] getResult () {
			InputStream input = getInputStream();

			// If the response does not contain any content, input will be null.
			if (input == null) {
				return StreamUtils.EMPTY_BYTES;
			}

			try
			{
				throw new NotImplementedException();
				// TODO: return StreamUtils.copyStreamToByteArray(input, connection.getContentLength());
			}
			catch (IOException e) {
				return StreamUtils.EMPTY_BYTES;
			} finally {
				StreamUtils.closeQuietly(input);
			}
		}

		public String getResultAsString () {
			InputStream input = getInputStream();

			// If the response does not contain any content, input will be null.
			if (input == null) {
				return "";
			}

			try
			{
				throw new NotImplementedException();
				// TODO: return StreamUtils.copyStreamToString(input, connection.getContentLength(), "UTF8");
			}
			catch (IOException e) {
				return "";
			} finally {
				StreamUtils.closeQuietly(input);
			}
		}

		public InputStream getResultAsStream () {
			return getInputStream();
		}

		public HttpStatus getStatus () {
			return status;
		}

		public String getHeader (String name)
		{
			throw new NotImplementedException();
			// TODO: return connection.getHeaderField(name);
		}

		public Map<String, List<String>> getHeaders () {
			throw new NotImplementedException();
			// TODO: return connection.getHeaderFields();
		}

		private InputStream getInputStream () {
			try {
				throw new NotImplementedException();
				// TODO: return connection.getInputStream();
			}
			catch (IOException e) {
				throw new NotImplementedException();
				// TODO: return connection.getErrorStream();
			}
		}
	}

	// TODO: private readonly ThreadPoolExecutor executorService;
	// TODO: readonly ObjectMap<HttpRequest, HttpURLConnection> connections;
	// TODO: readonly ObjectMap<HttpRequest, HttpResponseListener> listeners;
	// TODO: readonly ObjectMap<HttpRequest, Future<?>> tasks;

	public NetJavaImpl () 
	: this(int.MaxValue)
	{
		
	}

	public NetJavaImpl (int maxThreads)
	{
		throw new NotImplementedException();
		// bool isCachedPool = maxThreads == int.MaxValue;
		//executorService = new ThreadPoolExecutor(isCachedPool ? 0 : maxThreads, maxThreads, 60L, TimeUnit.SECONDS,
		//	isCachedPool ? new SynchronousQueue<Runnable>() : new LinkedBlockingQueue<Runnable>(), new ThreadFactory() {
		//		AtomicInteger threadID = new AtomicInteger();

		//		@Override
		//		public Thread newThread (Runnable r) {
		//			Thread thread = new Thread(r, "NetThread" + threadID.getAndIncrement());
		//			thread.setDaemon(true);
		//			return thread;
		//		}
		//	});
		//executorService.allowCoreThreadTimeOut(!isCachedPool);
		//connections = new ObjectMap<HttpRequest, HttpURLConnection>();
		//listeners = new ObjectMap<HttpRequest, HttpResponseListener>();
		//tasks = new ObjectMap<HttpRequest, Future<?>>();
	}

	public void sendHttpRequest (INet. HttpRequest httpRequest, INet. HttpResponseListener httpResponseListener) {
		throw new NotImplementedException();
		//if (httpRequest.getUrl() == null) {
		//	httpResponseListener.failed(new GdxRuntimeException("can't process a HTTP request without URL set"));
		//	return;
		//}

		//try {
		//	final String method = httpRequest.getMethod();
		//	URL url;

		//	final boolean doInput = !method.equalsIgnoreCase(HttpMethods.HEAD);
		//	// should be enabled to upload data.
		//	final boolean doingOutPut = method.equalsIgnoreCase(HttpMethods.POST) || method.equalsIgnoreCase(HttpMethods.PUT)
		//		|| method.equalsIgnoreCase(HttpMethods.PATCH);

		//	if (method.equalsIgnoreCase(HttpMethods.GET) || method.equalsIgnoreCase(HttpMethods.HEAD)) {
		//		String queryString = "";
		//		String value = httpRequest.getContent();
		//		if (value != null && !"".equals(value)) queryString = "?" + value;
		//		url = new URL(httpRequest.getUrl() + queryString);
		//	} else {
		//		url = new URL(httpRequest.getUrl());
		//	}

		//	final HttpURLConnection connection = (HttpURLConnection)url.openConnection();
		//	connection.setDoOutput(doingOutPut);
		//	connection.setDoInput(doInput);
		//	connection.setRequestMethod(method);
		//	HttpURLConnection.setFollowRedirects(httpRequest.getFollowRedirects());

		//	putIntoConnectionsAndListeners(httpRequest, httpResponseListener, connection);

		//	// Headers get set regardless of the method
		//	for (Map.Entry<String, String> header : httpRequest.getHeaders().entrySet())
		//		connection.addRequestProperty(header.getKey(), header.getValue());

		//	// Set Timeouts
		//	connection.setConnectTimeout(httpRequest.getTimeOut());
		//	connection.setReadTimeout(httpRequest.getTimeOut());

		//	tasks.put(httpRequest, executorService.submit(new Runnable() {
		//		@Override
		//		public void run () {
		//			try {
		//				// Set the content for POST and PUT (GET has the information embedded in the URL)
		//				if (doingOutPut) {
		//					// we probably need to use the content as stream here instead of using it as a string.
		//					String contentAsString = httpRequest.getContent();
		//					if (contentAsString != null) {
		//						OutputStreamWriter writer = new OutputStreamWriter(connection.getOutputStream(), "UTF8");
		//						try {
		//							writer.write(contentAsString);
		//						} finally {
		//							StreamUtils.closeQuietly(writer);
		//						}
		//					} else {
		//						InputStream contentAsStream = httpRequest.getContentStream();
		//						if (contentAsStream != null) {
		//							OutputStream os = connection.getOutputStream();
		//							try {
		//								StreamUtils.copyStream(contentAsStream, os);
		//							} finally {
		//								StreamUtils.closeQuietly(os);
		//							}
		//						}
		//					}
		//				}

		//				connection.connect();

		//				final HttpClientResponse clientResponse = new HttpClientResponse(connection);
		//				try {
		//					HttpResponseListener listener = getFromListeners(httpRequest);

		//					if (listener != null) {
		//						listener.handleHttpResponse(clientResponse);
		//					}
		//					removeFromConnectionsAndListeners(httpRequest);
		//				} finally {
		//					connection.disconnect();
		//				}
		//			} catch (final Exception e) {
		//				connection.disconnect();
		//				try {
		//					httpResponseListener.failed(e);
		//				} finally {
		//					removeFromConnectionsAndListeners(httpRequest);
		//				}
		//			}
		//		}
		//	}));
		//} catch (Exception e) {
		//	try {
		//		httpResponseListener.failed(e);
		//	} finally {
		//		removeFromConnectionsAndListeners(httpRequest);
		//	}
		//	return;
		//}
	}

	public void cancelHttpRequest (INet.HttpRequest httpRequest) {
		throw new NotImplementedException();
		//HttpResponseListener httpResponseListener = getFromListeners(httpRequest);

		//if (httpResponseListener != null) {
		//	httpResponseListener.cancelled();
		//	cancelTask(httpRequest);
		//	removeFromConnectionsAndListeners(httpRequest);
		//}
	}

	private void cancelTask (INet.HttpRequest httpRequest) {
		throw new NotImplementedException();
		//Future <?> task = tasks.get(httpRequest);

		//if (task != null) {
		//	task.cancel(false);
		//}
	}

	 void removeFromConnectionsAndListeners (INet. HttpRequest httpRequest) {
		 throw new NotImplementedException();
		//lock (this)
		//{
		//	connections.remove(httpRequest);
		//	listeners.remove(httpRequest);
		//	tasks.remove(httpRequest);
		//}
	}

// TODO:	void putIntoConnectionsAndListeners (final HttpRequest httpRequest,
//		final HttpResponseListener httpResponseListener, final HttpURLConnection connection) {
//			throw new NotImplementedException();
////lock (this)
////			{
////				connections.put(httpRequest, connection);
////				listeners.put(httpRequest, httpResponseListener);
////			}
//		}

	// TODO: synchronized HttpResponseListener getFromListeners (HttpRequest httpRequest) {
	//	HttpResponseListener httpResponseListener = listeners.get(httpRequest);
	//	return httpResponseListener;
	//}
}
