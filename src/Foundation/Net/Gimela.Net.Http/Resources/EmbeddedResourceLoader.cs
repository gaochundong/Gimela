using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Gimela.Common.Logging;

namespace Gimela.Net.Http.Resources
{
  /// <summary>
  /// Loads resources that are embedded in assemblies.
  /// </summary>
  /// <remarks>
  /// No locks used internally since all mappings are loaded during start up.
  /// </remarks>
  public class EmbeddedResourceLoader : IResourceLoader
  {
    private readonly Dictionary<string, Mapping> _mappings =
        new Dictionary<string, Mapping>(StringComparer.OrdinalIgnoreCase);

    private readonly List<PathMapping> _pathMappings = new List<PathMapping>();

    /// <summary>
    /// Initializes a new instance of the <see cref="EmbeddedResourceLoader"/> class.
    /// </summary>
    public EmbeddedResourceLoader()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EmbeddedResourceLoader"/> class.
    /// </summary>
    /// <param name="path">Path (Uri) requested by clients</param>
    /// <param name="assembly">Assembly that the resources exist in</param>
    /// <param name="nameSpace">Name space that the resources exist in</param>
    public EmbeddedResourceLoader(string path, Assembly assembly, string nameSpace)
    {
      AddPath(path, assembly, nameSpace);
      AddFilesInFolder(path, assembly, nameSpace);
    }

    /// <summary>
    /// Add a specific resource.
    /// </summary>
    /// <param name="path">Path (Uri) requested by clients</param>
    /// <param name="assembly">Assembly that the resources exist in</param>
    /// <param name="rootNameSpace">Name space to root folder under (all name spaces below the specified one are considered as folders)</param>
    /// <param name="fullResourceName">Name space and name of resource.</param>
    /// <example>
    /// <code>
    /// Add("/", Assembly.GetExecutingAssembly(), "MyApplication.Files", "Myapplication.Files.Images.MyImage.png");
    /// </code>
    /// </example>
    public void Add(string path, Assembly assembly, string rootNameSpace, string fullResourceName)
    {
      if (!path.EndsWith("/"))
        path += "/";

      var mapping = new Mapping { Assembly = assembly, FullResourceName = fullResourceName };

      // remove namespace and the dot after it.
      string filePath = fullResourceName.Remove(0, rootNameSpace.Length + 1);

      // get pos for extension.
      int extensionPos = filePath.LastIndexOf(".");
      if (extensionPos == -1)
        return; // got no extension, cant be a view.


      // check if it's a double extension.
      int nextPos = filePath.LastIndexOf(".", extensionPos - 1);
      if (nextPos != -1)
      {
        string typeExtension = filePath.Substring(nextPos + 1, extensionPos - nextPos - 1);
        if (typeExtension == "xml" || typeExtension == "json" || typeExtension == "js")
          mapping.TypeExtension = typeExtension;
      }

      if (string.IsNullOrEmpty(mapping.TypeExtension))
        nextPos = extensionPos;

      // TODO: next thing is to set the language. But not today.
      // /users/list.1053.xml.spark <--- language 1053, view for xml, spark is the view engine.

      filePath = filePath.Substring(0, nextPos).Replace(".", "/") + filePath.Substring(extensionPos);

      mapping.FileName = Path.GetFileName(filePath).ToLower();
      mapping.UriPath = (path + filePath).Replace('\\', '/').ToLower();

      Logger.Trace("Adding mapping '" + path + filePath + "' to resource '" + fullResourceName + "' assembly '" +
                    assembly +
                    "'.");
      _mappings.Add(path.ToLower() + filePath.ToLower(), mapping);
    }

    /// <summary>
    /// Add resources.
    /// </summary>
    /// <param name="path">Path (Uri) requested by clients</param>
    /// <param name="assembly">Assembly that the resources exist in</param>
    /// <param name="resourceName">Name of resource, including name space.</param>
    /// <returns><c>true</c> if file was found (and has not previously been added); otherwise <c>false</c>.</returns>
    /// <remarks>
    /// </remarks>
    public bool AddFile(string path, Assembly assembly, string resourceName)
    {
      if (!path.StartsWith("/"))
        path = "/" + path;

      string uriPath = Path.GetDirectoryName(path).Replace('\\', '/') + "/";
      var mapping = new Mapping
                      {
                        Assembly = assembly,
                        FullResourceName = resourceName,
                        FileName = Path.GetFileName(path),
                        TypeExtension = Path.GetExtension(path),
                        UriPath = uriPath.ToLower()
                      };

      if (_mappings.ContainsKey(path.ToLower()))
        return false;

      Logger.Trace("Added " + path + " = " + resourceName);
      _mappings.Add(path.ToLower(), mapping);
      return true;
    }

    /// <summary>
    /// Add resources in a specific path (will not work with sub folders)
    /// </summary>
    /// <param name="path">Path (Uri) requested by clients</param>
    /// <param name="assembly">Assembly that the resources exist in</param>
    /// <param name="rootNameSpace">Name space to root folder under which all name spaces exists in,</param>
    /// <returns><c>true</c> if any files was found; otherwise <c>false</c>.</returns>
    /// <remarks>
    /// <para>
    /// Adds all views in the specified folder. Sub folders are not supported since it's hard to determine
    /// with parts are the path and witch parts are the filename. Use <see cref="AddPath"/> to get support
    /// for sub folders.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// Add("/user/", typeof(MyController).Assembly, "YourProject.Users.Views");
    /// </code>
    /// </example>
    public bool AddFilesInFolder(string path, Assembly assembly, string rootNameSpace)
    {
      rootNameSpace = rootNameSpace.ToLower();

      bool foundFiles = false;
      foreach (string resourceName in assembly.GetManifestResourceNames())
      {
        if (!resourceName.ToLower().StartsWith(rootNameSpace))
          continue;

        string fileName = resourceName.Remove(0, rootNameSpace.Length + 1); //include last dot
        if (AddFile(path + fileName, assembly, resourceName))
          foundFiles = true;
      }

      return foundFiles;
    }

    /// <summary>
    /// Add resources in a folder and it's sub folder
    /// </summary>
    /// <param name="path"></param>
    /// <param name="assembly"></param>
    /// <param name="rootNameSpace"></param>
    /// <remarks>
    /// <para>This method is not going to map files but keep the mapping and 
    /// try to look up views every time they are requested. This is the method
    /// to use to add a resource folder that has sub folders.
    /// </para>
    /// </remarks>
    public void AddPath(string path, Assembly assembly, string rootNameSpace)
    {
      var mapping = new PathMapping
                        {
                          Assembly = assembly,
                          UriPath = path.ToLower(),
                          RootNameSpace = rootNameSpace
                        };
      _pathMappings.Add(mapping);

      // We need to create a resource index since resource loading 
      // is case sensitive and paths supplied by web client will probably not have
      // the correct case.
      foreach (var resourceName in assembly.GetManifestResourceNames())
      {
        if (!resourceName.StartsWith(rootNameSpace))
          continue;

        mapping.Resources.Add(resourceName);
      }
    }

    private Mapping FindResourceByWildcard(string uri)
    {
      foreach (Mapping value in _mappings.Values)
      {
        if (value.UriPath.Equals(uri))
          return value;
      }

      return null;
    }

    /// <summary>
    /// Tries to load file by using previously added paths.
    /// </summary>
    /// <param name="uriPath">Uri path including file name</param>
    /// <returns></returns>
    private bool LoadMappingFromPath(string uriPath)
    {
      uriPath = uriPath.ToLower();
      foreach (var pathMapping in _pathMappings)
      {
        if (!uriPath.StartsWith(pathMapping.UriPath))
          continue;

        int pos = uriPath.LastIndexOf('/');
        string fileName = uriPath.Substring(pos + 1);

        // create namespace 
        string ns = pathMapping.RootNameSpace + '.' +
                    uriPath.Remove(0, pathMapping.UriPath.Length).Replace('/', '.');


        // try to find the resource using our small index
        foreach (var resourceName in pathMapping.Resources)
        {
          if (!resourceName.Equals(ns, StringComparison.OrdinalIgnoreCase))
            continue;
          ns = resourceName;
          // no need for it in the index, since a correct
          // mapping is created below.
          pathMapping.Resources.Remove(resourceName);
          break;
        }


        // try to load it.
        Stream stream = pathMapping.Assembly.GetManifestResourceStream(ns);
        if (stream == null)
          continue;

        var mapping = new Mapping
                          {
                            Assembly = pathMapping.Assembly,
                            FileName = fileName,
                            FullResourceName = ns,
                            TypeExtension = Path.GetExtension(ns),
                            UriPath = uriPath.ToLower()
                          };

        lock (_mappings)
        {
          if (!_mappings.ContainsKey(uriPath))
            _mappings[uriPath] = mapping;
        }

        return true;
      }

      return false;
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
      return _mappings.ContainsKey(uriPath) || LoadMappingFromPath(uriPath);
    }

    /// <summary>
    /// Load a resource.
    /// </summary>
    /// <param name="uri">Uri of resource.</param>
    /// <returns>Resource if found and loaded; otherwise <c>null</c>.</returns>
    public Resource Get(string uri)
    {
      Mapping mapping;
      if (uri.EndsWith(".*"))
      {
        mapping = FindResourceByWildcard(uri.Remove(uri.Length - 2));
        if (mapping == null)
          return null;
      }
      else if (!_mappings.TryGetValue(uri, out mapping))
      {

        // try to load it from a path mapping.
        if (!LoadMappingFromPath(uri) || !_mappings.TryGetValue(uri, out mapping))
          return null;
      }

      try
      {
        return new Resource
                   {
                     Stream = mapping.Assembly.GetManifestResourceStream(mapping.FullResourceName),
                     ModifiedAt = File.GetLastWriteTime(mapping.Assembly.Location)
                   };
      }
      catch (FileNotFoundException)
      {
        return null;
      }
      catch (FileLoadException)
      {
        return null;
      }
    }


    /// <summary>
    /// Find all views in a folder/path.
    /// </summary>
    /// <param name="path">Uri path</param>
    /// <param name="viewNames">Collection to add all view names to.</param>
    public void Find(string path, List<string> viewNames)
    {
      path = path.ToLower();

      LoadFromPathMappings(path);

      foreach (Mapping mapping in _mappings.Values)
      {
        if (mapping.UriPath == path)
          viewNames.Add(mapping.FileName);
      }


    }

    /// <summary>
    /// Loads all files in a resource directory
    /// </summary>
    /// <param name="path"></param>
    private void LoadFromPathMappings(string path)
    {
      if (path != "/")
        path = path.TrimEnd('/');

      foreach (var mapping in _pathMappings)
      {
        if (!path.StartsWith(mapping.UriPath))
          continue;

        // get path that should be added to the namespace
        string relativePath = path.Remove(0, mapping.UriPath.Length);

        // create namespace to load files from
        string myNamespace = mapping.RootNameSpace + '.' + relativePath.Replace('/', '.');

        // Load files
        if (AddFilesInFolder(path + "/", mapping.Assembly, myNamespace.ToLower()))
          Logger.Debug("Added all files in namespace '" + myNamespace + "' from uri '" + path + "'.");
      }
    }

    #endregion

    #region Nested type: Mapping

    private class Mapping
    {
      /// <summary>
      /// Gets or sets assembly that the resource exists in.
      /// </summary>
      public Assembly Assembly { get; set; }

      /// <summary>
      /// Gets or sets resource name.
      /// </summary>
      public string FileName { get; set; }

      /// <summary>
      /// Gets or sets full name space path to resource.
      /// </summary>
      public string FullResourceName { get; set; }

      /// <summary>
      /// Gets or sets if this file is for a certain content type.
      /// </summary>
      public string TypeExtension { get; set; }

      /// <summary>
      /// Gets or sets full "virtual" Uri path, excluding file name.
      /// </summary>
      public string UriPath { get; set; }
    }

    #endregion

    #region Nested type: PathMapping

    private class PathMapping
    {
      /// <summary>
      /// Gets or sets assembly
      /// </summary>
      public Assembly Assembly { get; set; }

      /// <summary>
      /// Gets or sets name space root.
      /// </summary>
      public string RootNameSpace { get; set; }

      /// <summary>
      /// Gets or sets uri path.
      /// </summary>
      public string UriPath { get; set; }

      public List<string> Resources { get; private set; }

      public PathMapping()
      {
        Resources = new List<string>();
      }
    }

    #endregion
  }
}