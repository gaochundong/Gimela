using System.Collections.Generic;
using System.IO;
using System.Text;
using Gimela.Net.Http.Headers;

namespace Gimela.Net.Http
{
	/// <summary>
	/// Base interface for request and response.
	/// </summary>
	public interface IMessage : IEnumerable<IHeader>
	{
		/// <summary>
		/// Gets body stream.
		/// </summary>
		Stream Body { get; }

		/// <summary>
		/// Size of the body. MUST be specified before sending the header,
		/// unless property Chunked is set to <c>true</c>.
		/// </summary>
		NumericHeader ContentLength { get; }

		/// <summary>
		/// Kind of content in the body
		/// </summary>
		/// <remarks>Default is <c>text/html</c></remarks>
		ContentTypeHeader ContentType { get; }

		/// <summary>
		/// Gets or sets encoding
		/// </summary>
		Encoding Encoding { get; set; }

		/// <summary>
		/// Gets headers.
		/// </summary>
		IHeaderCollection Headers { get; }

		/// <summary>
		/// Add a new header.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		void Add(string name, IHeader value);

		/// <summary>
		/// Add a new header.
		/// </summary>
		/// <param name="header">Header to add.</param>
		void Add(IHeader header);
	}
}