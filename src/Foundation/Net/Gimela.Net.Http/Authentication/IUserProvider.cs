using System.Security.Principal;

namespace Gimela.Net.Http.Authentication
{
  /// <summary>
  /// Provider returning user to be authenticated.
  /// </summary>
  public interface IUserProvider
  {
    /// <summary>
    /// Lookups the specified user
    /// </summary>
    /// <param name="userName">User name.</param>
    /// <param name="host">Typically web server domain name.</param>
    /// <returns>User if found; otherwise <c>null</c>.</returns>
    /// <remarks>
    /// User name can basically be anything. For instance name entered by user when using
    /// basic or digest authentication, or SID when using Windows authentication.
    /// </remarks>
    IAuthenticationUser Lookup(string userName, string host);

    /// <summary>
    /// Gets the principal to use.
    /// </summary>
    /// <param name="user">Successfully authenticated user.</param>
    /// <returns></returns>
    /// <remarks>
    /// Invoked when a user have successfully been authenticated.
    /// </remarks>
    /// <seealso cref="GenericPrincipal"/>
    /// <seealso cref="WindowsPrincipal"/>
    IPrincipal GetPrincipal(IAuthenticationUser user);
  }

  /// <summary>
  /// User information used during authentication process.
  /// </summary>
  public interface IAuthenticationUser
  {
    /// <summary>
    /// Gets or sets user name used during authentication.
    /// </summary>
    string Username { get; set; }

    /// <summary>
    /// Gets or sets unencrypted password.
    /// </summary>
    /// <remarks>
    /// Password as clear text. You could use <see cref="HA1"/> instead if your passwords
    /// are encrypted in the database.
    /// </remarks>
    string Password { get; set; }

    /// <summary>
    /// Gets or sets HA1 hash.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Digest authentication requires clear text passwords to work. If you
    /// do not have that, you can store a HA1 hash in your database (which is part of
    /// the Digest authentication process).
    /// </para>
    /// <para>
    /// A HA1 hash is simply a Md5 encoded string: "UserName:Realm:Password". The quotes should
    /// not be included. Realm is the currently requested Host (as in <c>Request.Headers["host"]</c>).
    /// </para>
    /// <para>
    /// Leave the string as <c>null</c> if you are not using HA1 hashes.
    /// </para>
    /// </remarks>
    string HA1 { get; set; }
  }
}
