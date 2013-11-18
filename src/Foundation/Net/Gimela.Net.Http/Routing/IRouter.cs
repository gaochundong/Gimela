using System;

namespace Gimela.Net.Http.Routing
{
  /// <summary>
  /// Rules are used to perform operations before a request is being handled.
  /// Rules can be used to create routing etc.
  /// </summary>
  public interface IRouter
  {
    /// <summary>
    /// Process the incoming request.
    /// </summary>
    /// <param name="context">Request context information.</param>
    /// <returns>Processing result.</returns>
    /// <exception cref="ArgumentNullException">If any parameter is <c>null</c>.</exception>
    ProcessingResult Process(RequestContext context);
  }
}