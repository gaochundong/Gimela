using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Gimela.Net.Http.Headers;

namespace Gimela.Net.Http.BodyDecoders
{
  /// <summary>
  /// Collection of body decoders.
  /// </summary>
  /// <remarks>
  /// Body decoders are used to parse request body and convert it
  /// into a <see cref="HttpFileCollection"/> and a <see cref="ParameterCollection"/>.
  /// </remarks>
  public class BodyDecoderCollection : IEnumerable<IBodyDecoder>
  {
    private readonly Dictionary<string, IBodyDecoder> _decoders = new Dictionary<string, IBodyDecoder>();

    /// <summary>
    /// Gets number of decoders.
    /// </summary>
    public int Count
    {
      get { return _decoders.Values.Count; }
    }

    /// <summary>
    /// Add another body decoder.
    /// </summary>
    /// <param name="decoder"></param>
    public void Add(IBodyDecoder decoder)
    {
      foreach (string contentType in decoder.ContentTypes)
        _decoders.Add(contentType, decoder);
    }

    /// <summary>
    /// Decode body stream
    /// </summary>
    /// <param name="stream">Stream containing the content</param>
    /// <param name="contentType">Content type header</param>
    /// <param name="encoding">Stream encoding</param>
    /// <returns>Decoded data.</returns>
    /// <exception cref="FormatException">Body format is invalid for the specified content type.</exception>
    /// <exception cref="InternalServerException">Something unexpected failed.</exception>
    public DecodedData Decode(Stream stream, ContentTypeHeader contentType, Encoding encoding)
    {
      IBodyDecoder decoder;
      if (contentType == null || contentType.Value == "")
        contentType = new ContentTypeHeader("application/octet-stream");

      return !_decoders.TryGetValue(contentType.Value, out decoder)
                 ? null
                 : decoder.Decode(stream, contentType, encoding);
    }

    #region IEnumerable<IBodyDecoder> Members

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public IEnumerator<IBodyDecoder> GetEnumerator()
    {
      return _decoders.Values.GetEnumerator();
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