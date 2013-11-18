using System;
using Gimela.Common.Cultures;
using Gimela.Presentation.Controls;

namespace Gimela.Rukbat.GUI.Modules.Navigation
{
  public class ResponsiveMDIChild : MDIChild
  {
    public ResponsiveMDIChild()
    {
      CorrelationId = Guid.NewGuid().ToString();
    }

    public string CorrelationId { get; set; }

    public string TitleSource { get; set; }

    public void RefreshTitle()
    {
      Title = LanguageString.Find(TitleSource);
    }
  }
}
