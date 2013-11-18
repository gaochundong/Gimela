using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Gimela.Net.Http.Headers;

namespace Gimela.Net.Http.Messages
{
	/// <summary>
	/// Create a HTTP response object.
	/// </summary>
	internal class Response : IResponse
	{
		private readonly MemoryStream _body = new MemoryStream();
		private readonly IHttpContext _context;
		private readonly ResponseCookieCollection _cookies = new ResponseCookieCollection();
		private readonly HeaderCollection _headers;
		private ConnectionHeader _connection;
		private NumericHeader _contentLength = new NumericHeader("Content-Length", 0);

		/// <summary>
		/// Initializes a new instance of the <see cref="Response"/> class.
		/// </summary>
		/// <param name="version">HTTP Version.</param>
		/// <param name="code">HTTP status code.</param>
		/// <param name="reason">Why the status code was selected.</param>
		/// <exception cref="FormatException">Version must start with 'HTTP/'</exception>
		public Response(string version, HttpStatusCode code, string reason)
		{
			if (!version.StartsWith("HTTP/"))
				throw new FormatException("Version must start with 'HTTP/'");

			Status = code;
			Reason = reason;
			HttpVersion = version;
			ContentType = new ContentTypeHeader("text/html");
			Encoding = Encoding.UTF8;
			_headers = CreateHeaderCollection();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Response"/> class.
		/// </summary>
		/// <param name="context">Context that the response will be sent through.</param>
		/// <param name="request">Request that the response is for.</param>
		/// <exception cref="FormatException">Version must start with 'HTTP/'</exception>
		public Response(IHttpContext context, IRequest request)
		{
			_context = context;
			HttpVersion = request.HttpVersion;
			Reason = ""; // 可以在此定制
			Status = HttpStatusCode.OK;
			ContentType = new ContentTypeHeader("text/html");
			Encoding = request.Encoding;
			_headers = CreateHeaderCollection();
		}

		/// <summary>
		/// Gets a header.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public IHeader this[string name]
		{
			get { return _headers[name]; }
		}

		private HeaderCollection CreateHeaderCollection()
		{
			HeaderFactory headerFactory = HttpFactory.Current == null
																				? new HeaderFactory()
																				: HttpFactory.Current.Get<HeaderFactory>();
			return new HeaderCollection(headerFactory);
		}

		#region IResponse Members

		/// <summary>
		/// Gets connection type.
		/// </summary>
		public ConnectionHeader Connection
		{
			get
			{
				return _connection ??
							 (HttpVersion == "HTTP/1.0" ? ConnectionHeader.Default10 : ConnectionHeader.Default11);
			}
			set { _connection = value; }
		}

		/// <summary>
		/// Status code that is sent to the client.
		/// </summary>
		/// <remarks>Default is <see cref="HttpStatusCode.OK"/></remarks>
		public HttpStatusCode Status { get; set; }

		/// <summary>
		/// Gets HTTP version.
		/// </summary>
		/// <remarks>
		/// Default is HTTP/1.1
		/// </remarks>
		public string HttpVersion { get; private set; }

		/// <summary>
		/// Information about why a specific status code was used.
		/// </summary>
		public string Reason { get; set; }

		/// <summary>
		/// Size of the body. MUST be specified before sending the header,
		/// unless property Chunked is set to <c>true</c>.
		/// </summary>
		/// <value>
		/// Any specifically assigned value or Body stream length.
		/// </value>
		public NumericHeader ContentLength
		{
			get
			{
				if (Body.Length > 0)
					_contentLength.Value = Body.Length;
				return _contentLength;
			}
			set { _contentLength = value; }
		}

		/// <summary>
		/// Kind of content in the body
		/// </summary>
		/// <remarks>Default is <c>text/html</c></remarks>
		public ContentTypeHeader ContentType { get; set; }

		/// <summary>
		/// Gets or sets encoding
		/// </summary>
		public Encoding Encoding
		{
			get;
			set;
		}

		/// <summary>
		/// Gets cookies.
		/// </summary>
		public ResponseCookieCollection Cookies
		{
			get { return _cookies; }
		}

		/// <summary>
		/// Gets body stream.
		/// </summary>
		public Stream Body
		{
			get { return _body; }
		}

		/// <summary>
		/// Redirect user.
		/// </summary>
		/// <param name="uri">Where to redirect to.</param>
		/// <remarks>
		/// Any modifications after a redirect will be ignored.
		/// </remarks>
		public void Redirect(string uri)
		{
			Status = HttpStatusCode.Redirect;
			_headers["Location"] = new StringHeader("Location", uri);
		}


		/// <summary>
		/// Add a new header.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public void Add(string name, IHeader value)
		{
			string lowerName = name.ToLower();
			if (lowerName == "Content-Length")
				ContentLength = (NumericHeader)value;
			if (lowerName == "Content-Type")
				ContentType = (ContentTypeHeader)value;
			if (lowerName == "Connection")
				Connection = (ConnectionHeader)value;

			_headers.Add(name, value);
		}

		/// <summary>
		/// Add a new header.
		/// </summary>
		/// <param name="header">Header to add.</param>
		public void Add(IHeader header)
		{
			_headers.Add(header.Name, header);
		}

		/// <summary>
		/// Gets headers.
		/// </summary>
		public IHeaderCollection Headers
		{
			get { return _headers; }
		}

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public IEnumerator<IHeader> GetEnumerator()
		{
			return _headers.GetEnumerator();
		}

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}
}