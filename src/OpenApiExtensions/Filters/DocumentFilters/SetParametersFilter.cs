////-----------------------------------------------------------
//// Copyright (c) Microsoft Corporation.  All rights reserved.
////-----------------------------------------------------------

//using System.Collections.Generic;
//using Microsoft.OpenApi.Models;
//using Swashbuckle.AspNetCore.SwaggerGen;

//namespace Microsoft.Azure.OpenApiExtensions.Filters.DocumentFilters
//{
//    /// <summary>
//    /// Populates 'parameter' property with list of all the reusable parameters.
//    /// </summary>
//    public class SetParametersFilter : IDocumentFilter
//    {
//        private readonly IDictionary<string, OpenApiParameter> parameters;

//        /// <summary>
//        /// Initializes a new instance of the <see cref="SetParametersFilter"/> class.
//        /// </summary>
//        public SetParametersFilter()
//        {
//        }

//        /// <summary>
//        /// Initializes a new instance of the <see cref="SetParametersFilter"/> class.
//        /// </summary>
//        /// <param name="parameters">List of resuable parameters.</param>
//        public SetParametersFilter(IDictionary<string, OpenApiParameter> parameters)
//        {
//            this.parameters = parameters;
//        }

//        /// <summary>
//        /// Applies filter.
//        /// </summary>
//        /// <param name="swaggerDoc">OpenApiDocument.</param>
//        /// <param name="context">DocumentFilterContext.</param>
//        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
//        {
//            var reusableParameters = ReusableParameters.CreateReusableParameters(this.parameters);
//            swaggerDoc.Components.Parameters = reusableParameters.ParametersById;
//        }
//    }
//}
