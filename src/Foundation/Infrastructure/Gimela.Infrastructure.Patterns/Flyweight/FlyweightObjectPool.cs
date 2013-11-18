using System;
using System.Collections.Concurrent;

namespace Gimela.Infrastructure.Patterns
{
  /// <summary>
  /// 享元模式Flyweight的实现
  /// </summary>
  /// <typeparam name="T">Type of object.</typeparam>
  /// <example>
  /// // C# 中数组是引用类型
  /// var pool = new FlyweightObjectPool byte[] (() => new byte[65535]);
  /// pool.Allocate(1000);
  /// var buffer= pool.Dequeue();
  /// // .. do something here ..
  /// pool.Enqueue(buffer);
  /// </example>
  public class FlyweightObjectPool<T> where T : class
  {
    private readonly Func<T> _factoryMethod;
    private readonly ConcurrentQueue<T> _queue = new ConcurrentQueue<T>();

    /// <summary>
    /// 享元模式Flyweight的实现
    /// </summary>
    /// <param name="factoryMethod">分配缓存的方法</param>
    public FlyweightObjectPool(Func<T> factoryMethod)
    {
      _factoryMethod = factoryMethod;
    }

    /// <summary>
    /// 分配指定数量的对象
    /// </summary>
    /// <param name="count">指定的数量</param>
    public void Allocate(int count)
    {
      for (int i = 0; i < count; i++)
        _queue.Enqueue(_factoryMethod());
    }

    /// <summary>
    /// 缓存一个对象
    /// </summary>
    /// <param name="buffer">对象</param>
    public void Enqueue(T buffer)
    {
      _queue.Enqueue(buffer);
    }

    /// <summary>
    /// 获取一个对象
    /// </summary>
    /// <returns>对象</returns>
    public T Dequeue()
    {
      T buffer;
      return !_queue.TryDequeue(out buffer) ? _factoryMethod() : buffer;
    }
  }
}