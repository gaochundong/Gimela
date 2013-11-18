using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Gimela.Net.Http.Transports
{
  /// <summary>
  /// Client X.509 certificate, X.509 chain, and any SSL policy errors encountered
  /// during the SSL stream creation
  /// </summary>
  public class ClientCertificate
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ClientCertificate"/> class.
    /// </summary>
    /// <param name="certificate">The certificate.</param>
    /// <param name="chain">Client security certificate chain.</param>
    /// <param name="sslPolicyErrors">Any SSL policy errors encountered during the SSL stream creation.</param>
    public ClientCertificate(X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
      Certificate = certificate;
      Chain = chain;
      SslPolicyErrors = sslPolicyErrors;
    }

    /// <summary>
    /// Client security certificate
    /// </summary>
    public X509Certificate Certificate { get; private set; }

    /// <summary>
    /// Client security certificate chain
    /// </summary>
    public X509Chain Chain { get; private set; }

    /// <summary>
    /// Any SSL policy errors encountered during the SSL stream creation
    /// </summary>
    public SslPolicyErrors SslPolicyErrors { get; private set; }
  }
}