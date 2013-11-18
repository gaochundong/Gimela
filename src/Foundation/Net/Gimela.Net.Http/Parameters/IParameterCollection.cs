using System.Collections;
using System.Collections.Generic;

namespace Gimela.Net.Http
{
  /// <summary>
  /// Collection of parameters
  /// </summary>
  public interface IParameterCollection : IEnumerable<IParameter>
  {
    /// <summary>
    /// Gets number of parameters.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Gets last value of an parameter.
    /// </summary>
    /// <param name="name">Parameter name</param>
    /// <returns>String if found; otherwise <c>null</c>.</returns>
    string this[string name] { get; }

    /// <summary>
    /// Get a parameter.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    IParameter Get(string name);

    /// <summary>
    /// Add a query string parameter.
    /// </summary>
    /// <param name="name">Parameter name</param>
    /// <param name="value">Value</param>
    void Add(string name, string value);

    /// <summary>
    /// Checks if the specified parameter exists
    /// </summary>
    /// <param name="name">Parameter name.</param>
    /// <returns><c>true</c> if found; otherwise <c>false</c>;</returns>
    bool Exists(string name);
  }

  /// <summary>
  /// Parameter in <see cref="IParameterCollection"/>
  /// </summary>
  public interface IParameter : IEnumerable<string>
  {
    /// <summary>
    /// Gets *last* value.
    /// </summary>
    /// <remarks>
    /// Parameters can have multiple values. This property will always get the last value in the list.
    /// </remarks>
    /// <value>String if any value exist; otherwise <c>null</c>.</value>
    string Value { get; }

    /// <summary>
    /// Gets or sets name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets a list of all values.
    /// </summary>
    List<string> Values { get; }
  }

  /// <summary>
  /// A parameter in <see cref="IParameterCollection"/>.
  /// </summary>
  public class Parameter : IParameter
  {
    private readonly List<string> _values = new List<string>();

    /// <summary>
    /// Initializes a new instance of the <see cref="Parameter"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="values">The values.</param>
    public Parameter(string name, params string[] values)
    {
      Name = name;
      _values.AddRange(values);
    }
    /// <summary>
    /// Gets last value.
    /// </summary>
    /// <remarks>
    /// Parameters can have multiple values. This property will always get the last value in the list.
    /// </remarks>
    /// <value>String if any value exist; otherwise <c>null</c>.</value>
    public string Value
    {
      get { return _values.Count == 0 ? null : _values[_values.Count - 1]; }
    }

    /// <summary>
    /// Gets or sets name.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Gets a list of all values.
    /// </summary>
    public List<string> Values
    {
      get { return _values; }
    }

    #region IEnumerable<string> Members

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
    /// </returns>
    /// <filterpriority>1</filterpriority>
    IEnumerator<string> IEnumerable<string>.GetEnumerator()
    {
      return _values.GetEnumerator();
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
      return _values.GetEnumerator();
    }

    #endregion
  }
}