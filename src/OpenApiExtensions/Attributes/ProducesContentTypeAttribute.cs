using System;
using System.Net.Http.Headers;

namespace Microsoft.Azure.OpenApiExtensions.Attributes
{
    /// <summary>
    /// Used to set additional content types to response.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class ProducesContentTypeAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProducesContentTypeAttribute"/> class.
        /// </summary>
        /// <param name="contentType">Content type of response.</param>
        /// <param name="statusCode">HTTP status codes of response.</param>
        public ProducesContentTypeAttribute(string contentType, int statusCode)
        {
            this.ContentType = contentType;

            // Validate input content type.
            MediaTypeHeaderValue.Parse(contentType);

            this.StatusCode = statusCode;
        }

        /// <summary>
        /// Gets the HTTP status codes of response.
        /// </summary>
        public int StatusCode { get; }

        /// <summary>
        /// Gets the content type of response.
        /// </summary>
        public string ContentType { get; }
    }
}
