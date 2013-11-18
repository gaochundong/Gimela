using System;
using System.Collections.Generic;
using System.IO;

namespace Gimela.Net.Http
{
  /// <summary>
  /// Collection of files.
  /// </summary>
  public class HttpFileCollection
  {
    private readonly Dictionary<string, HttpFile> _files =
        new Dictionary<string, HttpFile>(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Get a file
    /// </summary>
    /// <param name="name">Name in form</param>
    /// <returns>File if found; otherwise <c>null</c>.</returns>
    public HttpFile this[string name]
    {
      get
      {
        HttpFile file;
        return _files.TryGetValue(name, out file) ? file : null;
      }
    }

    /// <summary>
    /// Checks if a file exists.
    /// </summary>
    /// <param name="name">Name of the file (form item name)</param>
    /// <returns></returns>
    public bool Contains(string name)
    {
      return _files.ContainsKey(name);
    }
    /// <summary>
    /// Gets number of files
    /// </summary>
    public int Count
    {
      get { return _files.Count; }
    }

    /// <summary>
    /// Add a new file.
    /// </summary>
    /// <param name="file">File to add.</param>
    public void Add(HttpFile file)
    {
      _files.Add(file.Name, file);
    }

    /// <summary>
    /// Remove all files from disk.
    /// </summary>
    public void Clear()
    {
      foreach (HttpFile file in _files.Values)
      {
        if (File.Exists(file.TempFileName))
          File.Delete(file.TempFileName);
      }
    }
  }
}