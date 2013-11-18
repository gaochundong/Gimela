
namespace Gimela.Net.Http.Sessions
{
  /// <summary>
  /// Stores sessions in your favorite store
  /// </summary>
  /// <remarks>
  /// 
  /// </remarks>
  public interface ISessionStore
  {
    /// <summary>
    /// Saves the specified session.
    /// </summary>
    /// <param name="session">The session.</param>
    void Save(Session session);

    /// <summary>
    /// Touches the specified session
    /// </summary>
    /// <param name="id">Session id.</param>
    /// <remarks>
    /// Used to prevent sessions from expiring.
    /// </remarks>
    void Touch(string id);


    /// <summary>
    /// Loads a session 
    /// </summary>
    /// <param name="id">Session id.</param>
    /// <returns>Session if found; otherwise <c>null</c>.</returns>
    Session Load(string id);

    /// <summary>
    /// Delete a session
    /// </summary>
    /// <param name="id">Id of session</param>
    void Delete(string id);
  }
}
