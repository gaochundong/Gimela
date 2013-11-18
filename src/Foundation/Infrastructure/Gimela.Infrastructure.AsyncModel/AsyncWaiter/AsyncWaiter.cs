using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gimela.Infrastructure.AsyncModel
{
  /// <summary>
  /// A asynchronous waiter.
  /// </summary>
  /// <typeparam name="TToken">Asynchronous token type.</typeparam>
  /// <typeparam name="TState">The user defined state data type.</typeparam>
  public class AsyncWaiter<TToken, TState> : IAsyncWaiter<TToken, TState>
  {
    private readonly Dictionary<AsyncToken<TToken>, TState> waiter;
    private readonly object syncRoot = new object();

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncWaiter&lt;TToken, TState&gt;"/> class.
    /// </summary>
    public AsyncWaiter()
    {
      waiter = new Dictionary<AsyncToken<TToken>, TState>();
    }

    /// <summary>
    /// Waits the specified token and with the specified state data.
    /// </summary>
    /// <param name="token">The asynchronous token.</param>
    /// <param name="state">The user defined state data.</param>
    public void Wait(AsyncToken<TToken> token, TState state)
    {
      lock (syncRoot)
      {
        if (waiter.ContainsKey(token))
          throw new InvalidProgramException("Invalid async token, the same key already exists.");

        waiter[token] = state;
      }
    }

    /// <summary>
    /// Peeks the specified asynchronous token.
    /// </summary>
    /// <param name="token">The asynchronous token.</param>
    /// <returns>The user defined state data.</returns>
    public TState Peek(AsyncToken<TToken> token)
    {
      lock (syncRoot)
      {
        if (!waiter.ContainsKey(token))
          throw new AsyncTokenNotFoundException<TToken>(token);

        TState state = waiter[token];
        return state;
      }
    }

    /// <summary>
    /// Pops the specified asynchronous token.
    /// </summary>
    /// <param name="token">The asynchronous token.</param>
    /// <returns>The user defined state data.</returns>
    public TState Pop(AsyncToken<TToken> token)
    {
      lock (syncRoot)
      {
        if (!waiter.ContainsKey(token))
          throw new AsyncTokenNotFoundException<TToken>(token);

        TState state = waiter[token];
        waiter.Remove(token);

        return state;
      }
    }

    /// <summary>
    /// Removes the specified token.
    /// </summary>
    /// <param name="token">The asynchronous token.</param>
    public void Remove(AsyncToken<TToken> token)
    {
      lock (syncRoot)
      {
        waiter.Remove(token);
      }
    }
  }
}
