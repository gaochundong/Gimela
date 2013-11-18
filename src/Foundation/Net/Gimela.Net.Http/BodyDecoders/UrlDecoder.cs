using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Gimela.Net.Http.Headers;
using Gimela.Net.Http.Parameters.Parsers;
using Gimela.Text;

namespace Gimela.Net.Http.BodyDecoders
{
  /// <summary>
  /// Decodes URL encoded values.
  /// </summary>
  public class UrlDecoder : IBodyDecoder
  {
    #region IBodyDecoder Members

    /// <summary>
    /// 
    /// </summary>
    /// <param name="stream">Stream containing the content</param>
    /// <param name="contentType">Content type header</param>
    /// <param name="encoding">Stream encoding</param>
    /// <returns>Collection with all parameters.</returns>
    /// <exception cref="FormatException">Body format is invalid for the specified content type.</exception>
    /// <exception cref="InternalServerException">Failed to read all bytes from body stream.</exception>
    public DecodedData Decode(Stream stream, ContentTypeHeader contentType, Encoding encoding)
    {
      if (stream == null || stream.Length == 0)
        return null;

      if (encoding == null)
        encoding = Encoding.UTF8;

      try
      {
        var content = new byte[stream.Length];
        int bytesRead = stream.Read(content, 0, content.Length);
        if (bytesRead != content.Length)
          throw new InternalServerException("Failed to read all bytes from body stream.");

        return new DecodedData { Parameters = ParameterParser.Parse(new BufferReader(content, encoding)) };
      }
      catch (ArgumentException err)
      {
        throw new FormatException(err.Message, err);
      }
    }

    /// <summary>
    /// All content types that the decoder can parse.
    /// </summary>
    /// <returns>A collection of all content types that the decoder can handle.</returns>
    public IEnumerable<string> ContentTypes
    {
      get { return new[] { "application/x-www-form-urlencoded" }; }
    }

    #endregion
  }
}