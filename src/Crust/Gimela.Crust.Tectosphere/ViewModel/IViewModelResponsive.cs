
namespace Gimela.Crust.Tectosphere
{
  public interface IViewModelResponsive
  {
    void Refresh();

    ViewModelStatus Status { get; }
  }
}
