using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gimela.Rukbat.DomainModels;

namespace Gimela.Rukbat.GUI.Modules.PublishMedia.Entities
{
  public class PublishPair
  {
    public PublishPair()
    {
    }

    public PublishPair(MediaService service, Camera camera)
      : this()
    {
      Service = service;
      Camera = camera;
    }

    public MediaService Service { get; set; }

    public Camera Camera { get; set; }
  }
}
