using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using Gimela.Net.Http.BodyDecoders.Mono;
using Gimela.Net.Http.Headers;

namespace Gimela.Net.Http.BodyDecoders
{
  /// <summary>
  /// Decodes forms that have multiple sections.
  /// </summary>
  /// <remarks>
  /// http://www.faqs.org/rfcs/rfc1867.html
  /// </remarks>
  public class MultiPartDecoder : IBodyDecoder
  {
    /// <summary>
    /// form-data
    /// </summary>
    public const string FormData = "form-data";

    /// <summary>
    /// multipart/form-data
    /// </summary>
    public const string MimeType = "multipart/form-data";

    #region IBodyDecoder Members

    /// <summary>
    /// Decode body stream
    /// </summary>
    /// <param name="stream">Stream containing the content</param>
    /// <param name="contentType">Content type header</param>
    /// <param name="encoding">Stream encoding</param>
    /// <returns>Decoded data.</returns>
    /// <exception cref="FormatException">Body format is invalid for the specified content type.</exception>
    /// <exception cref="InternalServerException">Something unexpected failed.</exception>
    /// <exception cref="ArgumentNullException"><c>stream</c> is <c>null</c>.</exception>
    public DecodedData Decode(Stream stream, ContentTypeHeader contentType, Encoding encoding)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (contentType == null)
        throw new ArgumentNullException("contentType");
      if (encoding == null)
        throw new ArgumentNullException("encoding");

      //multipart/form-data, boundary=AaB03x
      string boundry = contentType.Parameters["boundary"];
      if (boundry == null)
        throw new FormatException("Missing boundary in content type.");

      var multipart = new HttpMultipart(stream, boundry, encoding);

      var form = new DecodedData();
      /*
      FileStream stream1 = new FileStream("C:\\temp\\mimebody.tmp", FileMode.Create);
      byte[] bytes = new byte[stream.Length];
      stream.Read(bytes, 0, bytes.Length);
      stream1.Write(bytes, 0, bytes.Length);
      stream1.Flush();
      stream1.Close();
      */

      HttpMultipart.Element element;
      while ((element = multipart.ReadNextElement()) != null)
      {
        if (string.IsNullOrEmpty(element.Name))
          throw new FormatException("Error parsing request. Missing value name.\nElement: " + element);

        if (!string.IsNullOrEmpty(element.Filename))
        {
          if (string.IsNullOrEmpty(element.ContentType))
            throw new FormatException("Error parsing request. Value '" + element.Name +
                                      "' lacks a content type.");

          // Read the file data
          var buffer = new byte[element.Length];
          stream.Seek(element.Start, SeekOrigin.Begin);
          stream.Read(buffer, 0, (int)element.Length);

          // Generate a filename
          string originalFileName = element.Filename;
          string internetCache = Environment.GetFolderPath(Environment.SpecialFolder.InternetCache);

          // if the internet path doesn't exist, assume mono and /var/tmp
          string path = string.IsNullOrEmpty(internetCache)
                            ? Path.Combine("var", "tmp")
                            : Path.Combine(internetCache.Replace("\\\\", "\\"), "tmp");

          element.Filename = Path.Combine(path, Math.Abs(element.Filename.GetHashCode()) + ".tmp");

          // If the file exists generate a new filename
          while (File.Exists(element.Filename))
            element.Filename = Path.Combine(path, Math.Abs(element.Filename.GetHashCode() + 1) + ".tmp");

          if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

          File.WriteAllBytes(element.Filename, buffer);

          var file = new HttpFile
                         {
                           Name = element.Name,
                           OriginalFileName = originalFileName,
                           ContentType = element.ContentType,
                           TempFileName = element.Filename
                         };
          form.Files.Add(file);
        }
        else
        {
          var buffer = new byte[element.Length];
          stream.Seek(element.Start, SeekOrigin.Begin);
          stream.Read(buffer, 0, (int)element.Length);

          form.Parameters.Add(HttpUtility.UrlDecode(element.Name), encoding.GetString(buffer));
        }
      }

      return form;
    }


    /// <summary>
    /// All content types that the decoder can parse.
    /// </summary>
    /// <returns>A collection of all content types that the decoder can handle.</returns>
    public IEnumerable<string> ContentTypes
    {
      get { return new[] { MimeType, FormData }; }
    }

    #endregion
  }
}