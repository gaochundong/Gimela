using System.Windows.Controls;

namespace Gimela.Presentation.Windows
{
  /// <summary>
  /// 可包含子控件的接口
  /// </summary>
  public interface IControlContainable
  {
    /// <summary>
    /// 控件容器Panel
    /// </summary>
    Panel Container { get; }

    /// <summary>
    /// 控件菜单
    /// </summary>
    Menu Menu { get; }
  }
}
