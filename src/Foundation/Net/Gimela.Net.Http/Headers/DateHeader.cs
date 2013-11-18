using System;

namespace Gimela.Net.Http.Headers
{
  /// <summary>
  /// Header for "Date" and "If-Modified-Since"
  /// </summary>
  /// <remarks>
  /// <para>
  /// The field value is an HTTP-date, as described in section 3.3.1 in RFC2616;
  /// it MUST be sent in RFC 1123 [8]-date format. An example is
  ///<example>
  /// Date: Tue, 15 Nov 1994 08:12:31 GMT
  /// </example>
  ///</para><para>Origin servers MUST include a Date header field in all
  ///responses, except in these cases:
  ///<list type="number">
  /// <item>If the response status code is 100 (Continue) or 101 (Switching
  /// Protocols), the response MAY include a Date header field, at the server's
  /// option.
  ///</item><item>If the response status code conveys a server error, e.g. 500
  ///(Internal Server Error) or 503 (Service Unavailable), and it is inconvenient
  ///or impossible to generate a valid Date.
  ///</item><item>If the server does not have a clock that can provide a
  ///reasonable approximation of the current time, its responses MUST NOT include
  ///a Date header field. In this case, the rules in section 14.18.1 in RFC2616
  ///MUST be followed.
  /// </item>
  /// </list>
  ///</para><para>A received message that does not have a Date header field MUST
  ///be assigned one by the recipient if the message will be cached by that
  ///recipient or gatewayed via a protocol which requires a Date. An HTTP
  ///implementation without a clock MUST NOT cache responses without revalidating
  ///them on every use. An HTTP cache, especially a shared cache, SHOULD use a
  ///mechanism, such as NTP [28], to synchronize its clock with a reliable
  ///external standard.
  ///</para><para>Clients SHOULD only send a Date header field in messages that
  ///include an entity-body, as in the case of the PUT and POST requests, and
  ///even then it is optional. A client without a clock MUST NOT send a Date
  ///header field in a request.
  ///</para><para>The HTTP-date sent in a Date header SHOULD NOT represent a date
  ///and time subsequent to the generation of the message. It SHOULD represent
  ///the best available approximation of the date and time of message generation,
  ///unless the implementation has no means of generating a reasonably accurate
  ///date and time. In theory, the date ought to represent the moment just before
  ///the entity is generated. In practice, the date can be generated at any time
  ///during the message origination without affecting its semantic value.
  /// </para>
  /// </remarks>
  public class DateHeader : IHeader
  {
    /// <summary>
    /// Header name
    /// </summary>
    public const string DateName = "Date";

    /// <summary>
    /// Initializes a new instance of the <see cref="DateHeader"/> class.
    /// </summary>
    /// <param name="name">Header name.</param>
    /// <exception cref="ArgumentException">Name must not be empty.</exception>
    public DateHeader(string name)
    {
      if (string.IsNullOrEmpty(name))
        throw new ArgumentException("Name must not be empty.", "name");
      Name = name;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DateHeader"/> class.
    /// </summary>
    /// <param name="name">Header name.</param>
    /// <param name="value">Universal time.</param>
    public DateHeader(string name, DateTime value)
      : this(name)
    {
      Value = value;
    }

    /// <summary>
    /// Gets or sets date time.
    /// </summary>
    /// <value>Should be in UTC.</value>
    public DateTime Value { get; set; }

    /// <summary>
    /// Returns data formatted as a HTTP header value.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
    /// </returns>
    public override string ToString()
    {
      return HeaderValue;
    }

    #region IHeader Members

    /// <summary>
    /// Gets header name
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Gets value as it would be sent back to client.
    /// </summary>
    public string HeaderValue
    {
      get { return Value.ToString("r"); }
    }

    #endregion
  }
}