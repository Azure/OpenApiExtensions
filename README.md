(Work In Progress) 
---------------
Enlistment 
==========
#### Prerequisite - Configure dotnet swagger 
From the example APP project directory 
1. Run `dotnet new tool-manifest --force` to create a tool manifest file
2. Run `dotnet tool install --version 5.0.0 Swashbuckle.AspNetCore.Cli` to install swachbuckle cli as local tool

For more info check instructions here: [Swashbuckle.AspNetCore.Cli](https://github.com/domaindrivendev/Swashbuckle.AspNetCore#using-the-tool-with-the-net-core-30-sdk-or-later)

#### Compile and build
1. set a global json file
2. compile the project by running `dotnet build`. This will build the project and generate the swagger files
    * The generated swagger files should appear under `$(ProjectDir)\Docs\OpenApiSpecs`
 
Usage
=================

add this code on your startup `ConfigureServices()` method [see usage](./samples/BasicWebAppDemo/Startup.cs#L64)

```csharp
            // _swaggerConfig a class member of your Startup class
            _swaggerConfig = new SwaggerConfig
            {
                PolymorphicSchemaModels = new List<Type> { typeof(WeatherForecast) },
                ModelEnumsAsString = true,
                ReusableParameters = new Dictionary<string, Microsoft.OpenApi.Models.OpenApiParameter>()
                {
                    { "SubscriptionIdParameter", ArmReusableParameters.GetSubscriptionIdParameter() },
                    { "ResourceGroupNameParameter", ArmReusableParameters.GetResourceGroupNameParameter() },
                    { "ApiVersionParameter", ArmReusableParameters.GetApiVersionParameter() }                    
                },               
                EnableSwaggerSecurityTokenSupport = true                
            };

            services.AddAutorestCompliantSwagger(config);
```
and on `Configure()` method

add this at the beginning of the pipeline:
```csharp
  app.UseAutorestCompliantSwagger(_swaggerConfig);
```

Swagger Enrichment
=================
part of the process is to enrich your Swagger generation with metadata from your code.  
here is a list of enrichments you should consider:
1. [Xml documentation](https://docs.microsoft.com/en-us/visualstudio/msbuild/common-msbuild-project-properties?view=vs-2019) file file on your .csproj, this would read your triple slash comment, translate it into XML file, and swashbuckle will take this file and embed the comments into the generated swagger.
2. [Swashbuckle annotations ](https://github.com/domaindrivendev/Swashbuckle.AspNetCore#swashbuckleaspnetcoreannotations)
4. Attributes annotations (see [attributes folder](./src/OpenApiExtensions/Attributes)), some of which:
   1. ProducesContentTypeAttribute
   2. SwaggerHideParameterAttribute
   3. CustomSwaggerSchemaInheritanceAttribute
   4. ExampleAttribute
   5. CustomSwaggerSchemaIdAttribute
   6. CustomSwaggerGenericsSchemaNameStartegyAttribute
   7. LongRunningOperationAttribute
   8. HideInDocsAttribute 
   
