using System;
using System.IO;
using System.Text;
using Gimela.Common.ExceptionHandling;
using Gimela.Net.Http.Headers;
using Gimela.Common.Logging;

namespace Gimela.Net.Http.Messages
{
  /// <summary>
  /// Used to send a response back to the client.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Writes a <see cref="IResponse"/> object into a stream.
  /// </para>
  /// <para>
  /// Important! ResponseWriter do not throw any exceptions. Instead it just logs them and
  /// let them die peacefully. This is since the response writer is used from
  /// catch blocks here and there.
  /// </para>
  /// </remarks>
  public class ResponseWriter
  {
    /// <summary>
    /// Sends response using the specified context.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="response">The response.</param>
    public void Send(IHttpContext context, IResponse response)
    {
      SendHeaders(context, response);
      SendBody(context, response.Body);

      try
      {
        context.Stream.Flush();
      }
      catch (Exception err)
      {
        Logger.Error("Failed to flush context stream.");
        ExceptionHandler.Handle(err);
      }
    }

    /// <summary>
    /// Converts and sends a string.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="data"></param>
    /// <param name="encoding">Encoding used to transfer string</param>
    public void Send(IHttpContext context, string data, Encoding encoding)
    {
      try
      {
        byte[] buffer = encoding.GetBytes(data);
        Logger.Debug("Sending " + buffer.Length + " bytes.");
        if (data.Length < 4000)
          Logger.Trace(data);
        context.Stream.Write(buffer, 0, buffer.Length);
      }
      catch (Exception err)
      {
        Logger.Error("Failed to send data through context stream.");
        ExceptionHandler.Handle(err);
      }
    }

    /// <summary>
    /// Send a body to the client
    /// </summary>
    /// <param name="context">Context containing the stream to use.</param>
    /// <param name="body">Body to send</param>
    public void SendBody(IHttpContext context, Stream body)
    {
      try
      {
        body.Flush();
        body.Seek(0, SeekOrigin.Begin);
        var buffer = new byte[4196];
        int bytesRead = body.Read(buffer, 0, 4196);
        while (bytesRead > 0)
        {
          context.Stream.Write(buffer, 0, bytesRead);
          bytesRead = body.Read(buffer, 0, 4196);
        }
      }
      catch (Exception err)
      {
        Logger.Error("Failed to send body through context stream.");
        ExceptionHandler.Handle(err);
      }
    }

    /// <summary>
    /// Send all headers to the client
    /// </summary>
    /// <param name="response">Response containing call headers.</param>
    /// <param name="context">Content used to send headers.</param>
    public void SendHeaders(IHttpContext context, IResponse response)
    {
      var sb = new StringBuilder();
      sb.AppendFormat("{0} {1} {2}\r\n", response.HttpVersion, (int)response.Status, response.Reason);

      // replace content-type name with the actual one used.
      //response.ContentType.Parameters["charset"] = response.Encoding.WebName;

      // go through all property headers.
      sb.AppendFormat("{0}: {1}\r\n", response.ContentType.Name, response.ContentType);
      sb.AppendFormat("{0}: {1}\r\n", response.ContentLength.Name, response.ContentLength);
      sb.AppendFormat("{0}: {1}\r\n", response.Connection.Name, response.Connection);

      if (response.Cookies != null && response.Cookies.Count > 0)
      {
        //Set-Cookie: <name>=<value>[; <name>=<value>][; expires=<date>][; domain=<domain_name>][; path=<some_path>][; secure][; httponly]
        foreach (ResponseCookie cookie in response.Cookies)
        {
          sb.Append("Set-Cookie: ");
          sb.Append(cookie.Name);
          sb.Append("=");
          sb.Append(cookie.Value ?? string.Empty);

          if (cookie.Expires > DateTime.MinValue)
            sb.Append(";expires=" + cookie.Expires.ToString("R"));
          if (!string.IsNullOrEmpty(cookie.Path))
            sb.AppendFormat(";path={0}", cookie.Path);
          sb.Append("\r\n");
        }
      }

      foreach (IHeader header in response)
        sb.AppendFormat("{0}: {1}\r\n", header.Name, header);

      sb.Append("\r\n");
      Send(context, sb.ToString(), response.Encoding);
      HeadersSent(this, EventArgs.Empty);
    }

    /// <summary>
    /// Sends the error page.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="response">The response.</param>
    /// <param name="exception">The exception.</param>
    public void SendErrorPage(IHttpContext context, IResponse response, Exception exception)
    {
      string htmlTemplate = @"<html>
		<head>
				<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" />
				<title>{1}</title>
		</head>
		<body>
				<h1>{0} - {1}</h1>
				<pre>{2}</pre>
		</body>
</html>";

      var body = string.Format(htmlTemplate,
                               (int)response.Status,
                               response.Reason,
                               exception);
      byte[] bodyBytes = response.Encoding.GetBytes(body);
      response.Body.Write(bodyBytes, 0, bodyBytes.Length);
      Send(context, response);
    }

    /// <summary>
    /// Header sent event.
    /// </summary>
    public static event EventHandler HeadersSent = delegate { };
  }
}