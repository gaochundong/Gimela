using System;
using System.Collections;
using System.Collections.Generic;

namespace Gimela.Net.Http.Messages
{
  /// <summary>
  /// Cookies that should be set.
  /// </summary>
  public sealed class ResponseCookieCollection : IEnumerable<ResponseCookie>
  {
    private readonly IDictionary<string, ResponseCookie> _items = new Dictionary<string, ResponseCookie>();

    /// <summary>
    /// Gets the count of cookies in the collection.
    /// </summary>
    public int Count
    {
      get { return _items.Count; }
    }


    /// <summary>
    /// Gets the cookie of a given identifier.
    /// </summary>
    /// <value>Cookie if found; otherwise <c>null</c>.</value>
    public ResponseCookie this[string id]
    {
      get { return _items.ContainsKey(id) ? _items[id] : null; }
      set
      {
        if (_items.ContainsKey(id))
          _items[id] = value;
        else
          Add(value);
      }
    }

    /// <summary>
    /// Adds a cookie in the collection.
    /// </summary>
    /// <param name="cookie">cookie to add</param>
    /// <exception cref="ArgumentNullException">cookie is <c>null</c></exception>
    /// <exception cref="ArgumentException">Name and Content must be specified.</exception>
    public void Add(ResponseCookie cookie)
    {
      // Verifies the parameter
      if (cookie == null)
        throw new ArgumentNullException("cookie");
      if (cookie.Name == null || cookie.Name.Trim() == string.Empty)
        throw new ArgumentException("Name must be specified.");
      if (cookie.Value == null || cookie.Value.Trim() == string.Empty)
        throw new ArgumentException("Content must be specified.");

      if (_items.ContainsKey(cookie.Name))
        _items[cookie.Name] = cookie;
      else _items.Add(cookie.Name, cookie);
    }

    /// <summary>
    /// Copy a request cookie
    /// </summary>
    /// <param name="cookie"></param>
    /// <param name="expires">When the cookie should expire</param>
    public void Add(RequestCookie cookie, DateTime expires)
    {
      Add(new ResponseCookie(cookie, expires));
    }


    /// <summary>
    /// Remove all cookies
    /// </summary>
    public void Clear()
    {
      _items.Clear();
    }

    #region IEnumerable<ResponseCookie> Members

    /// <summary>
    /// Gets a collection enumerator on the cookie list.
    /// </summary>
    /// <returns>collection enumerator</returns>
    public IEnumerator GetEnumerator()
    {
      return _items.Values.GetEnumerator();
    }

    ///<summary>
    ///Returns an enumerator that iterates through the collection.
    ///</summary>
    ///
    ///<returns>
    ///A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
    ///</returns>
    ///<filterpriority>1</filterpriority>
    IEnumerator<ResponseCookie> IEnumerable<ResponseCookie>.GetEnumerator()
    {
      return _items.Values.GetEnumerator();
    }

    #endregion
  }
}