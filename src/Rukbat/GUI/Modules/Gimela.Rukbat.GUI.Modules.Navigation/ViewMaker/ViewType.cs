using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gimela.Rukbat.GUI.Modules.Navigation
{
  /// <summary>
  /// 显示View类型
  /// </summary>
  internal enum ViewType
  {
    /// <summary>
    /// 默认
    /// </summary>
    None = 0,
    /// <summary>
    /// 设备管理
    /// </summary>
    DeviceConfiguration,
    /// <summary>
    /// 实时视频摄像机列表
    /// </summary>
    LiveVideoCameraList,
    /// <summary>
    /// 实时视频
    /// </summary>
    LiveVideo,
    /// <summary>
    /// 媒体发布
    /// </summary>
    PublishMedia,
    /// <summary>
    /// 皮肤
    /// </summary>
    SkinConfiguration,
    /// <summary>
    /// 关于
    /// </summary>
    Abort,
  }
}
