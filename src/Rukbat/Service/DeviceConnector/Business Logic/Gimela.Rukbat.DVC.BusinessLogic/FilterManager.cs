using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Gimela.Common.ExceptionHandling;
using Gimela.Media.Video.DirectShow;
using Gimela.Rukbat.DVC.BusinessEntities;

namespace Gimela.Rukbat.DVC.BusinessLogic
{
  public class FilterManager : IFilterManager
  {
    private bool _disposed = false;
    private IList<CameraFilter> _cameraFilters;
    private IList<DesktopFilter> _desktopFilters;
    private System.Threading.Timer _adjustFilterTimer = null;

    public FilterManager()
    {
      _cameraFilters = GetCameraFilters(); // 获取所有本地USB摄像机
      _desktopFilters = GetDesktopFilters(); // 获取所有本地桌面屏幕

      _adjustFilterTimer = new System.Threading.Timer(OnAdjustFilterTimer, null, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
    }

    #region IFilterManager Members

    public event EventHandler<FilterRemovedEventArgs> FilterRemoved;

    public bool IsFilterExist(FilterType filterType, string filterId)
    {
      bool isExist = false;

      switch (filterType)
      {
        case FilterType.LocalCamera:
          isExist = GetCameraFilters().FirstOrDefault(f => f.Id == filterId) != null;
          break;
        case FilterType.LocalDesktop:
          isExist = GetDesktopFilters().FirstOrDefault(f => f.Id == filterId) != null;
          break;
      }

      return isExist;
    }

    public IList<CameraFilter> GetCameraFilters()
    {
      List<CameraFilter> filters = new List<CameraFilter>();

      try
      {
        FilterInfoCollection localFilters = new FilterInfoCollection(FilterCategory.VideoInputDevice);
        for (int i = 0; i < localFilters.Count; i++)
        {
          CameraFilter filter = new CameraFilter(localFilters[i].MonikerString) { Name = localFilters[i].Name };
          filters.Add(filter);
        }
      }
      catch (ApplicationException ex)
      {
        ExceptionHandler.Handle(ex);
      }

      return filters;
    }

    public IList<DesktopFilter> GetDesktopFilters()
    {
      List<DesktopFilter> filters = new List<DesktopFilter>();

      try
      {
        Screen[] screens = Screen.AllScreens;
        for (int i = 0; i < screens.Length; i++)
        {
          DesktopFilter filter = new DesktopFilter(i) { Name = screens[i].DeviceName };
          filter.IsPrimary = screens[i].Primary;
          filter.Bounds = screens[i].Bounds;
          filters.Add(filter);
        }
      }
      catch (ApplicationException ex)
      {
        ExceptionHandler.Handle(ex);
      }

      return filters;
    }

    #endregion

    private void OnAdjustFilterTimer(object state)
    {
      AdjustFilter();
    }

    private void AdjustFilter()
    {
      IList<CameraFilter> cameraFilters = GetCameraFilters();
      IList<DesktopFilter> desktopFilters = GetDesktopFilters();

      foreach (var item in _cameraFilters)
      {
        if (cameraFilters.Select(c => c.Id == item.Id).Count() <= 0)
        {
          RaiseFilterRemoved(FilterType.LocalCamera, item.Id);
        }
      }
      foreach (var item in _desktopFilters)
      {
        if (desktopFilters.Select(c => c.Id == item.Id).Count() <= 0)
        {
          RaiseFilterRemoved(FilterType.LocalDesktop, item.Id);
        }
      }

      _cameraFilters = cameraFilters;
      _desktopFilters = desktopFilters;
    }

    private void RaiseFilterRemoved(FilterType filterType, string filterId)
    {
      if (FilterRemoved != null)
      {
        FilterRemoved(this, new FilterRemovedEventArgs(filterType, filterId));
      }
    }

    #region IDisposable Members

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!this._disposed)
      {
        if (disposing)
        {
          if (_adjustFilterTimer != null)
          {
            _adjustFilterTimer.Change(Timeout.Infinite, Timeout.Infinite);
            _adjustFilterTimer.Dispose();
            _adjustFilterTimer = null;
          }
        }

        _disposed = true;
      }
    }

    #endregion
  }
}
