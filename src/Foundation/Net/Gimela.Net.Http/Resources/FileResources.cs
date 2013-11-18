using System;
using System.Collections.Generic;
using System.IO;
using System.Security;

namespace Gimela.Net.Http.Resources
{
  /// <summary>
  /// Load resources from disk.
  /// </summary>
  public class FileResources : IResourceLoader
  {
    /// <summary>
    /// Default forbidden characters.
    /// </summary>
    public static readonly string[] DefaultForbiddenChars = new[] { "..", ":" };

    private static readonly string PathSeparator = Path.DirectorySeparatorChar.ToString();

    private readonly List<Mapping> _mappings = new List<Mapping>();

    /// <summary>
    /// relative to absolute path mappings.
    /// </summary>
    private Dictionary<string, string> _paths = new Dictionary<string, string>();

    /// <summary>
    /// Initializes a new instance of the <see cref="FileResources"/> class.
    /// </summary>
    public FileResources()
    {
      ForbiddenCharacters = DefaultForbiddenChars;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileResources"/> class.
    /// </summary>
    /// <param name="uri">Request URI path</param>
    /// <param name="absolutePath">Disk path</param>
    /// <remarks>
    /// File names should not be included in URI or path.
    /// </remarks>
    /// <example>
    /// <code>
    /// new FileResources("/files/user/", "C:\\intetpub\\files\users\\");
    /// </code>
    /// </example>
    public FileResources(string uri, string absolutePath)
    {
      ForbiddenCharacters = DefaultForbiddenChars;
      Add(uri, absolutePath);
    }

    /// <summary>
    /// Gets or sets forbidden characters.
    /// </summary>
    /// <remarks>
    /// Used to revoke access to any system files.
    /// </remarks>
    public string[] ForbiddenCharacters { get; set; }

    /// <summary>
    /// Add a new resource mapping.
    /// </summary>
    /// <param name="uri">Request URI path</param>
    /// <param name="absolutePath">Disk path</param>
    /// <remarks>
    /// File names should not be included in URI or path.
    /// </remarks>
    /// <example>
    /// <code>
    /// resources.Add("/files/", "C:\\intetpub\\files\\");
    /// </code>
    /// </example>
    /// <exception cref="DirectoryNotFoundException"><c>absolutePath</c> is not found.</exception>
    public void Add(string uri, string absolutePath)
    {
      if (!absolutePath.EndsWith(PathSeparator))
        absolutePath += PathSeparator;

      if (!Directory.Exists(absolutePath))
        throw new DirectoryNotFoundException(absolutePath);

      if (!uri.EndsWith("/"))
        uri += "/";
      string relativePath = uri.Replace('/', Path.PathSeparator);
      _mappings.Add(new Mapping { AbsolutePath = absolutePath, UriPath = uri, RelativePath = relativePath });
    }

    /// <summary>
    /// check if source contains any of the chars.
    /// </summary>
    /// <param name="source">string to check</param>
    /// <param name="chars">Characters to fined</param>
    /// <returns></returns>
    private static bool Contains(string source, IEnumerable<string> chars)
    {
      foreach (string s in chars)
      {
        if (source.Contains(s))
          return true;
      }

      return false;
    }

    /// <summary>
    /// Finds the files.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    /// <param name="searchPattern">The search pattern.</param>
    /// <param name="viewNames">The view names.</param>
    public void FindFiles(string filePath, string searchPattern, List<string> viewNames)
    {
      string[] files = Directory.GetFiles(filePath, searchPattern);
      foreach (string file in files)
        viewNames.Add(Path.GetFileName(file));
    }

    /// <summary>
    /// Go through all mappings and find requested Uri.
    /// </summary>
    /// <param name="uriPath">Uri to get local path for.</param>
    /// <returns>Path if found; otherwise <c>null</c>.</returns>
    private string GetFullFilePath(string uriPath)
    {
      int pos = uriPath.LastIndexOf('/');
      if (pos == -1)
        return null;
      string path = uriPath.Substring(0, pos + 1);
      string fileName = uriPath.Substring(pos + 1);

      foreach (Mapping mapping in _mappings)
      {
        if (!path.StartsWith(mapping.UriPath)) continue;
        path = path.Remove(0, mapping.UriPath.Length);
        path = path.Replace("/", PathSeparator);
        return mapping.AbsolutePath + path + fileName;
      }

      return null;
    }

    #region IResourceLoader Members

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
    public bool Exists(string uriPath)
    {
      if (Contains(uriPath, ForbiddenCharacters))
        return false;

      string filePath = GetFullFilePath(uriPath);

      return filePath != null
             && File.Exists(filePath)
             && (File.GetAttributes(filePath) & FileAttributes.ReparsePoint) == 0; // not a symlink
    }

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
    public Resource Get(string uriPath)
    {
      string filePath = GetFullFilePath(uriPath);
      if (filePath == null)
        return null;

      if (Contains(uriPath, ForbiddenCharacters))
        throw new ForbiddenException("Uri contains forbidden characters.");

      try
      {
        if (!File.Exists(filePath))
          return null;

        var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        return new Resource
                   {
                     ModifiedAt = File.GetLastWriteTime(filePath).ToUniversalTime(),
                     Stream = fileStream
                   };
      }
      catch (FileNotFoundException)
      {
        return null;
      }
      catch (IOException)
      {
        return null;
      }
      catch (SecurityException)
      {
        return null;
      }
      catch (UnauthorizedAccessException)
      {
        return null;
      }
    }

    /// <summary>
    /// Find all views in a folder/path.
    /// </summary>
    /// <param name="path">Absolute Uri path to files that should be found, can end with wild card.</param>
    /// <param name="viewNames">Collection to add all view names to.</param>
    /// <exception cref="ForbiddenException">Uri contains forbidden characters.</exception>
    /// <example>
    /// Find("
    /// </example>
    public void Find(string path, List<string> viewNames)
    {
      if (path.EndsWith("*"))
      {
        FindByWildCard(path, viewNames);
        return;
      }
      if (!path.EndsWith("/"))
        path += "/";

      if (Contains(path, ForbiddenCharacters))
        throw new ForbiddenException("Uri contains forbidden characters.");

      string diskPath = path.Replace('/', Path.PathSeparator).Remove(0, 1);

      foreach (Mapping mapping in _mappings)
      {
        // same directory
        if (mapping.UriPath == path)
        {
          FindFiles(mapping.AbsolutePath, "*.*", viewNames);
          return;
        }

        if (!path.StartsWith(mapping.UriPath))
          continue;

        // sub diretory?
        if (Directory.Exists(mapping.AbsolutePath + diskPath))
        {
          FindFiles(mapping.AbsolutePath + diskPath, "*.*", viewNames);
          return;
        }
      }
    }

    private void FindByWildCard(string path, List<string> viewNames)
    {
      string fileName = Path.GetFileName(path);
      path = path.Remove(path.Length - fileName.Length, fileName.Length);
      if (!path.EndsWith("/"))
        path += "/";

      if (Contains(path, ForbiddenCharacters))
        throw new ForbiddenException("Uri contains forbidden characters.");

      string diskPath = path.Replace('/', Path.PathSeparator).Remove(0, 1);

      foreach (Mapping mapping in _mappings)
      {
        if (mapping.UriPath == path)
        {
          FindFiles(mapping.AbsolutePath, fileName, viewNames);
          return;
        }

        if (!path.StartsWith(mapping.UriPath))
          continue;

        // sub folder?
        string absolutePath = Path.Combine(mapping.AbsolutePath, diskPath);
        if (Directory.Exists(absolutePath))
        {
          FindFiles(absolutePath, fileName, viewNames);
          return;
        }
      }
    }

    #endregion

    #region Nested type: Mapping

    private class Mapping
    {
      /// <summary>
      /// Gets or sets absolute path on disk, including file name.
      /// </summary>
      public string AbsolutePath { get; set; }

      /// <summary>
      /// Gets or sets relative file path.
      /// </summary>
      public string RelativePath { get; set; }

      /// <summary>
      /// Gets or sets Uri path, excluding file name
      /// </summary>
      public string UriPath { get; set; }
    }

    #endregion
  }
}