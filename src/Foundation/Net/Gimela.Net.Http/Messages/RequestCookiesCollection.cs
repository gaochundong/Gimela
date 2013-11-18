using System;
using System.Collections;
using System.Collections.Generic;

namespace Gimela.Net.Http.Messages
{
  /// <summary>
  /// A list of request cookies.
  /// </summary>
  public sealed class RequestCookieCollection : IEnumerable<RequestCookie>
  {
    private readonly IDictionary<string, RequestCookie> _items = new Dictionary<string, RequestCookie>();

    /// <summary>
    /// Let's copy all the cookies.
    /// </summary>
    /// <param name="cookies">value from cookie header.</param>
    public RequestCookieCollection(string cookies)
    {
      if (string.IsNullOrEmpty(cookies))
        return;

      string name = string.Empty;
      int state = 0;
      int start = -1;
      for (int i = 0; i < cookies.Length; ++i)
      {
        char ch = cookies[i];

        // searching for start of cookie name
        switch (state)
        {
          case 0:
            if (char.IsWhiteSpace(ch))
              continue;
            start = i;
            ++state;
            break;
          case 1:
            if (char.IsWhiteSpace(ch) || ch == '=')
            {
              if (start == -1)
                return; // todo: decide if an exception should be thrown.
              name = cookies.Substring(start, i - start);
              start = -1;
              ++state;
            }
            break;
          case 2:
            if (!char.IsWhiteSpace(ch) && ch != '=')
            {
              start = i;
              ++state;
            }
            break;
          case 3:
            if (ch == ';')
            {
              if (start != -1)
                Add(new RequestCookie(name, cookies.Substring(start, i - start)));
              start = -1;
              state = 0;
              name = string.Empty;
            }
            break;
        }
      }

      // last cookie
      if (name == string.Empty)
        return;

      if (start == -1)
        Add(new RequestCookie(name, string.Empty));
      else
        Add(new RequestCookie(name, cookies.Substring(start, cookies.Length - start)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestCookieCollection"/> class.
    /// </summary>
    public RequestCookieCollection()
    {

    }

    /// <summary>
    /// Gets the count of cookies in the collection.
    /// </summary>
    public int Count
    {
      get { return _items.Count; }
    }


    /// <summary>
    /// Gets the cookie of a given identifier (<c>null</c> if not existing).
    /// </summary>
    public RequestCookie this[string id]
    {
      get { return _items.ContainsKey(id) ? _items[id] : null; }
    }

    /// <summary>
    /// Adds a cookie in the collection.
    /// </summary>
    /// <param name="cookie">cookie to add</param>
    /// <exception cref="ArgumentNullException">cookie is <c>null</c></exception>
    /// <exception cref="ArgumentException">Name must be specified.</exception>
    internal void Add(RequestCookie cookie)
    {
      // Verifies the parameter
      if (cookie == null)
        throw new ArgumentNullException("cookie");
      if (cookie.Name == null || cookie.Name.Trim() == string.Empty)
        throw new ArgumentException("Name must be specified.");

      if (_items.ContainsKey(cookie.Name))
        _items[cookie.Name] = cookie;
      else _items.Add(cookie.Name, cookie);
    }


    /// <summary>
    /// Remove all cookies.
    /// </summary>
    public void Clear()
    {
      _items.Clear();
    }

    /// <summary>
    /// Remove a cookie from the collection.
    /// </summary>
    /// <param name="cookieName">Name of cookie.</param>
    public void Remove(string cookieName)
    {
      lock (_items)
      {
        if (!_items.ContainsKey(cookieName))
          return;

        _items.Remove(cookieName);
      }
    }

    #region IEnumerable<RequestCookie> Members

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
    IEnumerator<RequestCookie> IEnumerable<RequestCookie>.GetEnumerator()
    {
      return _items.Values.GetEnumerator();
    }

    #endregion
  }
}