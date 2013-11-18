using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Gimela.Data.Json;
using Gimela.Net.Http;
using Gimela.Net.Http.Headers;
using Gimela.Net.Http.Messages;
using Gimela.Net.Http.Modules;

namespace TestHttpServer.Modules
{
  public class HomeModule : IModule
  {
    public ProcessingResult Process(RequestContext context)
    {
      // URL format : ~/login
      Regex r = new Regex(@"^/login$", RegexOptions.IgnoreCase);
      Match m = r.Match(context.Request.Uri.AbsolutePath.ToLowerInvariant());

      if (!m.Success) return ProcessingResult.Continue;

      IRequest request = context.Request;
      IResponse response = context.Response;

      var assembly = Assembly.GetExecutingAssembly();
      var resourceName = Assembly.GetExecutingAssembly().GetName().Name + @".html.cameras.html";

      string html;
      using (Stream stream = assembly.GetManifestResourceStream(resourceName))
      using (StreamReader reader = new StreamReader(stream))
      {
        html = reader.ReadToEnd();
      }

      response.ContentType = new ContentTypeHeader("txt/html");
      response.ContentLength.Value = html.Length;
      response.Add(new DateHeader("Last-Modified", DateTime.Now.ToUniversalTime()));

      var generator = HttpFactory.Current.Get<ResponseWriter>();
      generator.SendHeaders(context.HttpContext, response);
      generator.SendBody(context.HttpContext, new MemoryStream(Encoding.Default.GetBytes(html)));

      return ProcessingResult.Abort;
    }
  }
}
