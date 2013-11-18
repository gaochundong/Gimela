using System;
using System.IO;
using System.Net.Sockets;

namespace Gimela.Net.Http.Transports
{
  /// <summary>
  /// Custom network stream to mark sockets as reusable when disposing the stream.
  /// </summary>
  public class ReusableSocketNetworkStream : NetworkStream
  {
    private bool _isDisposed;

    /// <summary>
    /// Creates a new instance of the <see cref="T:System.Net.Sockets.NetworkStream" /> class for the specified <see cref="T:System.Net.Sockets.Socket" />.
    /// </summary>
    /// <param name="socket">
    /// The <see cref="T:System.Net.Sockets.Socket" /> that the <see cref="T:System.Net.Sockets.NetworkStream" /> will use to send and receive data. 
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// The <paramref name="socket" /> parameter is <c>null</c>. 
    /// </exception>
    /// <exception cref="T:System.IO.IOException">
    /// The <paramref name="socket" /> parameter is not connected.
    /// -or- 
    /// The <see cref="P:System.Net.Sockets.Socket.SocketType" /> property of the <paramref name="socket" /> parameter is not <see cref="F:System.Net.Sockets.SocketType.Stream" />.
    /// -or- 
    /// The <paramref name="socket" /> parameter is in a nonblocking state. 
    /// </exception>
    public ReusableSocketNetworkStream(Socket socket)
      : base(socket)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Net.Sockets.NetworkStream" /> class for the specified <see cref="T:System.Net.Sockets.Socket" /> with the specified <see cref="T:System.Net.Sockets.Socket" /> ownership.
    /// </summary>
    /// <param name="socket">
    /// The <see cref="T:System.Net.Sockets.Socket" /> that the <see cref="T:System.Net.Sockets.NetworkStream" /> will use to send and receive data. 
    /// </param>
    /// <param name="ownsSocket">
    /// Set to <c>true</c> to indicate that the <see cref="T:System.Net.Sockets.NetworkStream" /> will take ownership of the <see cref="T:System.Net.Sockets.Socket" />; otherwise, <c>false</c>. 
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// The <paramref name="socket" /> parameter is <c>null</c>. 
    /// </exception>
    /// <exception cref="T:System.IO.IOException">
    /// The <paramref name="socket" /> parameter is not connected.
    /// -or- 
    /// the value of the <see cref="P:System.Net.Sockets.Socket.SocketType" /> property of the <paramref name="socket" /> parameter is not <see cref="F:System.Net.Sockets.SocketType.Stream" />.
    /// -or- 
    /// the <paramref name="socket" /> parameter is in a nonblocking state. 
    /// </exception>
    public ReusableSocketNetworkStream(Socket socket, bool ownsSocket)
      : base(socket, ownsSocket)
    {
    }

    /// <summary>
    /// Creates a new instance of the <see cref="T:System.Net.Sockets.NetworkStream" /> class for the specified <see cref="T:System.Net.Sockets.Socket" /> with the specified access rights.
    /// </summary>
    /// <param name="socket">
    /// The <see cref="T:System.Net.Sockets.Socket" /> that the <see cref="T:System.Net.Sockets.NetworkStream" /> will use to send and receive data. 
    /// </param>
    /// <param name="access">
    /// A bitwise combination of the <see cref="T:System.IO.FileAccess" /> values that specify the type of access given to the <see cref="T:System.Net.Sockets.NetworkStream" /> over the provided <see cref="T:System.Net.Sockets.Socket" />. 
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// The <paramref name="socket" /> parameter is <c>null</c>. 
    /// </exception>
    /// <exception cref="T:System.IO.IOException">
    /// The <paramref name="socket" /> parameter is not connected.
    /// -or- 
    /// the <see cref="P:System.Net.Sockets.Socket.SocketType" /> property of the <paramref name="socket" /> parameter is not <see cref="F:System.Net.Sockets.SocketType.Stream" />.
    /// -or- 
    /// the <paramref name="socket" /> parameter is in a nonblocking state. 
    /// </exception>
    public ReusableSocketNetworkStream(Socket socket, FileAccess access)
      : base(socket, access)
    {
    }

    /// <summary>
    /// Creates a new instance of the <see cref="T:System.Net.Sockets.NetworkStream" /> class for the specified <see cref="T:System.Net.Sockets.Socket" /> with the specified access rights and the specified <see cref="T:System.Net.Sockets.Socket" /> ownership.
    /// </summary>
    /// <param name="socket">
    /// The <see cref="T:System.Net.Sockets.Socket" /> that the <see cref="T:System.Net.Sockets.NetworkStream" /> will use to send and receive data. 
    /// </param>
    /// <param name="access">
    /// A bitwise combination of the <see cref="T:System.IO.FileAccess" /> values that specifies the type of access given to the <see cref="T:System.Net.Sockets.NetworkStream" /> over the provided <see cref="T:System.Net.Sockets.Socket" />. 
    /// </param>
    /// <param name="ownsSocket">
    /// Set to <c>true</c> to indicate that the <see cref="T:System.Net.Sockets.NetworkStream" /> will take ownership of the <see cref="T:System.Net.Sockets.Socket" />; otherwise, <c>false</c>. 
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// The <paramref name="socket" /> parameter is <c>null</c>. 
    /// </exception>
    /// <exception cref="T:System.IO.IOException">
    /// The <paramref name="socket" /> parameter is not connected.
    /// -or- 
    /// The <see cref="P:System.Net.Sockets.Socket.SocketType" /> property of the <paramref name="socket" /> parameter is not <see cref="F:System.Net.Sockets.SocketType.Stream" />.
    /// -or- 
    /// The <paramref name="socket" /> parameter is in a nonblocking state. 
    /// </exception>
    public ReusableSocketNetworkStream(Socket socket, FileAccess access, bool ownsSocket)
      : base(socket, access, ownsSocket)
    {
    }

    /// <summary>
    /// Closes the current stream and releases any resources (such as sockets and file handles) associated with the current stream.
    /// </summary>
    public override void Close()
    {
      if (_isDisposed) throw new ObjectDisposedException(GetType().FullName);
      if (Socket != null && Socket.Connected)
        Socket.Close(); //TODO: Maybe use Disconnect with reuseSocket=true? I tried but it took forever.

      base.Close();
    }

    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="T:System.Net.Sockets.NetworkStream"/> and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected override void Dispose(bool disposing)
    {
      if (_isDisposed) return;

      try
      {
        if (disposing)
        {
          if (Socket != null && Socket.Connected)
          {
            try
            {
              Socket.Disconnect(true);
            }
            catch (ObjectDisposedException) { }
          }
        }
      }
      finally
      {
        base.Dispose(disposing);
        _isDisposed = true;
      }
    }
  }
}