using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace OpenApiExtensions.Test.Integration
{
    internal class SwaggerDocument
    {
        public Dictionary<string, SwaggerPath> Paths  { get; set; }

        public Dictionary<string, object> Definitions { get; set; }

        public SwaggerDocumentInfo Info { get; set; }

    }

    class SwaggerDocumentInfo
    {
        public string Version { get; set; }
    }

    class SwaggerPath : Dictionary<string, SwaggerOperation>
    {


    }

    class SwaggerOperation
    {
        public JObject[] Parameters { get; set; }
    }
}
