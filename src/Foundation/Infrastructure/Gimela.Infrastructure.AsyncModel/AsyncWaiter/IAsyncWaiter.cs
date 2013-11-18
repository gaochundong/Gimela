using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gimela.Infrastructure.AsyncModel
{
  /// <summary>
  /// Interface of a asynchronous waiter.
  /// </summary>
  /// <typeparam name="TToken">The type of the token.</typeparam>
  /// <typeparam name="TState">The type of the state.</typeparam>
  public interface IAsyncWaiter<TToken, TState>
  {
    /// <summary>
    /// Waits the specified token and with the specified state data.
    /// </summary>
    /// <param name="token">The asynchronous token.</param>
    /// <param name="state">The user defined state data.</param>
    void Wait(AsyncToken<TToken> token, TState state);

    /// <summary>
    /// Peeks the specified asynchronous token.
    /// </summary>
    /// <param name="token">The asynchronous token.</param>
    /// <returns>The user defined state data.</returns>
    TState Peek(AsyncToken<TToken> token);

    /// <summary>
    /// Pops the specified asynchronous token.
    /// </summary>
    /// <param name="token">The asynchronous token.</param>
    /// <returns>The user defined state data.</returns>
    TState Pop(AsyncToken<TToken> token);

    /// <summary>
    /// Removes the specified token.
    /// </summary>
    /// <param name="token">The asynchronous token.</param>
    void Remove(AsyncToken<TToken> token);
  }
}
