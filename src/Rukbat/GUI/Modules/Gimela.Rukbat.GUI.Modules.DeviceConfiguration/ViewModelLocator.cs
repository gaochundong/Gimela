using Gimela.Infrastructure.Patterns;
using Gimela.Rukbat.GUI.Modules.DeviceConfiguration.Models;
using Gimela.Rukbat.GUI.Modules.DeviceConfiguration.ViewModels;
using Gimela.Rukbat.DomainModels;

namespace Gimela.Rukbat.GUI.Modules.DeviceConfiguration
{
  internal static class ViewModelLocator
  {
    public static DeviceConnectorServiceClient ServiceClient
    {
      get
      {
        return Singleton<DeviceConnectorServiceClient>.Instance;
      }
    }

    public static MediaService SelectedService { get; set; }

    public static ServiceSelectionViewModel ServiceSelection
    {
      get
      {
        return new ServiceSelectionViewModel(Singleton<Models.ServiceModel>.Instance);
      }
    }

    public static CameraManagementViewModel CameraManagement
    {
      get
      {
        return new CameraManagementViewModel(Singleton<CameraModel>.Instance);
      }
    }

    public static CameraCreationViewModel CameraCreation
    {
      get
      {
        return new CameraCreationViewModel(Singleton<CameraModel>.Instance, Singleton<CameraFilterModel>.Instance, Singleton<DesktopFilterModel>.Instance);
      }
    }

    public static CameraUpdateViewModel CameraUpdate
    {
      get
      {
        return new CameraUpdateViewModel(Singleton<CameraModel>.Instance, Singleton<CameraFilterModel>.Instance, Singleton<DesktopFilterModel>.Instance);
      }
    }

    public static VideoSourceNavigationViewModel VideoSourceNavigation
    {
      get
      {
        return new VideoSourceNavigationViewModel(Singleton<CameraFilterModel>.Instance, Singleton<DesktopFilterModel>.Instance);
      }
    }

    public static LocalCameraVideoSourceViewModel LocalCameraVideoSource
    {
      get
      {
        return new LocalCameraVideoSourceViewModel();
      }
    }

    public static LocalAVIFileVideoSourceViewModel LocalAVIFileVideoSource
    {
      get
      {
        return new LocalAVIFileVideoSourceViewModel();
      }
    }

    public static LocalDesktopVideoSourceViewModel LocalDesktopVideoSource
    {
      get
      {
        return new LocalDesktopVideoSourceViewModel();
      }
    }

    public static NetworkJPEGVideoSourceViewModel NetworkJPEGVideoSource
    {
      get
      {
        return new NetworkJPEGVideoSourceViewModel();
      }
    }

    public static NetworkMJPEGVideoSourceViewModel NetworkMJPEGVideoSource
    {
      get
      {
        return new NetworkMJPEGVideoSourceViewModel();
      }
    }
  }
}
