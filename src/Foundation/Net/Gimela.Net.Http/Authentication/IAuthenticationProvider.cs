
namespace Gimela.Net.Http.Authentication
{
  /// <summary>
  /// Used to authenticate users 
  /// </summary>
  /// <remarks>
  /// Authentication is requested by throwing 
  /// </remarks>
  public interface IAuthenticationProvider
  {
    /// <summary>
    /// Requests the authentication.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="realm">The realm.</param>
    void RequestAuthentication(IHttpContext context, string realm);
  }
}
