using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Gimela.Net.Http.Headers;

namespace Gimela.Net.Http.BodyDecoders
{
  /// <summary>
  /// Decodes body stream.
  /// </summary>
  public interface IBodyDecoder
  {
    /// <summary>
    /// All content types that the decoder can parse.
    /// </summary>
    /// <returns>A collection of all content types that the decoder can handle.</returns>
    IEnumerable<string> ContentTypes { get; }

    /// <summary>
    /// Decode body stream
    /// </summary>
    /// <param name="stream">Stream containing the content</param>
    /// <param name="contentType">Content type header</param>
    /// <param name="encoding">Stream encoding</param>
    /// <returns>Decoded data.</returns>
    /// <exception cref="FormatException">Body format is invalid for the specified content type.</exception>
    /// <exception cref="InternalServerException">Something unexpected failed.</exception>
    DecodedData Decode(Stream stream, ContentTypeHeader contentType, Encoding encoding);
  }
}