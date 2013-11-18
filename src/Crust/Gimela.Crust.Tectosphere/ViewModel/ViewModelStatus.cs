using System;

namespace Gimela.Crust.Tectosphere
{
  /// <summary>
  /// 在MVVM模式中ViewModel的状态
  /// </summary>
  [Flags]
  public enum ViewModelStatus
  {
    /// <summary>
    /// ViewModel无状态
    /// </summary>
    None = 0x0,
    /// <summary>
    /// ViewModel正在初始化
    /// </summary>
    Initializing = 0x1,
    /// <summary>
    /// ViewModel初始化完毕
    /// </summary>
    Initialized = 0x2,
    /// <summary>
    /// ViewModel正在加载
    /// </summary>
    Loading = 0x4,
    /// <summary>
    /// ViewModel加载完毕
    /// </summary>
    Loaded = 0x8,
    /// <summary>
    /// ViewModel正在保存
    /// </summary>
    Saving = 0x16,
    /// <summary>
    /// ViewModel保存完毕
    /// </summary>
    Saved = 0x32
  }
}
