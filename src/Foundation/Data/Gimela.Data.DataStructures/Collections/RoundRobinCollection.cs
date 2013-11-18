using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Gimela.Data.DataStructures
{
  /// <summary>
  /// 循环列表
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class RoundRobinCollection<T> : IEnumerable<T>
  {
    private T[] _items;
    private int _head;

    /// <summary>
    /// 循环列表
    /// </summary>
    /// <param name="items">供循环的列表项</param>
    public RoundRobinCollection(IEnumerable<T> items)
    {
      if (items == null || items.Count<T>() == 0)
      {
        throw new ArgumentException("One or more items must be provided", "items");
      }

      // copy the list to ensure it doesn't change on us (and so we can lock() on our private copy) 
      _items = items.ToArray();
    }

    /// <summary>
    /// 获取循环器
    /// </summary>
    /// <returns></returns>
    public IEnumerator<T> GetEnumerator()
    {
      int currentHead;

      lock (_items)
      {
        currentHead = _head++;

        if (_head == _items.Length)
        {
          // wrap back to the start 
          _head = 0;
        }
      }

      // return results [current] ... [last] 
      for (int i = currentHead; i < _items.Length; i++)
      {
        yield return _items[i];
      }

      // return wrap-around (if any) [0] ... [current-1] 
      for (int i = 0; i < currentHead; i++)
      {
        yield return _items[i];
      }
    }

    /// <summary>
    /// 获取循环器
    /// </summary>
    /// <returns></returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }
  }
}
