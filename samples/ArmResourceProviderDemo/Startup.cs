using ArmResourceProviderDemo.WebModels;
using AsiSwaggerExtensions.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Sentinel.ServiceHostingTools.SwaggerExtensions.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ArmResourceProviderDemo
{
    public class Startup
    {
        private readonly SwaggerConfig _swaggerConfig;
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            var genarateInternalSwagger = Environment.GetCommandLineArgs().Contains("--internal-swagger");
            var genarateExternalSwagger = !genarateInternalSwagger;
            var OdataReusableParameters = new List<string>() { "$filter", "$orderBy", "$skipToken", "$top" };
            _swaggerConfig = new SwaggerConfig
            {
                PolymorphicSchemaModels = new List<Type> { typeof(TrafficResource) },
                ModelEnumsAsString = true,
                GlobalCommonReusableParameters = new Dictionary<string, Microsoft.OpenApi.Models.OpenApiParameter>()
                {
                    { "SubscriptionIdParameter", ArmReusableParameters.GetSubscriptionIdParameter() },
                    { "ResourceGroupNameParameter", ArmReusableParameters.GetResourceGroupNameParameter() },
                    { "ApiVersionParameter", ArmReusableParameters.GetApiVersionParameter() }
                },
                ResourceProviderReusableParameters = OdataReusableParameters.Concat(new List<string> { "WorkspaceName" }).ToList(),
                HideParametersEnabled = genarateExternalSwagger,
                GenerateExternalSwagger = genarateExternalSwagger,
                XmlCommentFile = Assembly.GetExecutingAssembly().GetName().Name,
                SupportedApiVersions = new[] { "v1" }
            };
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson(c=>
            {
                c.SerializerSettings.Converters.Add(new ResourceJsonConverter<TrafficResource, TrafficBaseProperties>(
                    new Dictionary<string, Type>
                    {
                        { "Israel", typeof(TrafficIsraelProperties)},
                        { "India", typeof(TrafficIndiaProperties)}
                    }));
            });
            services.AddArmCompliantSwagger(_swaggerConfig);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger(options =>
            {
                options.RouteTemplate = "swagger/{documentName}/swagger.json";
                // Change generated swagger version to 2.0
                options.SerializeAsV2 = true;
            });

            app.UseSwaggerUI(option =>
            {
                IEnumerable<string> actualDocumentsToGenerate = _swaggerConfig.SupportedApiVersions;
                if (actualDocumentsToGenerate == null || !actualDocumentsToGenerate.Any())
                {
                    actualDocumentsToGenerate = new[] { _swaggerConfig.DefaultVersion };
                }
                actualDocumentsToGenerate.ToList().ForEach(v => option.SwaggerEndpoint($"/swagger/{v}/swagger.json", v));
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
