using System;
using System.IO;
using System.Security;
using Gimela.Common.ExceptionHandling;
using Microsoft.Win32;

namespace Gimela.Management
{
  /// <summary>
  /// 注册表操作器
  /// </summary>
  public class RegistryOperator : IDisposable
  {
    private RegistryKey _rootkey;

    /// <summary>
    /// 注册表操作器
    /// </summary>
    /// <param name="rootKey">根键</param>
    public RegistryOperator(string rootKey)
    {
      if (string.IsNullOrEmpty(rootKey))
        throw new ArgumentNullException("rootKey");

      switch (rootKey.ToUpperInvariant())
      {
        case "CLASSES_ROOT":
          _rootkey = Microsoft.Win32.Registry.ClassesRoot;
          break;
        case "CURRENT_USER":
          _rootkey = Microsoft.Win32.Registry.CurrentUser;
          break;
        case "LOCAL_MACHINE":
          _rootkey = Microsoft.Win32.Registry.LocalMachine;
          break;
        case "USERS":
          _rootkey = Microsoft.Win32.Registry.Users;
          break;
        case "CURRENT_CONFIG":
          _rootkey = Microsoft.Win32.Registry.CurrentConfig;
          break;
        case "PERFORMANCE_DATA":
          _rootkey = Microsoft.Win32.Registry.PerformanceData;
          break;
        default:
          _rootkey = Microsoft.Win32.Registry.CurrentUser;
          break;
      }
    }

    /// <summary>
    /// 创建路径为keyPath的键
    /// </summary>
    /// <param name="keyPath">The key path.</param>
    /// <returns></returns>
    public string CreateRegistryKeyPath(string keyPath)
    {
      RegistryKey key = null;

      try
      {
        key = _rootkey.CreateSubKey(keyPath);
      }
      catch (SecurityException ex)
      {
        ExceptionHandler.Handle(ex);
      }
      catch (ObjectDisposedException ex)
      {
        ExceptionHandler.Handle(ex);
      }
      catch (UnauthorizedAccessException ex)
      {
        ExceptionHandler.Handle(ex);
      }
      catch (IOException ex)
      {
        ExceptionHandler.Handle(ex);
      }

      return key == null ? null : key.ToString();
    }

    /// <summary>
    /// 删除路径为keyPath的子项
    /// </summary>
    /// <param name="keyPath">The key path.</param>
    /// <returns></returns>
    public bool DeleteRegistryKeyPath(string keyPath)
    {
      bool result = false;

      try
      {
        _rootkey.DeleteSubKey(keyPath);
        result = true;
      }
      catch (SecurityException ex)
      {
        ExceptionHandler.Handle(ex);
      }
      catch (ObjectDisposedException ex)
      {
        ExceptionHandler.Handle(ex);
      }
      catch (UnauthorizedAccessException ex)
      {
        ExceptionHandler.Handle(ex);
      }
      catch (IOException ex)
      {
        ExceptionHandler.Handle(ex);
      }

      return result;
    }
    
    /// <summary>
    /// 设置路径为keyPath，键名为keyName的注册表键值为keyValue
    /// </summary>
    /// <param name="keyPath">The key path.</param>
    /// <param name="keyName">Name of the key.</param>
    /// <param name="keyValue">The key value.</param>
    /// <returns></returns>
    public bool SetRegistryKeyValue(string keyPath, string keyName, string keyValue)
    {
      bool result = false;

      try
      {
        using (RegistryKey key = _rootkey.OpenSubKey(keyPath))
        {
          key.SetValue(keyName, keyValue);
          result = true;
        }
      }
      catch (SecurityException ex)
      {
        ExceptionHandler.Handle(ex);
      }
      catch (ObjectDisposedException ex)
      {
        ExceptionHandler.Handle(ex);
      }
      catch (UnauthorizedAccessException ex)
      {
        ExceptionHandler.Handle(ex);
      }
      catch (IOException ex)
      {
        ExceptionHandler.Handle(ex);
      }

      return result;
    }

    /// <summary>
    /// 读取路径为keyPath，键名为keyName的注册表键值
    /// </summary>
    /// <param name="keyPath">The key path.</param>
    /// <param name="keyName">Name of the key.</param>
    /// <returns></returns>
    public string GetRegistryKeyValue(string keyPath, string keyName)
    {
      object keyValue = null;

      try
      {
        using (RegistryKey key = _rootkey.OpenSubKey(keyPath))
        {
          keyValue = key.GetValue(keyName);
        }
      }
      catch (SecurityException ex)
      {
        ExceptionHandler.Handle(ex);
      }
      catch (ObjectDisposedException ex)
      {
        ExceptionHandler.Handle(ex);
      }
      catch (UnauthorizedAccessException ex)
      {
        ExceptionHandler.Handle(ex);
      }
      catch (IOException ex)
      {
        ExceptionHandler.Handle(ex);
      }

      return keyValue == null ? null : keyValue.ToString();
    }

    /// <summary>
    /// 删除路径为keyPath下键名为keyName的键值
    /// </summary>
    /// <param name="keyPath">The key path.</param>
    /// <param name="keyName">Name of the key.</param>
    /// <returns></returns>
    public bool DeleteRegistryKeyValue(string keyPath, string keyName)
    {
      bool result = false;

      try
      {
        using (RegistryKey key = _rootkey.OpenSubKey(keyPath))
        {
          key.DeleteValue(keyName);
          result = true;
        }
      }
      catch (SecurityException ex)
      {
        ExceptionHandler.Handle(ex);
      }
      catch (ObjectDisposedException ex)
      {
        ExceptionHandler.Handle(ex);
      }
      catch (UnauthorizedAccessException ex)
      {
        ExceptionHandler.Handle(ex);
      }
      catch (IOException ex)
      {
        ExceptionHandler.Handle(ex);
      }

      return result;
    }

    /// <summary>
    /// 删除路径为keyPath的子项及其附属子项
    /// </summary>
    /// <param name="keyPath">The key path.</param>
    /// <returns></returns>
    public bool DeleteRegistrySubkeyTree(string keyPath)
    {
      bool result = false;

      try
      {
        _rootkey.DeleteSubKeyTree(keyPath);
        result = true;
      }
      catch (SecurityException ex)
      {
        ExceptionHandler.Handle(ex);
      }
      catch (ObjectDisposedException ex)
      {
        ExceptionHandler.Handle(ex);
      }
      catch (UnauthorizedAccessException ex)
      {
        ExceptionHandler.Handle(ex);
      }

      return result;
    }

    #region IDisposable Members

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (_rootkey != null)
        {
          _rootkey.Dispose();
          _rootkey = null;
        }
      }
    }

    #endregion
  }
}
