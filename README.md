(Work In Progress) 
---------------
Enlistment 
==========
#### Prerequisite - Configure dotnet swagger 
From the exmaple APP project directory 
1. Run `dotnet new tool-manifest --force` to create a tool manifest file
2. Run `dotnet tool install --version 5.0.0 Swashbuckle.AspNetCore.Cli` to install swachbuckle cli as local tool

For more info check instructions here: [Swashbuckle.AspNetCore.Cli](https://github.com/domaindrivendev/Swashbuckle.AspNetCore#using-the-tool-with-the-net-core-30-sdk-or-later)

#### Compile and build
1. set a global json file
2. compile the project by running `dotnet build`. This will build the project and generate the swagger files
    * The generated swagger files should appear under `$(ProjectDir)\Docs\OpenApiSpecs`

References
==========
  * [main library repo](https://msazure.visualstudio.com/One/_git/AGCI-CSF?path=/src/Service/OpenApi/OpenApiServiceExtension.cs&_a=contents&version=GBmaster)
  * [swaggerGenerationLibrary](https://msazure.visualstudio.com/One/_git/AGE-Documents?path=%2Fdocs%2FCommon%2FswaggerGenerationLibrary.md&version=GBmaster&_a=preview)
  * [more custom swagger filters](https://msazure.visualstudio.com/One/_git/DI-Agri?path=/src/PaaS/src/csharp/BaseNetCoreApp/ServiceCollectionExtentions/Helpers)
  * [autorest/extensions](http://azure.github.io/autorest/extensions/)
  * [example](https://dev.azure.com/msazure/One/_git/DI-Agri/pullrequest/5144979?_a=files&path=/src/PaaS/src/csharp/ResourceProviderService/Docs/OpenApiSpecs/latest/semi_automated_swagger.json)
  * [PPT](https://microsoft-my.sharepoint.com/:p:/p/prjayasw/Ed7S0Ia9ZnVGhB1WQK16T5IBLsd4V_O-sxjizYcUuYjo8Q)

 
capability example
==================
https://dev.azure.com/msazure/One/_git/DI-Agri/pullrequest/5144979
https://msazure.visualstudio.com/One/_git/DI-Agri?path=/src/PaaS/src/csharp/ResourceProviderService/Docs/arm-swagger/agfood/resource-manager/Microsoft.AgFoodPlatform/preview/2020-05-12-preview/agfood.json


Usage
=================

add this code on your startup `ConfigureServices()` method [see usage](./samples/SomeWebApp/Startup.cs#L64)

```csharp
            var config = new SwaggerConfig
            {
                PolymorphicSchemaModels = new List<Type> { typeof(WeatherForecast) },
                ModelEnumsAsString = true,
                ReusableParameters = new Dictionary<string, Microsoft.OpenApi.Models.OpenApiParameter>()
                {
                    { "SubscriptionIdParameter", ArmReusableParameters.GetSubscriptionIdParameter() },
                    { "ResourceGroupNameParameter", ArmReusableParameters.GetResourceGroupNameParameter() },
                    { "ApiVersionParameter", ArmReusableParameters.GetApiVersionParameter() }                    
                },               
                EnableSwaggerSecurityTokenSupport = true,
                XmlCommentFile = "SomeWebApp.xml"
            };

            services.AddArmCompliantSwagger(config);
```
and on `Configure()` method

add this at the beginning of the pipeline:
```csharp
    app.UseSwagger(option =>
            {
                // Enable middleware to serve generated Swagger as a JSON endpoint.
                option.RouteTemplate = OpenApiOptions.JsonRoute;
                // Change generated swagger version to 2.0
                option.SerializeAsV2 = true;
            });

    app.UseSwaggerUI(option =>
    {
        option.SwaggerEndpoint(OpenApiOptions.UiEndpoint(), OpenApiOptions.Description);
    });
```

Swagger Enrichment
=================
part of the process is to enrich your Swagger generation with metadata from your code.  
here is a list of enrichments you should consider:
1. [Xml documentation](https://docs.microsoft.com/en-us/visualstudio/msbuild/common-msbuild-project-properties?view=vs-2019) file file on your .csproj, this would read your triple slash comment, translate it into XML file, and swashbuckle will take this file and embed the comments into the generated swagger.
2. [Swashbuckle annotations ](https://github.com/domaindrivendev/Swashbuckle.AspNetCore#swashbuckleaspnetcoreannotations)
3. [AGCI-CSF](https://msazure.visualstudio.com/One/_git/AGE-Documents?path=/docs/Common/swaggerGenerationLibrary.md&version=GBmaster&_a=preview) nuget annotations
4. AsiSwaggerExtensions annotations:
   1. ProducesContentTypeAttribute
   2. SwaggerHideParameterAttribute
   3. CustomSwaggerSchemaInheritanceAttribute
   4. AsiExampleAttribute
   5. CustomSwaggerSchemaIdAttribute
   6. CustomSwaggerGenericsSchemaNameStartegyAttribute
   7. LongRunningOperationAttribute
   8. HideInDocsAttribute 
   
