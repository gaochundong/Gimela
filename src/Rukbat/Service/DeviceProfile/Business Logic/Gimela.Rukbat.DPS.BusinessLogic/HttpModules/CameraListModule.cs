using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Gimela.Data.Json;
using Gimela.Infrastructure.ResourceLocation;
using Gimela.Net.Http;
using Gimela.Net.Http.Headers;
using Gimela.Net.Http.Messages;
using Gimela.Net.Http.Modules;
using Gimela.Rukbat.DPS.BusinessEntities;

namespace Gimela.Rukbat.DPS.BusinessLogic.HttpModules
{
  public class CameraListModule : IModule
  {
    public ProcessingResult Process(RequestContext context)
    {
      // URL format : ~/cameras
      Regex r = new Regex(@"^/cameras$", RegexOptions.IgnoreCase);
      Match m = r.Match(context.Request.Uri.AbsolutePath.ToLowerInvariant());

      if (!m.Success) return ProcessingResult.Continue;

      List<Camera> cameras = Locator.Get<ICameraManager>().GetCameras().ToList();
      string json = JsonConvert.SerializeObject(cameras);

      IRequest request = context.Request;
      IResponse response = context.Response;

      response.ContentType = new ContentTypeHeader("application/json");
      response.ContentLength.Value = json.Length;
      response.Add(new DateHeader("Last-Modified", DateTime.Now.ToUniversalTime()));

      var generator = HttpFactory.Current.Get<ResponseWriter>();
      generator.SendHeaders(context.HttpContext, response);
      generator.SendBody(context.HttpContext, new MemoryStream(Encoding.Default.GetBytes(json)));

      return ProcessingResult.Abort;
    }
  }
}
