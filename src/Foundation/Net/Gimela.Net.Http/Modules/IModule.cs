namespace Gimela.Net.Http.Modules
{
  /// <summary>
  /// HTTP Module
  /// </summary>
  public interface IModule
  {
    /// <summary>
    /// Process a request.
    /// </summary>
    /// <param name="context">Request information</param>
    /// <returns>What to do next.</returns>
    ProcessingResult Process(RequestContext context);
  }
}