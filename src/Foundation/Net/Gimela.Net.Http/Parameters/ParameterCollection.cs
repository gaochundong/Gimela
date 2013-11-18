using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Gimela.Net.Http
{
  /// <summary>
  /// Collection of parameters.
  /// </summary>
  /// <remarks>
  /// <see cref="Dictionary{TKey,TValue}"/> or <see cref="NameValueCollection"/> is not used since each parameter can
  /// have multiple values.
  /// </remarks>
  public class ParameterCollection : IParameterCollection
  {
    private readonly Dictionary<string, IParameter> _items = new Dictionary<string, IParameter>(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterCollection"/> class.
    /// </summary>
    /// <param name="collections">Collections to merge.</param>
    /// <remarks>
    /// Later collections will overwrite parameters from earlier collections.
    /// </remarks>
    public ParameterCollection(params IParameterCollection[] collections)
    {
      foreach (IParameterCollection collection in collections)
      {
        if (collection == null)
          continue;
        foreach (IParameter p in collection)
        {
          foreach (string value in p)
            Add(p.Name, value);
        }
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterCollection"/> class.
    /// </summary>
    public ParameterCollection()
    {
    }

    /// <summary>
    /// Get a list of string arrays.
    /// </summary>
    /// <returns></returns>
    public string[] GetArrayNames()
    {
      var names = new List<string>();
      foreach (var item in _items)
      {
        int pos = item.Key.IndexOf("[");
        if (pos == -1)
          continue;

        names.Add(item.Key.Substring(0, pos));
      }

      return names.ToArray();
    }

    /// <summary>
    /// Get parameters 
    /// </summary>
    /// <param name="arrayName">Sub array (text array)</param>
    /// <returns></returns>
    public IParameterCollection GetParameters(string arrayName)
    {
      var collection = new ParameterCollection();
      arrayName = arrayName + "[";
      foreach (var item in _items)
      {
        if (!item.Key.StartsWith(arrayName)) continue;
        int pos = arrayName.IndexOf("]");
        if (pos == -1) continue;

        string name = arrayName.Substring(arrayName.Length, pos - arrayName.Length);
        foreach (string value in item.Value)
          collection.Add(name, value);
      }

      return collection;
    }

    #region IParameterCollection Members

    /// <summary>
    /// Gets number of parameters.
    /// </summary>
    public int Count
    {
      get { return _items.Count; }
    }

    /// <summary>
    /// Gets last value of an parameter.
    /// </summary>
    /// <param name="name">Parameter name</param>
    /// <returns>String if found; otherwise <c>null</c>.</returns>
    public string this[string name]
    {
      get
      {
        IParameter param = Get(name);
        return param != null ? param.Value : null;
      }
    }


    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public IEnumerator<IParameter> GetEnumerator()
    {
      return _items.Values.GetEnumerator();
    }

    /// <summary>
    /// Returns an enumerator that iterates through a collection.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
    /// </returns>
    /// <filterpriority>2</filterpriority>
    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    /// <summary>
    /// Get a parameter.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public IParameter Get(string name)
    {
      IParameter parameter;
      return _items.TryGetValue(name, out parameter) ? parameter : null;
    }

    /// <summary>
    /// Add a query string parameter.
    /// </summary>
    /// <param name="name">Parameter name</param>
    /// <param name="value">Value</param>
    public void Add(string name, string value)
    {
      IParameter parameter;
      if (!_items.TryGetValue(name, out parameter))
      {
        parameter = new Parameter(name, value);
        _items.Add(name, parameter);
      }
      else
        parameter.Values.Add(value);

    }

    /// <summary>
    /// Checks if the specified parameter exists
    /// </summary>
    /// <param name="name">Parameter name.</param>
    /// <returns><c>true</c> if found; otherwise <c>false</c>;</returns>
    public bool Exists(string name)
    {
      return _items.ContainsKey(name);
    }

    #endregion
  }
}