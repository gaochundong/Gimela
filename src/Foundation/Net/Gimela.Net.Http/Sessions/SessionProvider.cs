using System;
using Gimela.Net.Http.Messages;

namespace Gimela.Net.Http.Sessions
{
  /// <summary>
  /// Used to load/store sessions in the server.
  /// </summary>
  public class SessionProvider<TSession> where TSession : Session
  {
    [ThreadStatic]
    private static TSession _currentSession;
    private readonly Server _server;
    private readonly ISessionStore _store;
    private string _cookieName = "__session_id";

    /// <summary>
    /// Initializes a new instance of the <see cref="SessionProvider{TSession}"/> class.
    /// </summary>
    /// <param name="server">Web server that the provider is for..</param>
    /// <param name="store">Store to use.</param>
    public SessionProvider(Server server, ISessionStore store)
    {
      _server = server;
      _store = store;
      _server.RequestReceived += OnRequest;
      ResponseWriter.HeadersSent += OnHeaderSent;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SessionProvider{TSession}"/> class.
    /// </summary>
    /// <param name="server">The server.</param>
    /// <remarks>
    /// Uses a file store.
    /// </remarks>
    public SessionProvider(Server server)
    {
      _server = server;
      _server.BeforeProcessRequest += OnRequest;
      _store = new SessionFileStore(server.ServerName);
      ResponseWriter.HeadersSent += OnHeaderSent;
    }

    /// <summary>
    /// Gets current session
    /// </summary>
    /// <value>Session if set, otherwise <c>null</c>.</value>
    public static TSession CurrentSession
    {
      get { return _currentSession; }
    }

    /// <summary>
    /// Gets or sets the session life time in minutes.
    /// </summary>
    /// <value>The session life time.</value>
    public int SessionLifeTime { get; set; }

    /// <summary>
    /// Adds the session cookie.
    /// </summary>
    /// <param name="response">The response.</param>
    /// <param name="sessionId">The session id.</param>
    public void AddSessionCookie(IResponse response, string sessionId)
    {
      response.Cookies.Add(new ResponseCookie(_cookieName, sessionId, DateTime.Now.AddMinutes(SessionLifeTime)));
    }

    /// <summary>
    /// Deletes the specified id.
    /// </summary>
    /// <param name="id">The id.</param>
    public void Delete(string id)
    {
      if (_currentSession != null && _currentSession.SessionId == id)
        _currentSession = null;
      _store.Delete(id);
    }

    private void OnHeaderSent(object sender, EventArgs e)
    {
      if (_currentSession != null)
        _store.Save(_currentSession);
    }


    /// <summary>
    /// Loads a session for all requests that got the session cookie.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The RequestEventArgs instance containing the event data.</param>
    private void OnRequest(object sender, RequestEventArgs e)
    {
      _currentSession = null;
      RequestCookie cookie = e.Request.Cookies[_cookieName];
      if (cookie == null)
        return;

      string id = cookie.Value;
      _currentSession = (TSession)_store.Load(id);
      SessionLoaded(this, EventArgs.Empty);
      if (_currentSession != null)
        _store.Touch(_currentSession.SessionId);
    }

    /// <summary>
    /// Saves the specified session.
    /// </summary>
    /// <param name="session">The session.</param>
    public void Save(TSession session)
    {
      _currentSession = session;
      _store.Save(session);
    }

    /// <summary>
    /// A session have been loaded. Use <see cref="CurrentSession"/> to access it.
    /// </summary>
    public event EventHandler SessionLoaded = delegate { };
  }
}