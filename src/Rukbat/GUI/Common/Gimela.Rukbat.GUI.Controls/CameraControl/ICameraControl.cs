using Gimela.Rukbat.DomainModels;

namespace Gimela.Rukbat.GUI.Controls
{
  public interface ICameraControl
  {
    ICamera BindingCamera { get; }

    void AddCameraBinding(ICamera camera);

    void RemoveCameraBinding();
  }
}
