OpenApiExtensions
==================
OpenApiExtensions Nuget Package is providing out of the box, generating rich Swagger files out of your Aspnet Api code. The Generated Swagger file is enriched by decorating your code with Attributes and comments, and therefore is able to generated a fully autorest compliant Swagger file (and request/response Example files).
the generation process is relying on [Swashbuckle](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)

# Getting Started

### Target Project Prerequisites
* "Aspnet core 3.1 or higher" for your WebApi 

How To use
=================

add this code on your startup `ConfigureServices()` method [see usage](./samples/BasicWebAppDemo/Startup.cs#L64)

```csharp
            var config = new SwaggerConfig
            {
                PolymorphicSchemaModels = new List<Type> { typeof(WeatherForecast) },
                ModelEnumsAsString = true,
                ReusableParameters = new Dictionary<string, Microsoft.OpenApi.Models.OpenApiParameter>()
                {
                    { "SomeReusableParam", ReusableParameters.GetSomeReusableParameter() },                    
                    { "ApiVersionParameter", ReusableParameters.GetApiVersionParameter() }                    
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


having these minimal Prerequisites are enough to generate a swagger file.  
when running your ArmApi on "localmode" you can now browse to   
`http://127.0.0.1:<<YOUR PORT>>/swagger/<<YOUR API VERSION>>/swagger.json`  
and get the file on your browser  
or just `http://127.0.0.1:<<YOUR PORT>>/swagger` to get the **swagger ui**.

## Enrichment
### [Code Enrichment](#code-enrich)
to be able to be fully compliant with Autorest Swagger standards, your code should be decorated with Comments and attributes.
### Enrich by Comments:
In order to enrich your swagger file from comments, you need to set msbuild [Xml documentation file](https://docs.microsoft.com/en-us/visualstudio/msbuild/common-msbuild-project-properties?view=vs-2019).  
upon build, the complier will read all your triple slash comments and save them into a XML file, which the swagger generation process will automatically read to enrich your Classes, Operations.
see [example](./samples/BasicWebAppDemo/WebModels/WeatherForecast.cs)

example:
```csharp
    /// <summary>
    /// Some Descriptive Summary Description (Reflected on your XML Comment -> and Swashbuckle read the        XmlDocumentation file and enrich the schemas , see https://github.com/domaindrivendev/Swashbuckle.AspNetCore#include-descriptions-from-xml-comments)
    /// </summary>
    abstract public class WeatherForecast
    {

    }
```

### Enrich by Attributes:
As the Swagger Generation is based on [Swashbuckle](https://github.com/domaindrivendev/Swashbuckle.AspNetCore), you may use any of the swashbuckle annotations.  
in addition to that you may refer to full list of provided annotation (attributes) by [this repo](OpenApiExtensions\Attributes)



you might also want to run the code tour of this repo by opening [this folder](.) with VSCode, and start the code tour.

### [Enriching your Controllers](#Enriching-your-Controllers)

#### ArmApi
```csharp
    [ApiVersion("2022-01-01-preview")]
    [Route("MyArmApiController")]
    [ApiController]
    [SwaggerApiVersionRange(fromVersion: "2022-01-01-preview")] // this tell the swagger generating to include this controller in any "2022-01-01-preview" and above (not mandatory)
    [SwaggerTag("ReadSomeResource")] // this would set up the tag for your controller
    public class MyArmApiController : ControllerBase
    {
        ....
    }
```
#### Internal Api
to avoid Internal Apis to be reflected in External Swagger docs use 
`[HideInDocs]` attribute to decorate either your Controllers or Actions
```csharp
    [ApiVersion("2022-01-01-preview")]
    [Route("InternalApi")]
    [HideInDocs]
    public class MyInternalApiController : ControllerBase
    {
        ....
    }
```

### [Enriching your Actions](#Enriching-your-Actions)

in this sample we provide the [SwaggerOperation] attribute that enrich the swagger model, and Request+Response + Example Providers.
```csharp

        [SwaggerOperation(
           Summary = "Get MyArmResources records",
           Description = "Fetches My Arm Resources",
           OperationId = "Get",
           Tags = new[] { "MyArmResources" })]
        [SwaggerResponse(200, "MyArmResources records fetched", typeof(SomeResourceList))]
        [ResponseExample(200, typeof(GetAllMyArmResourcesResponseExample))]
        [RequestExample(typeof(GetAllMyArmResourcesAsyncRequestExample))]
        [Pageable]        
        [HttpGet]
        public async Task<SomeResourceList> GetListOfResources(
            Guid subscriptionId,
            string resourceGroupName,
            string workspaceName,            
            [SwaggerHideParameter][FromHeader(Name = ArmHeaderConstants.InternalWorkspaceIdHeaderName)] Guid workspaceId,
            [LogIgnore] CancellationToken cancellationToken)
        {
          ...
        }
```

```csharp
        /// <param name="resourceId">Some Documentation to be shown on swagger</param>
        [SwaggerOperation(
           Summary = "Get MyArmResource record",
           Description = "Fetches My Arm Resource",
           OperationId = "Get",
           Tags = new[] { "MyArmResource" })]
        [SwaggerResponse(200, "MyArmResource fetched", typeof(MyArmResource))]
        [ResponseExample(200, typeof(GetMyArmResourceResponseExample))]
        [RequestExample(typeof(GetAllSomeResourceAsyncRequestExample))]          
        [HttpGet("{resourceId}")]
        public async Task<MyArmResource> GetSingleResource(
            Guid subscriptionId,
            string resourceGroupName,
            string workspaceName,            
            string resourceId,
            [SwaggerHideParameter][FromHeader(Name = ArmHeaderConstants.InternalWorkspaceIdHeaderName)] Guid workspaceId,
            [LogIgnore] CancellationToken cancellationToken)
        {
          ...
        }        
```
#### [Examples](#providing-example)
by specifying above your action these attributes : `[ResponseExample]` and `[RequestExample]`, examples will be auto generated for you. [see sample](samples\BasicWebAppDemo\Controllers\WeatherForecastController.cs#57)


### [Enriching your WebModels](#some-markdown-heading)
See [samples](./samples/] projects for more info  
for Virtual inheritance in your swagger file refer [this sample](samples\ArmResourceProviderDemo)  
(virtual inheritance is when you have polymorphism on your APIs, and the classes in your swagger doesnt exist on your code)

### Renaming your WebModels
sometimes you want to call your models differently on the swagger doc than your code.  
you have a few options to deal with this.
1. Renaming your class in code to match swagger, as WebModels class renaming is not a breaking change. you can do so if you can.
2. If you are not able to control your WebModels, in such cases when returning a generic Object, `MyObjectGenericsWrapper<T>`, you still can :
   1. Not using `MyObjectGenericsWrapper<T>` and use your own class.
   2. inherit from `MyObjectGenericsWrapper<T>` and return this inheritance from your controllers actions.
3. Use provided Swagger Attributes to control the naming:  
    here are some examples:
   * Basic constant renaming:
     ```csharp
      // in this example MyBoringClassResource would be named MySwaggerAwesomeName
       [SwaggerSchemaNameStrategy("MySwaggerAwesomeName")]
       public class MyBoringClassResource
       {
       }

     ```

   * Naming a generic wrapper:
    ```csharp        
        [SwaggerSchemaNameStrategy(NamingStrategy.ApplyToParentWrapper, "MySwaggerAwesomeNameModel")]
        public class MyResourceProperties
        {
        }


        public class MyObjectGenericsWrapper<T> // this class when used as MyObjectGenericsWrapper<MyResourceProperties> will be named in swagger as MySwaggerAwesomeNameModel
        {
        }
    ```

    ```csharp
    // in this example a Generic Wrapper that holds ConcreteModel would be named according to the logic of SwaggerAwesomeClassNameProvider
    [SwaggerSchemaNameStrategy(NamingStrategy.ApplyToParentWrapper, typeof(SwaggerAwesomeClassNameProvider) )]
    public class ConcreteModel
    {               
    }

    public class SwaggerAwesomeClassNameProvider : ICustomSchemaNameProvider
    {
        public string GetCustomName(Type type)
        {
            if (typeof(MyObjectGenericsWrapper<ConcreteModel>) == type)
            {
                return "ConcreteClass";
            }

            if (typeof(MyObjectsCollectionGenericsWrapper<ConcreteModel>) == type)
            {
                return "ConcreteClassesList";
            }

            throw new InvalidOperationException("Add more here ..");
        }
    }      
   ```
  <small>Open Api Extension also provide out of the box Custom name providers such as [ArmResourceWrapperNameProvider](OpenApiExtensions\Helpers\ArmResourceWrapperNameProvider.cs)</small>

### Models hierarchy
You might need to create a model hierarch in swagger that doesn't reflect your code. to do so, you can use the `CustomSwaggerSchemaInheritance` to create an object Hierarchy that doesn't reflect your code.  
see this example:

```csharp
    /// <summary>
    /// The Some Resource Properties model.
    /// </summary>    
    [ClientFlatten]    
    [CustomSwaggerSchemaInheritance(externalSchemaName: "MySwaggerOnlyBaseCLass", notInheritedPropertiesName: new[] { nameof(Properties) }, CommonObjectType.ResourceProviderCommonDefinition )]
    public class SomeResourceModel
    {
        /// <summary>
        /// Gets or sets nested level of properties which contains the resource content
        /// </summary>
        [JsonProperty("properties", Required = Required.Always)]
        public MyResourceProperties Properties { get; set; }
    }
```
will result this Definition in swagger:
```json
 "definitions": {
    "SomeResourceModel": {
      "description": "The Some Resource Properties model.",
      "required": [
        "properties"
      ],
      "type": "object",
      "allOf": [
        {
          "$ref": "../../../common/2.0/types.json#/definitions/MySwaggerOnlyBaseClass"
        }
      ],
      "properties": {
        "properties": {
          "description": "Gets or sets nested level of properties which contains the resource content",
          "type": "object",
          "allOf": [
            {
              "$ref": "#/definitions/MyResourceProperties"
            }
          ],
          "x-ms-client-flatten": true,          
        }
      }
    }
 }
```
### [Inheritance and Polymorphism](#Inheritance-and-Polymorphism)
Polymorphic in this context means, that your request or response is/contains a base (or abstract) class.  
this can lead to some challenges. in order to overcome it, OpenApiExtension has some out of the box solutions.  
for starters you need to register your polymorphic class in the SwaggerConfig:
   * in case of "Virtual Inheritance" - means the inherited classes are not in your code, please refer (to sample)[ArmResourceProviderDemo](samples\ArmResourceProviderDemo).
   * in other case you need to provide the `Discriminator` see [reference here](samples\BasicWebAppDemo\WebModels\WeatherForecast.cs)



### [Api version](#api-version)
Api version is supported out of box by reading the `[ApiVersion]` attributes on your Controllers.  
to support also the "Api Version Fallback" you may Provide the `[SwaggerApiVersionRangeAttribute]` as well, to specify ranges version of that your api supports.

## [Creating Swagger files for your entire Repo by Script](#gen-script)
### intro
 Since your code now contains everything that is needed to generate a well defined Swagger file, you can leverage an automation script that will:
1. generate your swagger file
2. "prettify" it so it would be valid for linters
3. Generate Autorest csharp SDK so you can observe it.
4. Validate that the swagger file is compliant

### Implement   
In order to support automate the process of generating and validating your swagger outcome, you need to set up 2 things:
   1.  **Setup the script configuration section**. 
   2.  a WebHost function that hosts the app, and the cli tool can call it to run the swagger pipeline. see [this](https://github.com/domaindrivendev/Swashbuckle.AspNetCore#use-the-cli-tool-with-a-custom-host-configuration) example for more info. 