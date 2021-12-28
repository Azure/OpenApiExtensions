
Resource Provider Demo
=============================

sometimes you might want to extern actual polymorphic resources on your API.  
in this demo, [`TrafficResource`](./WebModels/TrafficResource.cs) is the resource type, and it owns a genric ploymorphic property with the name `properties`.  
`properties` might be `TrafficIsraelProperties` or `TrafficIndiaProperties`.

The problem is that our csharp code neither `TrafficIndia` nor `TrafficIsrael` exist,  
in addition the "kind" (discriminator) lives on the parent class [`TrafficResource`](./WebModels/TrafficResource.cs) and not part of the polymorphic property (`TrafficIsraelProperties` or `TrafficIndiaProperties`).
Yet the swagger generated a defination for each. by using the [`AsiSwaggerVirtualInheritancesAttribute`](../../src/AsiSwaggerExtensions/Attributes/AsiSwaggerVirtualInheritencesAttribute.cs), and registering the `TrafficResource` as polymorfic type.  
The names `TrafficIndia` and `TrafficIsrael` are "figured out" by using by default a custom naming strategy: `GenericArgumentPropertiesSuffixRemover`  
take a deep look at : [`TrafficKindsVirtualInheritanceProvider`](./WebModels/TrafficKindsVirtualInheritanceProvider.cs) this class generates a class out of a template (generic class), and this new runtime generated class is regsitered to swagger.   
in this case we generate 2 classes :  
 *  `VirtualInheritecePropertiesWrapperTemplate<TrafficIsraelProperties>` => later to be named as **TrafficIsrael** on swagger
 *  `VirtualInheritecePropertiesWrapperTemplate<TrafficIndiaProperties>` => later to be named as **TrafficIndia** on swagger  
p.s. we could have used the  [`ResourceProxy<>`](./WebModels/ResourceProxy.cs) as template class as well (instead of `VirtualInheritecePropertiesWrapperTemplate<>`)

This is an example of the generated SWAGGER:

```json
...
  "definitions": {
    "TrafficBaseProperties": {
      "type": "object",
      "properties": {
        "baseProperty": {
          "format": "int32",
          "type": "integer"
        }
      }
    },
    "TrafficResource": {
      "required": [
        "kind"
      ],
      "type": "object",
      "properties": {
        "kind": {
          "type": "string"
        },
        "properties": {
          "type": "object",
          "allOf": [
            {
              "$ref": "#/definitions/TrafficBaseProperties"
            }
          ]
        },
        "id": {
          "type": "string"
        },
        "name": {
          "type": "string"
        },
        "etag": {
          "type": "string"
        }
      },
      "discriminator": "kind"
    },
    "TrafficIsraelProperties": {
      "type": "object",
      "properties": {
        "israelProperty": {
          "format": "int32",
          "type": "integer"
        },
        "baseProperty": {
          "format": "int32",
          "type": "integer"
        }
      }
    },
    "TrafficIsrael": {
      "type": "object",
      "allOf": [
        {
          "$ref": "#/definitions/TrafficResource"
        }
      ],
      "properties": {
        "properties": {
          "type": "object",
          "allOf": [
            {
              "$ref": "#/definitions/TrafficIsraelProperties"
            }
          ],
          "x-ms-client-flatten": true
        }
      },
      "x-ms-discriminator-value": "Israel"
    },
    "TrafficIndiaProperties": {
      "type": "object",
      "properties": {
        "indiaProperty": {
          "format": "int32",
          "type": "integer"
        },
        "baseProperty": {
          "format": "int32",
          "type": "integer"
        }
      }
    },
    "TrafficIndia": {
      "type": "object",
      "allOf": [
        {
          "$ref": "#/definitions/TrafficResource"
        }
      ],
      "properties": {
        "properties": {
          "type": "object",
          "allOf": [
            {
              "$ref": "#/definitions/TrafficIndiaProperties"
            }
          ],
          "x-ms-client-flatten": true
        }
      },
      "x-ms-discriminator-value": "India"
    }
  },

...

```
