using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Gimela.Net.Http.Headers;
using Gimela.Net.Http.Messages;
using Gimela.Net.Http.Resources;

namespace Gimela.Net.Http.Modules
{
  /// <summary>
  /// Serves files in the web server.
  /// </summary>
  /// <example>
  /// <code>
  /// FileModule fileModule = new FileModule();
  /// fileModule.Resources.Add(new FileResources("/", "C:\\inetpub\\myweb"));
  /// </code>
  /// </example>
  public class FileModule : IModule
  {
    private readonly Dictionary<string, ContentTypeHeader> _contentTypes =
        new Dictionary<string, ContentTypeHeader>();

    private readonly IResourceProvider _resourceManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileModule"/> class.
    /// </summary>
    /// <exception cref="ArgumentNullException"><c>baseUri</c> or <c>basePath</c> is <c>null</c>.</exception>
    public FileModule()
    {
      _resourceManager = new ResourceProvider();
      AddDefaultMimeTypes();

    }

    /// <summary>
    /// Gets a list with all allowed content types. 
    /// </summary>
    /// <remarks>All other mime types will result in <see cref="HttpStatusCode.Forbidden"/>.</remarks>
    public IDictionary<string, ContentTypeHeader> ContentTypes
    {
      get { return _contentTypes; }
    }

    /// <summary>
    /// Gets provider used to add files to the file manager,
    /// </summary>
    public IResourceProvider Resources
    {
      get { return _resourceManager; }
    }

    /// <summary>
    /// Mime types that this class can handle per default
    /// </summary>
    /// <remarks>
    /// Contains the following mime types:
    /// <list type="table">
    /// <item><term><![CDATA[txt]]></term><value><![CDATA[text/plain]]></value></item>
    /// <item><term><![CDATA[html]]></term><value><![CDATA[text/html]]></value></item>
    /// <item><term><![CDATA[htm]]></term><value><![CDATA[text/html]]></value></item>
    /// <item><term><![CDATA[jpg]]></term><value><![CDATA[image/jpg]]></value></item>
    /// <item><term><![CDATA[jpeg]]></term><value><![CDATA[image/jpg]]></value></item>
    /// <item><term><![CDATA[bmp]]></term><value><![CDATA[image/bmp]]></value></item>
    /// <item><term><![CDATA[gif]]></term><value><![CDATA[image/gif]]></value></item>
    /// <item><term><![CDATA[png]]></term><value><![CDATA[image/png]]></value></item>
    /// <item><term><![CDATA[ico]]></term><value><![CDATA[image/vnd.microsoft.icon]]></value></item>
    /// <item><term><![CDATA[css]]></term><value><![CDATA[text/css]]></value></item>
    /// <item><term><![CDATA[gzip]]></term><value><![CDATA[application/x-gzip]]></value></item>
    /// <item><term><![CDATA[zip]]></term><value><![CDATA[multipart/x-zip]]></value></item>
    /// <item><term><![CDATA[tar]]></term><value><![CDATA[application/x-tar]]></value></item>
    /// <item><term><![CDATA[pdf]]></term><value><![CDATA[application/pdf]]></value></item>
    /// <item><term><![CDATA[rtf]]></term><value><![CDATA[application/rtf]]></value></item>
    /// <item><term><![CDATA[xls]]></term><value><![CDATA[application/vnd.ms-excel]]></value></item>
    /// <item><term><![CDATA[ppt]]></term><value><![CDATA[application/vnd.ms-powerpoint]]></value></item>
    /// <item><term><![CDATA[doc]]></term><value><![CDATA[application/application/msword]]></value></item>
    /// <item><term><![CDATA[js]]></term><value><![CDATA[application/javascript]]></value></item>
    /// <item><term><![CDATA[au]]></term><value><![CDATA[audio/basic]]></value></item>
    /// <item><term><![CDATA[snd]]></term><value><![CDATA[audio/basic]]></value></item>
    /// <item><term><![CDATA[es]]></term><value><![CDATA[audio/echospeech]]></value></item>
    /// <item><term><![CDATA[mp3]]></term><value><![CDATA[audio/mpeg]]></value></item>
    /// <item><term><![CDATA[mp2]]></term><value><![CDATA[audio/mpeg]]></value></item>
    /// <item><term><![CDATA[mid]]></term><value><![CDATA[audio/midi]]></value></item>
    /// <item><term><![CDATA[wav]]></term><value><![CDATA[audio/x-wav]]></value></item>
    /// <item><term><![CDATA[swf]]></term><value><![CDATA[application/x-shockwave-flash]]></value></item>
    /// <item><term><![CDATA[avi]]></term><value><![CDATA[video/avi]]></value></item>
    /// <item><term><![CDATA[rm]]></term><value><![CDATA[audio/x-pn-realaudio]]></value></item>
    /// <item><term><![CDATA[ram]]></term><value><![CDATA[audio/x-pn-realaudio]]></value></item>
    /// <item><term><![CDATA[aif]]></term><value><![CDATA[audio/x-aiff]]></value></item>
    /// </list>
    /// </remarks>
    public void AddDefaultMimeTypes()
    {
      ContentTypes.Add("default", new ContentTypeHeader("application/octet-stream"));
      ContentTypes.Add("txt", new ContentTypeHeader("text/plain"));
      ContentTypes.Add("html", new ContentTypeHeader("text/html"));
      ContentTypes.Add("htm", new ContentTypeHeader("text/html"));
      ContentTypes.Add("jpg", new ContentTypeHeader("image/jpg"));
      ContentTypes.Add("jpeg", new ContentTypeHeader("image/jpg"));
      ContentTypes.Add("bmp", new ContentTypeHeader("image/bmp"));
      ContentTypes.Add("gif", new ContentTypeHeader("image/gif"));
      ContentTypes.Add("png", new ContentTypeHeader("image/png"));

      ContentTypes.Add("ico", new ContentTypeHeader("image/vnd.microsoft.icon"));
      ContentTypes.Add("css", new ContentTypeHeader("text/css"));
      ContentTypes.Add("gzip", new ContentTypeHeader("application/x-gzip"));
      ContentTypes.Add("zip", new ContentTypeHeader("multipart/x-zip"));
      ContentTypes.Add("tar", new ContentTypeHeader("application/x-tar"));
      ContentTypes.Add("pdf", new ContentTypeHeader("application/pdf"));
      ContentTypes.Add("rtf", new ContentTypeHeader("application/rtf"));
      ContentTypes.Add("xls", new ContentTypeHeader("application/vnd.ms-excel"));
      ContentTypes.Add("ppt", new ContentTypeHeader("application/vnd.ms-powerpoint"));
      ContentTypes.Add("doc", new ContentTypeHeader("application/application/msword"));
      ContentTypes.Add("js", new ContentTypeHeader("application/javascript"));
      ContentTypes.Add("au", new ContentTypeHeader("audio/basic"));
      ContentTypes.Add("snd", new ContentTypeHeader("audio/basic"));
      ContentTypes.Add("es", new ContentTypeHeader("audio/echospeech"));
      ContentTypes.Add("mp3", new ContentTypeHeader("audio/mpeg"));
      ContentTypes.Add("mp2", new ContentTypeHeader("audio/mpeg"));
      ContentTypes.Add("mid", new ContentTypeHeader("audio/midi"));
      ContentTypes.Add("wav", new ContentTypeHeader("audio/x-wav"));
      ContentTypes.Add("swf", new ContentTypeHeader("application/x-shockwave-flash"));
      ContentTypes.Add("avi", new ContentTypeHeader("video/avi"));
      ContentTypes.Add("rm", new ContentTypeHeader("audio/x-pn-realaudio"));
      ContentTypes.Add("ram", new ContentTypeHeader("audio/x-pn-realaudio"));
      ContentTypes.Add("aif", new ContentTypeHeader("audio/x-aiff"));
    }


    /// <summary>
    /// Will send a file to client.
    /// </summary>
    /// <param name="context">HTTP context containing outbound stream.</param>
    /// <param name="response">Response containing headers.</param>
    /// <param name="stream">File stream</param>
    private void SendFile(IHttpContext context, IResponse response, Stream stream)
    {
      response.ContentLength.Value = stream.Length;

      var generator = HttpFactory.Current.Get<ResponseWriter>();
      generator.SendHeaders(context, response);
      generator.SendBody(context, stream);
    }

    #region IModule Members

    /// <summary>
    /// Process a request.
    /// </summary>
    /// <param name="context">Request information</param>
    /// <returns>What to do next.</returns>
    /// <exception cref="InternalServerException">Failed to find file extension</exception>
    /// <exception cref="ForbiddenException">Forbidden file type.</exception>
    public ProcessingResult Process(RequestContext context)
    {
      Resource resource = _resourceManager.Get(context.Request.Uri.LocalPath);
      if (resource == null)
        return ProcessingResult.Continue;

      IRequest request = context.Request;
      IResponse response = context.Response;
      try
      {
        string fileExtension = Path.GetExtension(request.Uri.AbsolutePath).TrimStart('.');

        ContentTypeHeader header;
        if (!ContentTypes.TryGetValue(fileExtension, out header))
          return ProcessingResult.Continue;

        response.ContentType = header;

        // Only send file if it has not been modified.
        var browserCacheDate = request.Headers["If-Modified-Since"] as DateHeader;
        if (browserCacheDate != null)
        {
          DateTime since = browserCacheDate.Value.ToUniversalTime();
          DateTime modified = resource.ModifiedAt;

          // Allow for file systems with subsecond time stamps
          modified = new DateTime(modified.Year, modified.Month, modified.Day, modified.Hour, modified.Minute, modified.Second, modified.Kind);
          if (since >= modified)
          {
            response.Status = HttpStatusCode.NotModified;
            return ProcessingResult.SendResponse;
          }
        }

        using (resource.Stream)
        {
          response.Add(new DateHeader("Last-Modified", resource.ModifiedAt));

          // Send response and tell server to do nothing more with the request.
          SendFile(context.HttpContext, response, resource.Stream);
          return ProcessingResult.Abort;
        }
      }
      catch (FileNotFoundException err)
      {
        throw new InternalServerException("Failed to process file '" + request.Uri + "'.", err);
      }
    }

    #endregion
  }
}