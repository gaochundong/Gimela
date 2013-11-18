using System;
using System.Collections;
using System.Collections.Generic;

namespace Gimela.Data.DataStructures
{
  /// <summary>
  /// This is a dictionary guaranteed to have only one of each item and key. 
  /// It may be searched either by TFirst or by TSecond, giving a unique answer because it is 1 to 1.
  /// </summary>
  /// <typeparam name="TFirst">The type of the "key"</typeparam>
  /// <typeparam name="TSecond">The type of the "item"</typeparam>
  public sealed class BidirectionalCollection<TFirst, TSecond> : IEnumerable<FirstSecondPair<TFirst, TSecond>>, IEnumerable
  {
    #region Fields
    
    private IDictionary<TFirst, TSecond> firstToSecond = new Dictionary<TFirst, TSecond>();
    private IDictionary<TSecond, TFirst> secondToFirst = new Dictionary<TSecond, TFirst>();

    #endregion

    #region Public Methods
    
    /// <summary>
    /// Tries to add the pair to the dictionary.
    /// Throws an exception if either element is already in the dictionary
    /// </summary>
    /// <param name="first"></param>
    /// <param name="second"></param>
    public void Add(TFirst first, TSecond second)
    {
      if (firstToSecond.ContainsKey(first) || secondToFirst.ContainsKey(second))
        throw new ArgumentException("Duplicate first or second");

      firstToSecond.Add(first, second);
      secondToFirst.Add(second, first);
    }

    /// <summary>
    /// Determines whether the collection contains a specific item.
    /// </summary>
    /// <param name="first">The object to locate in the collection.</param>
    /// <returns>true if l is found in the collection; otherwise, false.</returns>
    public bool ContainsFirst(TFirst first)
    {
      return firstToSecond.ContainsKey(first);
    }

    /// <summary>
    /// Determines whether the collection contains a specific item.
    /// </summary>
    /// <param name="second">The object to locate in the collection.</param>
    /// <returns>true if l is found in the collection; otherwise, false.</returns>
    public bool ContainsSecond(TSecond second)
    {
      return secondToFirst.ContainsKey(second);
    }

    /// <summary>
    /// Find the TSecond corresponding to the TFirst first
    /// Throws an exception if first is not in the dictionary.
    /// </summary>
    /// <param name="first">the key to search for</param>
    /// <returns>the item corresponding to first</returns>
    public TSecond GetByFirst(TFirst first)
    {
      TSecond second;
      if (!firstToSecond.TryGetValue(first, out second))
        throw new KeyNotFoundException("Cannot find second by first.");

      return second;
    }

    /// <summary>
    /// Find the TFirst corresponing to the Second second.
    /// Throws an exception if second is not in the dictionary.
    /// </summary>
    /// <param name="second">the key to search for</param>
    /// <returns>the item corresponding to second</returns>
    public TFirst GetBySecond(TSecond second)
    {
      TFirst first;
      if (!secondToFirst.TryGetValue(second, out first))
        throw new KeyNotFoundException("Cannot find first by second.");

      return first;
    }

    /// <summary>
    /// Remove the record containing first.
    /// If first is not in the dictionary, throws an Exception.
    /// </summary>
    /// <param name="first">the key of the record to delete</param>
    public void RemoveByFirst(TFirst first)
    {
      TSecond second;
      if (!firstToSecond.TryGetValue(first, out second))
        throw new KeyNotFoundException("Cannot find second by first.");

      firstToSecond.Remove(first);
      secondToFirst.Remove(second);
    }

    /// <summary>
    /// Remove the record containing second.
    /// If second is not in the dictionary, throws an Exception.
    /// </summary>
    /// <param name="second">the key of the record to delete</param>
    public void RemoveBySecond(TSecond second)
    {
      TFirst first;
      if (!secondToFirst.TryGetValue(second, out first))
        throw new KeyNotFoundException("Cannot find first by second.");

      secondToFirst.Remove(second);
      firstToSecond.Remove(first);
    }

    /// <summary>
    /// Tries to add the pair to the dictionary.
    /// Returns false if either element is already in the dictionary        
    /// </summary>
    /// <param name="first"></param>
    /// <param name="second"></param>
    /// <returns>true if successfully added, false if either element are already in the dictionary</returns>
    public bool TryAdd(TFirst first, TSecond second)
    {
      if (firstToSecond.ContainsKey(first) || secondToFirst.ContainsKey(second))
        return false;

      firstToSecond.Add(first, second);
      secondToFirst.Add(second, first);
      return true;
    }

    /// <summary>
    /// Find the TSecond corresponding to the TFirst first.
    /// Returns false if first is not in the dictionary.
    /// </summary>
    /// <param name="first">the key to search for</param>
    /// <param name="second">the corresponding item</param>
    /// <returns>true if first is in the dictionary, false otherwise</returns>
    public bool TryGetByFirst(TFirst first, out TSecond second)
    {
      return firstToSecond.TryGetValue(first, out second);
    }

    /// <summary>
    /// Find the TFirst corresponding to the TSecond second.
    /// Returns false if second is not in the dictionary.
    /// </summary>
    /// <param name="second">the key to search for</param>
    /// <param name="first">the corresponding item</param>
    /// <returns>true if second is in the dictionary, false otherwise</returns>
    public bool TryGetBySecond(TSecond second, out TFirst first)
    {
      return secondToFirst.TryGetValue(second, out first);
    }

    /// <summary>
    /// Remove the record containing first, if there is one.
    /// </summary>
    /// <param name="first"></param>
    /// <returns>If first is not in the dictionary, returns false, otherwise true</returns>
    public bool TryRemoveByFirst(TFirst first)
    {
      TSecond second;
      if (!firstToSecond.TryGetValue(first, out second))
        return false;

      firstToSecond.Remove(first);
      secondToFirst.Remove(second);
      return true;
    }

    /// <summary>
    /// Remove the record containing second, if there is one.
    /// </summary>
    /// <param name="second"></param>
    /// <returns>If second is not in the dictionary, returns false, otherwise true</returns>
    public bool TryRemoveBySecond(TSecond second)
    {
      TFirst first;
      if (!secondToFirst.TryGetValue(second, out first))
        return false;

      secondToFirst.Remove(second);
      firstToSecond.Remove(first);
      return true;
    }

    /// <summary>
    /// The number of pairs stored in the dictionary
    /// </summary>
    public int Count
    {
      get { return firstToSecond.Count; }
    }

    /// <summary>
    /// Removes all items from the dictionary.
    /// </summary>
    public void Clear()
    {
      firstToSecond.Clear();
      secondToFirst.Clear();
    }

    #endregion

    #region IEnumerable<FirstSecondPair<TFirst,TSecond>> Members

    IEnumerator<FirstSecondPair<TFirst, TSecond>> IEnumerable<FirstSecondPair<TFirst, TSecond>>.GetEnumerator()
    {
      foreach (var item in firstToSecond)
      {
        yield return new FirstSecondPair<TFirst, TSecond>(item.Key, item.Value);
      }
    }

    #endregion

    #region IEnumerable Members

    IEnumerator IEnumerable.GetEnumerator()
    {
      foreach (var item in firstToSecond)
      {
        yield return new FirstSecondPair<TFirst, TSecond>(item.Key, item.Value);
      }
    }

    #endregion
  }
}
