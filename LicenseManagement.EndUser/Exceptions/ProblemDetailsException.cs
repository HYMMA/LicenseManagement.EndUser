using System;
using System.Net;

namespace LicenseManagement.EndUser.Exceptions
{
    /// <summary>
    /// Thrown when the license server returned a non-2xx response with an RFC 7807 problem+json body.
    /// Carries the structured fields so callers can branch on <see cref="StatusCode"/> /
    /// <see cref="Type"/> / <see cref="Title"/> instead of grepping a free-form message.
    /// </summary>
    [Serializable]
    public class ProblemDetailsException : ApiException
    {
        public HttpStatusCode StatusCode { get; }
        public string Type { get; }
        public string Title { get; }
        public string Detail { get; }
        public string Instance { get; }
        public string CorrelationId { get; }

        public ProblemDetailsException(
            HttpStatusCode statusCode,
            string type,
            string title,
            string detail,
            string instance,
            string correlationId)
            : base(BuildMessage(title, detail, statusCode, correlationId), statusCode)
        {
            StatusCode = statusCode;
            Type = type;
            Title = title;
            Detail = detail;
            Instance = instance;
            CorrelationId = correlationId;
        }

        private static string BuildMessage(string title, string detail, HttpStatusCode status, string correlationId)
        {
            var msg = !string.IsNullOrEmpty(detail) ? detail : (title ?? "License server returned an error.");
            return correlationId != null
                ? $"{msg} (status={(int)status}, correlationId={correlationId})"
                : $"{msg} (status={(int)status})";
        }

        protected ProblemDetailsException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
