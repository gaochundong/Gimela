using System;
using System.Text.RegularExpressions;

namespace Gimela.Net.Http.Routing
{
  /// <summary>
  /// Class to make dynamic binding of redirects. Instead of having to specify a number of similar redirect rules
  /// a regular expression can be used to identify redirect URLs and their targets.
  /// </summary>
  /// <example>
  /// <![CDATA[
  /// new RegexRedirectRule("/(?<target>[a-z0-9]+)", "/users/${target}/?find=true", RegexOptions.IgnoreCase)
  /// ]]>
  /// </example>
  public class RegExRouter : SimpleRouter
  {
    private readonly Regex _matchUrl;

    /// <summary>
    /// Initializes a new instance of the <see cref="RegExRouter"/> class.
    /// </summary>
    /// <param name="fromUrlExpression">Expression to match URL</param>
    /// <param name="toUrlExpression">Expression to generate URL</param>
    /// <example>
    /// <![CDATA[
    /// server.Add(new RegexRedirectRule("/(?<first>[a-zA-Z0-9]+)", "/user/${first}"));
    /// Result of ie. /employee1 will then be /user/employee1
    /// ]]>
    /// </example>
    public RegExRouter(string fromUrlExpression, string toUrlExpression)
      : this(fromUrlExpression, toUrlExpression, RegexOptions.None, true)
    {
    }

    #region public RegexRedirectRule(string fromUrlExpression, string toUrlExpression, RegexOptions options)

    /// <summary>
    /// Initializes a new instance of the <see cref="RegExRouter"/> class.
    /// </summary>
    /// <param name="fromUrlExpression">Expression to match URL</param>
    /// <param name="toUrlExpression">Expression to generate URL</param>
    /// <param name="options">Regular expression options to use, can be <c>null</c></param>
    /// <example>
    /// <![CDATA[
    /// server.Add(new RegexRedirectRule("/(?<first>[a-zA-Z0-9]+)", "/user/{first}", RegexOptions.IgnoreCase));
    /// Result of ie. /employee1 will then be /user/employee1
    /// ]]>
    /// </example>
    public RegExRouter(string fromUrlExpression, string toUrlExpression, RegexOptions options)
      : this(fromUrlExpression, toUrlExpression, options, true)
    {
    }

    #endregion

    #region public RegexRedirectRule(string fromUrlExpression, string toUrlExpression, RegexOptions options, bool shouldRedirect)

    /// <summary>
    /// Initializes a new instance of the <see cref="RegExRouter"/> class.
    /// </summary>
    /// <param name="fromUrlExpression">Expression to match URL</param>
    /// <param name="toUrlExpression">Expression to generate URL</param>
    /// <param name="options">Regular expression options to apply</param>
    /// <param name="shouldRedirect"><c>true</c> if request should be redirected, <c>false</c> if the request URI should be replaced.</param>
    /// <example>
    /// <![CDATA[
    /// server.Add(new RegexRedirectRule("/(?<first>[a-zA-Z0-9]+)", "/user/${first}", RegexOptions.None));
    /// Result of ie. /employee1 will then be /user/employee1
    /// ]]>
    /// </example>
    /// <exception cref="ArgumentNullException">Argument is <c>null</c>.</exception>
    /// <seealso cref="SimpleRouter.ShouldRedirect"/>
    public RegExRouter(string fromUrlExpression, string toUrlExpression, RegexOptions options, bool shouldRedirect)
      :
          base(fromUrlExpression, toUrlExpression, shouldRedirect)
    {
      if (string.IsNullOrEmpty(fromUrlExpression))
        throw new ArgumentNullException("fromUrlExpression");
      if (string.IsNullOrEmpty(toUrlExpression))
        throw new ArgumentNullException("toUrlExpression");

      _matchUrl = new Regex(fromUrlExpression, options);
    }

    #endregion

    /// <summary>
    /// Process the incoming request.
    /// </summary>
    /// <param name="context">Request context.</param>
    /// <returns>Processing result.</returns>
    /// <exception cref="ArgumentNullException">If any parameter is <c>null</c>.</exception>
    public override ProcessingResult Process(RequestContext context)
    {
      if (context == null)
        throw new ArgumentNullException("context");

      IRequest request = context.Request;
      IResponse response = context.Response;

      // If a match is found
      if (_matchUrl.IsMatch(request.Uri.AbsolutePath))
      {
        // Return the replace result
        string resultUrl = _matchUrl.Replace(request.Uri.AbsolutePath, ToUrl);
        if (!ShouldRedirect)
        {
          request.Uri = new Uri(request.Uri, resultUrl);
          return ProcessingResult.Continue;
        }

        response.Redirect(resultUrl);
        return ProcessingResult.SendResponse;
      }

      return ProcessingResult.Continue;
    }
  }
}