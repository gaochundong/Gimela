using Gimela.Data.Json;

namespace Gimela.Rukbat.DomainModels
{
  public static class CameraFactory
  {
    public static Camera CreateCamera()
    {
      Camera camera = new Camera();
      camera.Id = ObjectId.NewObjectId();

      return camera;
    }
  }
}
