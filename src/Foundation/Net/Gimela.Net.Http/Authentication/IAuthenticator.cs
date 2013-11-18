using System;
using Gimela.Net.Http.Headers;

namespace Gimela.Net.Http.Authentication
{
  /// <summary>
  /// Authenticates requests
  /// </summary>
  public interface IAuthenticator
  {
    /// <summary>
    /// Authenticate request
    /// </summary>
    /// <param name="header">Authorization header send by web client</param>
    /// <param name="realm">Realm to authenticate in, typically a domain name.</param>
    /// <param name="httpVerb">HTTP Verb used in the request.</param>
    /// <returns><c>User</c> if authentication was successful; otherwise <c>null</c>.</returns>
    IAuthenticationUser Authenticate(AuthorizationHeader header, string realm, string httpVerb);

    /// <summary>
    /// Gets authenticator scheme
    /// </summary>
    /// <example>
    /// digest
    /// </example>
    string Scheme { get; }

    /// <summary>
    /// Create a authentication challenge.
    /// </summary>
    /// <param name="realm">Realm that the user should authenticate in</param>
    /// <returns>A WWW-Authenticate header.</returns>
    /// <exception cref="ArgumentNullException">If realm is empty or <c>null</c>.</exception>
    IHeader CreateChallenge(string realm);
  }
}