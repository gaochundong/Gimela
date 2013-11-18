using System.Collections.Generic;

namespace Gimela.Net.Http.Resources
{
  /// <summary>
  /// Loads resources from a specific location (such as assembly, hard drive etc).
  /// </summary>
  public interface IResourceLoader
  {
    /// <summary>
    /// Checks if a resource exists in the specified directory
    /// </summary>
    /// <param name="uriPath">Uri path to resource</param>
    /// <returns><c>true</c> if resource was found; otherwise <c>false</c>.</returns>
    /// <example>
    /// <code>
    /// if (resources.Exists("/files/user/user.png"))
    ///   Debug.WriteLine("Resource exists.");
    /// </code>
    /// </example>
    bool Exists(string uriPath);

    /// <summary>
    /// Find all views in a folder/path.
    /// </summary>
    /// <param name="path">Absolute Uri path to files that should be found, can end with wild card.</param>
    /// <param name="viewNames">Collection to add all view names to.</param>
    void Find(string path, List<string> viewNames);


    /// <summary>
    /// Gets a resource.
    /// </summary>
    /// <param name="uriPath">Uri path to resource.</param>
    /// <returns>Resource</returns>
    /// <exception cref="ForbiddenException">Uri contains forbidden characters.</exception>
    /// <example>
    /// <code>
    /// Resource resource = resources.Get("/files/user/user.png");
    /// </code>
    /// </example>
    Resource Get(string uriPath);
  }
}