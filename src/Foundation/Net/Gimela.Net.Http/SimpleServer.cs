using System.IO;
using System.Reflection;
using Gimela.Net.Http.BodyDecoders;
using Gimela.Net.Http.Modules;

namespace Gimela.Net.Http
{
  /// <summary>
  /// 简单HTTP服务器
  /// </summary>
  /// <remarks>
  /// Used to make it easy to create and use a web server.
  /// <para>
  /// All resources must exist in the "YourProject.Content" namespace 
  /// (or a subdirectory called "Content" relative to yourapp.exe).
  /// </para>
  /// </remarks>
  public class SimpleServer : Server
  {
    /// <summary>
    /// 简单HTTP服务器
    /// </summary>
    public SimpleServer(string serverName)
      : base(serverName)
    {
      // 增加消息体解析器
      Add(new MultiPartDecoder());
      Add(new UrlDecoder());

      // 增加文件模块
      var fileModule = new FileModule();
      fileModule.AddDefaultMimeTypes();

      AddEmbeddedResources(Assembly.GetCallingAssembly(), fileModule);
      AddFileResources(Assembly.GetCallingAssembly(), fileModule);
    }

    /// <summary>
    /// 添加程序集内的所有嵌入的资源
    /// </summary>
    /// <param name="assembly"></param>
    /// <param name="fileModule"></param>
    private void AddEmbeddedResources(Assembly assembly, FileModule fileModule)
    {
      string contentNamespace = null;
      foreach (var resourceName in assembly.GetManifestResourceNames())
      {
        if (!resourceName.Contains("Content"))
          continue;

        contentNamespace = resourceName;
        break;
      }

      if (contentNamespace == null)
        return;

      int pos = contentNamespace.IndexOf("Content");
      contentNamespace = contentNamespace.Substring(0, pos);
      fileModule.Resources.Add(new Resources.EmbeddedResourceLoader(
        "/content/", Assembly.GetCallingAssembly(), contentNamespace));
    }

    /// <summary>
    /// 增加程序集内的所有文件资源
    /// </summary>
    /// <param name="assembly"></param>
    /// <param name="fileModule"></param>
    private void AddFileResources(Assembly assembly, FileModule fileModule)
    {
      var assemblyPath = Path.GetDirectoryName(assembly.Location);
      var filePath = Path.Combine(assemblyPath, "Public");
      if (Directory.Exists(filePath))
        fileModule.Resources.Add(new Resources.FileResources("/content/", filePath));
    }
  }
}
