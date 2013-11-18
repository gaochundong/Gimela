using System;
using System.Net;

namespace Gimela.Net.Http
{
    /// <summary>
    /// Something unexpected went wrong.
    /// </summary>
    public class InternalServerException : HttpException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InternalServerException"/> class.
        /// </summary>
        /// <param name="errMsg">Exception description.</param>
        public InternalServerException(string errMsg)
            : base(HttpStatusCode.InternalServerError, errMsg)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InternalServerException"/> class.
        /// </summary>
        /// <param name="errMsg">Exception description.</param>
        /// <param name="inner">Inner exception.</param>
        public InternalServerException(string errMsg, Exception inner)
            : base(HttpStatusCode.InternalServerError, errMsg, inner)
        {
        }
    }
}