using System;
using System.Net.Sockets;

namespace Gimela.Net.Http
{
  /// <summary>
  /// Used by <see cref="HttpListener.SocketAccepted"/> to filter out unwanted connections.
  /// </summary>
  public class SocketFilterEventArgs : EventArgs
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="SocketFilterEventArgs"/> class.
    /// </summary>
    /// <param name="socket">The socket.</param>
    public SocketFilterEventArgs(Socket socket)
    {
      Socket = socket;
      IsSocketOk = true;
    }

    /// <summary>
    /// Gets or sets if socket can be accepted.
    /// </summary>
    public bool IsSocketOk { get; set; }

    /// <summary>
    /// Gets socket.
    /// </summary>
    public Socket Socket { get; private set; }
  }
}