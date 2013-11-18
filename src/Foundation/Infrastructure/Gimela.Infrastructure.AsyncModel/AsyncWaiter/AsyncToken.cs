using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gimela.Infrastructure.AsyncModel
{
  /// <summary>
  /// The asynchronous token
  /// </summary>
  /// <typeparam name="T">Token value taype</typeparam>
  public class AsyncToken<T>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncToken&lt;T&gt;"/> class.
    /// </summary>
    /// <param name="token">The token value.</param>
    public AsyncToken(T token)
    {
      if (object.ReferenceEquals(token, null))
      {
        throw new ArgumentNullException("token");
      }

      Token = token;
    }

    /// <summary>
    /// The asynchronous token value.
    /// </summary>
    public T Token { get; private set; }

    /// <summary>
    /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
    /// </summary>
    /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
    /// <returns>
    ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
    /// </returns>
    public override bool Equals(object obj)
    {
      if (!(obj is AsyncToken<T>))
      {
        return false;
      }

      AsyncToken<T> t = (AsyncToken<T>)obj;
      return t.Token.Equals(this.Token);
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>
    /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
    /// </returns>
    public override int GetHashCode()
    {
      return Token.GetHashCode();
    }

    /// <summary>
    /// Returns a <see cref="System.String"/> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String"/> that represents this instance.
    /// </returns>
    public override string ToString()
    {
      return Token.ToString();
    }
  }
}
