using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Gimela.Infrastructure.Patterns
{
  public static class ConcurrentDictionaryExtensions
  {
    public static TValue Add<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> collection, TKey key, TValue @value)
    {
      TValue result = collection.AddOrUpdate(key, @value, (k, v) => { return @value; });
      return result;
    }

    public static TValue Update<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> collection, TKey key, TValue @value)
    {
      TValue result = collection.AddOrUpdate(key, @value, (k, v) => { return @value; });
      return result;
    }

    public static TValue Get<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> collection, TKey key)
    {
      TValue @value = default(TValue);
      collection.TryGetValue(key, out @value);
      return @value;
    }

    public static TValue Remove<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> collection, TKey key)
    {
      TValue @value = default(TValue);
      collection.TryRemove(key, out @value);
      return @value;
    }
  }
}
