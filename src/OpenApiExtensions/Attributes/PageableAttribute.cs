using System;

namespace Microsoft.Azure.OpenApiExtensions.Attributes
{
    /// <summary>
    /// The REST API guidelines define a common pattern for paging through lists of data.
    /// The operation response is modeled in OpenAPI as a list of items (a "page") and a link to the next page,
    /// effectively resembling a singly linked list. Tag the operation as x-ms-pageable and the generated code will
    /// include methods for navigating between pages.
    /// </summary>
    /// <see hre="https://github.com/Azure/autorest/tree/master/docs/extensions#x-ms-pageable">x-ms-pageable.</see>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class PageableAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets optional (default: value). Specifies the name of the property that provides the collection of pageable items.
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// Gets or sets required. Specifies the name of the property that provides the next link (common: nextLink).
        /// If the model does not have a next link property then specify null. This is useful for services that return
        /// an object that has an array referenced by itemName. The object is then flattened in a way that the array is
        /// directly returned, no paging is used. This provides a better client side API to the end user.
        /// </summary>
        public string NextLinkName { get; set; }

        /// <summary>
        /// Gets or sets optional (default: &lt;operationName&gt;Next). Specifies the name of the operation for retrieving the next page.
        /// </summary>
        public string OperationName { get; set; }
    }
}
