using SharpGDX.Utils;
using SharpGDX.Shims;
using System.Text;

namespace SharpGDX.Net;

/** Provides utility methods to work with the {@link HttpRequest} content and parameters. */
public sealed class HttpParametersUtils {

	private HttpParametersUtils () {
	}

	public static String defaultEncoding = "UTF-8";
	public static String nameValueSeparator = "=";
	public static String parameterSeparator = "&";

	/** Useful method to convert a map of key,value pairs to a String to be used as part of a GET or POST content.
	 * @param parameters A Map<String, String> with the parameters to encode.
	 * @return The String with the parameters encoded. */
	public static String convertHttpParameters (Map<String, String> parameters) {
		var keySet = parameters.keySet();
		StringBuilder convertedParameters = new StringBuilder();
		foreach (String name in keySet) {
			convertedParameters.Append(encode(name, defaultEncoding));
			convertedParameters.Append(nameValueSeparator);
			convertedParameters.Append(encode(parameters.get(name), defaultEncoding));
			convertedParameters.Append(parameterSeparator);
		}

		if (convertedParameters.Length > 0)
		{
			throw new NotImplementedException();
			// TODO: convertedParameters.deleteCharAt(convertedParameters.Length - 1);
		}
		return convertedParameters.ToString();
	}

	private static String encode (String content, String encoding) {
		try
		{
			throw new NotImplementedException();
			// TODO: return URLEncoder.encode(content, encoding);
		} catch (UnsupportedEncodingException e) {
			throw new NotImplementedException();
			// TODO: throw new IllegalArgumentException(e);
		}
	}
}
