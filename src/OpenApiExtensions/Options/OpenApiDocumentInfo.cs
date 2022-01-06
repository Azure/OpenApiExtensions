using System;

namespace Microsoft.Azure.OpenApiExtensions.Options

{
    /// <summary>
    /// Corresponds to openAPI document info section.
    /// </summary>
    public class OpenApiDocumentInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenApiDocumentInfo"/> class.
        /// </summary>
        /// <param name="title">OpenAPI document title.</param>
        /// <param name="description">OpenAPI document description.</param>
        /// <param name="version">OpenAPI document version.</param>
        /// <param name="clientName">OpenAPI document clientName.</param>
        public OpenApiDocumentInfo(string title, string description, string version, string clientName)
        {
            if (string.IsNullOrEmpty(title))
            {
                throw new ArgumentNullException(nameof(title), "Can not be null or empty.");
            }

            if (string.IsNullOrEmpty(description))
            {
                throw new ArgumentNullException(nameof(description), "Can not be null or empty.");
            }

            if (string.IsNullOrEmpty(version))
            {
                throw new ArgumentNullException(nameof(version), "Can not be null or empty.");
            }

            if (string.IsNullOrEmpty(clientName))
            {
                throw new ArgumentNullException(nameof(clientName), "Can not be null or empty.");
            }

            Title = title;
            Description = description;
            Version = version;
            ClientName = clientName;
        }

        /// <summary>
        /// Corresponds to openAPI document info.title
        /// Autorest generated SDK client title.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Corresponds to openAPI document info.Description
        /// Autorest generated SDK client description.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Corresponds to openAPI document info.Version
        /// Autorest generated SDK client supported api verison.
        /// </summary>
        public string Version { get; }

        /// <summary>
        /// Corresponds to openAPI document info.x-ms-code-generation-settings.name
        /// Autorest generated SDK client name.
        /// </summary>
        public string ClientName { get; }
    }
}
