using SimpleKindArmResourceProviderDemo.WebModels;
using SimpleKindArmResourceProviderDemo.WebModels.Traffic;
using SimpleKindArmResourceProviderDemo.WebModels.Wind;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.OpenApiExtensions.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ParameterName = System.String;
using ActualParameterName = System.String;

namespace SimpleKindArmResourceProviderDemo
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
            
            _swaggerConfig = new SwaggerConfig
            {
                PolymorphicSchemaModels = new List<Type> { typeof(TrafficResource), typeof(WindResource) },
                ModelEnumsAsString = true,
                GlobalCommonReusableParameters = new Dictionary<string, Microsoft.OpenApi.Models.OpenApiParameter>()
                {
                   { "SomeGlobalParam", new OpenApiParameter {
                                    Description = "SomGlobalParam Description",
                                    Name = "testParamName",
                                    In = ParameterLocation.Path,
                                    Required = true,
                                    Schema = new OpenApiSchema()
                                    {
                                        Type = "string",
                                        MinLength = 1,
                                    },
                        }
                    }
                },
                ResourceProviderReusableParameters = new List<KeyValuePair<ParameterName, ActualParameterName>> {
                    new KeyValuePair<ParameterName, ActualParameterName>("WorkspaceName", "WorkspaceName") },
                HideParametersEnabled = genarateExternalSwagger,
                GenerateExternalSwagger = genarateExternalSwagger,                
                SupportedApiVersions = new[] { "v1" },
                GlobalCommonFilePath = "../../../../../Global/types.json",
                RPCommonFilePath = "../Demo/types.json",
                Title = "Arm Resource Provider Demo App",
                Description = "Arm Resource Provider Demo App",
                ClientName = "SimpleKindArmResourceProviderDemo"
            };
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson(c =>
            {              
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
                    actualDocumentsToGenerate = new[] { _swaggerConfig.DefaultApiVersion };
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
