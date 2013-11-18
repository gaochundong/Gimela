using System;
using System.Collections.Generic;
using Gimela.Common.Logging;

namespace Gimela.Net.Http.Resources
{
  /// <summary>
  /// Provides resources.
  /// </summary>
  public class ResourceProvider : IResourceProvider
  {
    private readonly List<IResourceLoader> _providers = new List<IResourceLoader>();
    private bool _isStarted;

    /// <summary>
    /// Get all view names from a folder.
    /// </summary>
    /// <param name="path">Path to find views in.</param>
    /// <returns>A collection of view names (without path).</returns>
    public IList<string> Find(string path)
    {
      if (path == null)
        return new string[0]; // Spark view engine passes null. So don't throw exception.

      var viewNames = new List<string>();
      foreach (IResourceLoader provider in _providers)
        provider.Find(path, viewNames);

      return viewNames;
    }

    #region IResourceProvider Members

    /// <summary>
    /// Gets number of resource providers
    /// </summary>
    public int Count
    {
      get { return _providers.Count; }
    }

    /// <summary>
    /// Add a new resource loader.
    /// </summary>
    /// <param name="loader">Provider to add.</param>
    /// <exception cref="InvalidOperationException">Manager have been started.</exception>
    public void Add(IResourceLoader loader)
    {
      if (_isStarted)
        throw new InvalidOperationException("Manager have been started.");
      Logger.Trace("Adding resource loader '" + loader.GetType().FullName + "'.");
      _providers.Add(loader);
    }

    /// <summary>
    /// Start manager.
    /// </summary>
    public void Start()
    {
      _isStarted = true;
      Logger.Info("Started.");
    }

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
    public bool Exists(string uriPath)
    {
      foreach (IResourceLoader provider in _providers)
      {
        if (provider.Exists(uriPath))
          return true;
      }

      return false;
    }

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
    public Resource Get(string uri)
    {
      foreach (IResourceLoader provider in _providers)
      {
        Resource resource = provider.Get(uri);
        if (resource != null)
          return resource;
      }

      return null;
    }

    #endregion
  }
}