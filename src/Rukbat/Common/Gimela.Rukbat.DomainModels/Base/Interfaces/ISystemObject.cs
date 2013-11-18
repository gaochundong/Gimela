using System.Windows.Media;

namespace Gimela.Rukbat.DomainModels
{
  public interface ISystemObject : IBaseObject
  {
    ImageSource ObjectImage { get; set; }
    void RefreshObjectImage();
  }
}
