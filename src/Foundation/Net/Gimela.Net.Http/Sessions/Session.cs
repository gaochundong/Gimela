using System;

namespace Gimela.Net.Http.Sessions
{
  /// <summary>
  /// Session in the system
  /// </summary>
  [Serializable]
  public class Session
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="Session"/> class.
    /// </summary>
    /// <param name="sessionId">The session id.</param>
    public Session(string sessionId)
    {
      SessionId = sessionId;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Session"/> class.
    /// </summary>
    public Session()
    {
      SessionId = Guid.NewGuid().ToString("N");
    }

    /// <summary>
    /// Gets or sets session id.
    /// </summary>
    public string SessionId { get; set; }
  }
}
