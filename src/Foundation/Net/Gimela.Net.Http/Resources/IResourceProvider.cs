using System;

namespace Gimela.Net.Http.Resources
{
  /// <summary>
  /// Used to access resources.
  /// </summary>
  public interface IResourceProvider
  {
    /// <summary>
    /// Gets number of resource providers
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Add a new resource loader.
    /// </summary>
    /// <param name="loader">Provider to add.</param>
    /// <exception cref="InvalidOperationException">Manager have been started.</exception>
    void Add(IResourceLoader loader);

    /// <summary>
    /// Check if a resource exists.
    /// </summary>
    /// <param name="uriPath">Uri to check</param>
    /// <returns><c>true</c> if found; otherwise <c>false</c>.</returns>
    /// <example>
    /// <code>
    /// if (manager.Exists("/views/user/view.haml"))
    ///   return true
    /// </code>
    /// </example>
    bool Exists(string uriPath);

    /// <summary>
    /// Get a resource.
    /// </summary>
    /// <param name="uri">Uri path to resource.</param>
    /// <returns>Resource if found; otherwise <c>null</c>.</returns>
    /// <example>
    /// <code>
    /// Resource resource = manager.Get("/views/user/view.haml");
    /// </code>
    /// </example>
    Resource Get(string uri);

    /// <summary>
    /// Start manager.
    /// </summary>
    void Start();
  }
}