using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SDLS.Services.ApiExceptions
{
    public class ApiException : Exception
    {
        public int StatusCode { get; set; }

        public object? Errors { get; set; }

        public ApiException(string message, int statusCode = 400, object? errors = null)
            : base(message)
        {
            StatusCode = statusCode;
            Errors = errors;
        }

        // ===== COMMON ERRORS =====

        public static ApiException BadRequest(string message = "Bad Request")
            => new ApiException(message, (int)HttpStatusCode.BadRequest);

        public static ApiException Unauthorized(string message = "Unauthorized")
            => new ApiException(message, (int)HttpStatusCode.Unauthorized);

        public static ApiException Forbidden(string message = "Forbidden")
            => new ApiException(message, (int)HttpStatusCode.Forbidden);

        public static ApiException NotFound(string message = "Not Found")
            => new ApiException(message, (int)HttpStatusCode.NotFound);

        public static ApiException Conflict(string message = "Conflict")
            => new ApiException(message, (int)HttpStatusCode.Conflict);

        public static ApiException PaymentRequired(string message = "Payment Required")
            => new ApiException(message, (int)HttpStatusCode.PaymentRequired);

        public static ApiException MethodNotAllowed(string message = "Method Not Allowed")
            => new ApiException(message, (int)HttpStatusCode.MethodNotAllowed);

        public static ApiException Internal(string message = "Internal Server Error")
            => new ApiException(message, (int)HttpStatusCode.InternalServerError);
    }
}
