
namespace Gimela.Rukbat.DomainModels
{
  public interface IMediaObject : ISystemObject
  {
    string HostName { get; set; }
    string HostUri { get; set; }
    string Description { get; set; }
    string Tags { get; set; }
    DeviceStatus Status { get; }
  }
}
