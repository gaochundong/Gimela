using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

namespace Gimela.Infrastructure.ResourceLocation
{
  /// <summary>
  /// IoC容器，实现ServiceLocator模式
  /// </summary>
  public static class Locator
  {
    #region Fields

    private static readonly Dictionary<Type, Type> _mapping = new Dictionary<Type, Type>();
    private static readonly Dictionary<Type, object> _resources = new Dictionary<Type, object>();
    private static object _operationLock = new object();

    #endregion

    #region Add

    /// <summary>
    /// 添加注册资源
    /// </summary>
    /// <typeparam name="TClass">资源类型</typeparam>
    /// <param name="instance">资源实例</param>
    public static void Add<TClass>(object instance)
        where TClass : class
    {
      Add(typeof(TClass), instance);
    }

    /// <summary>
    /// 添加注册资源
    /// </summary>
    /// <param name="typeOfInstance">资源类型</param>
    /// <param name="instance">资源实例</param>
    public static void Add(Type typeOfInstance, object instance)
    {
      if (typeOfInstance == null)
        throw new ArgumentNullException("typeOfInstance");
      if (instance == null)
        throw new ArgumentNullException("instance");

      if (!(typeOfInstance.IsInstanceOfType(instance)))
      {
        throw new InvalidCastException(
            string.Format(CultureInfo.InvariantCulture,
            "Resource does not implement supplied interface: {0}", typeOfInstance.FullName));
      }

      lock (_operationLock)
      {
        if (_resources.ContainsKey(typeOfInstance))
        {
          throw new ArgumentException(
              string.Format(CultureInfo.InvariantCulture, "Resource is already existing : {0}", typeOfInstance.FullName));
        }
        _resources[typeOfInstance] = instance;
      }
    }

    #endregion

    #region Get

    /// <summary>
    /// 查找指定类型的资源实例
    /// </summary>
    /// <typeparam name="TClass">资源类型</typeparam>
    /// <returns>资源实例</returns>
    public static TClass Get<TClass>()
        where TClass : class
    {
      return Get(typeof(TClass)) as TClass;
    }

    /// <summary>
    /// 查找指定类型的资源实例
    /// </summary>
    /// <param name="typeOfInstance">The type of instance.</param>
    /// <returns>资源实例</returns>
    public static object Get(Type typeOfInstance)
    {
      if (typeOfInstance == null)
        throw new ArgumentNullException("typeOfInstance");

      object resource;

      lock (_operationLock)
      {
        if (!_resources.TryGetValue(typeOfInstance, out resource))
        {
          throw new ResourceNotFoundException(typeOfInstance.FullName);
        }
      }

      if (resource == null)
      {
        throw new ResourceNotInstantiatedException(typeOfInstance.FullName);
      }

      return resource;
    }

    /// <summary>
    /// 尝试查找指定类型的资源实例
    /// </summary>
    /// <typeparam name="TClass">资源类型</typeparam>
    /// <param name="resource">资源实例</param>
    /// <returns>是否存在指定资源类型的资源实例</returns>
    public static bool TryGet<TClass>(out TClass resource)
        where TClass : class
    {
      bool isFound = false;

      resource = null;
      object target;

      lock (_operationLock)
      {
        if (_resources.TryGetValue(typeof(TClass), out target))
        {
          resource = target as TClass;
          isFound = true;
        }
      }

      return isFound;
    }

    #endregion

    #region Register

    /// <summary>
    /// 注册类型
    /// </summary>
    /// <typeparam name="TClass">实体类型，类型限制为有公共无参构造函数</typeparam>
    public static void RegisterType<TClass>()
      where TClass : class, new()
    {
      lock (_operationLock)
      {
        _mapping[typeof(TClass)] = typeof(TClass);
      }
    }

    /// <summary>
    /// 注册类型
    /// </summary>
    /// <typeparam name="TFrom">资源类型</typeparam>
    /// <typeparam name="TTo">实体类型，类型限制为有公共无参构造函数</typeparam>
    public static void RegisterType<TFrom, TTo>()
      where TFrom : class
      where TTo : TFrom, new()
    {
      lock (_operationLock)
      {
        _mapping[typeof(TFrom)] = typeof(TTo);
        _mapping[typeof(TTo)] = typeof(TTo);
      }
    }

    /// <summary>
    /// 是否已注册此类型
    /// </summary>
    /// <typeparam name="TClass">资源类型</typeparam>
    /// <returns>是否已注册此类型</returns>
    public static bool IsRegistered<TClass>()
    {
      lock (_operationLock)
      {
        return _mapping.ContainsKey(typeof(TClass));
      }
    }

    #endregion

    #region Resolve

    /// <summary>
    /// 获取类型实例
    /// </summary>
    /// <typeparam name="TClass">资源类型</typeparam>
    /// <returns>类型实例</returns>
    public static TClass Resolve<TClass>()
      where TClass : class
    {
      TClass resource = default(TClass);

      bool existing = TryGet<TClass>(out resource);
      if (!existing)
      {
        ConstructorInfo constructor = null;

        lock (_operationLock)
        {
          if (!_mapping.ContainsKey(typeof(TClass)))
          {
            throw new ResourceNotResolvedException(
              string.Format(CultureInfo.InvariantCulture, "Cannot find the target type : {0}", typeof(TClass).FullName));
          }

          Type concrete = _mapping[typeof(TClass)];
          constructor = concrete.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, new Type[0], null);
          if (constructor == null)
          {
            throw new ResourceNotResolvedException(
              string.Format(CultureInfo.InvariantCulture, "Public constructor is missing for type : {0}", typeof(TClass).FullName));
          }
        }

        Add<TClass>((TClass)constructor.Invoke(null));
      }

      return Get<TClass>();
    }

    #endregion

    #region Remove

    /// <summary>
    /// 移除指定类型的资源实例
    /// </summary>
    /// <typeparam name="TClass">资源类型</typeparam>
    public static void Remove<TClass>()
    {
      Teardown(typeof(TClass));
    }

    /// <summary>
    /// 移除指定类型的资源实例
    /// </summary>
    /// <param name="typeOfInstance">资源类型</param>
    public static void Remove(Type typeOfInstance)
    {
      if (typeOfInstance == null)
        throw new ArgumentNullException("typeOfInstance");

      lock (_operationLock)
      {
        _resources.Remove(typeOfInstance);
      }
    }

    #endregion

    #region Teardown

    /// <summary>
    /// 拆除指定类型的资源实例及注册映射类型
    /// </summary>
    /// <typeparam name="TClass">资源类型</typeparam>
    public static void Teardown<TClass>()
    {
      Teardown(typeof(TClass));
    }

    /// <summary>
    /// 拆除指定类型的资源实例及注册映射类型
    /// </summary>
    /// <param name="typeOfInstance">资源类型</param>
    public static void Teardown(Type typeOfInstance)
    {
      if (typeOfInstance == null)
        throw new ArgumentNullException("typeOfInstance");

      lock (_operationLock)
      {
        _resources.Remove(typeOfInstance);
        _mapping.Remove(typeOfInstance);
      }
    }

    #endregion

    #region Clear

    /// <summary>
    /// 移除所有资源
    /// </summary>
    public static void Clear()
    {
      lock (_operationLock)
      {
        _resources.Clear();
        _mapping.Clear();
      }
    }

    #endregion
  }
}
