using System.Text.Json.Nodes;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ProductAPI.Swagger.Filters
{
    /// <summary>
    /// Operation Filter for JSONPatch
    /// </summary>
    public class JsonPatchDocumentOperationFilter : IOperationFilter
    {
        /// <summary>
        /// Setup for JSONPatch Operation Filter along with applying its rules.
        /// </summary>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var patchDocType = context.ApiDescription.ParameterDescriptions
           .FirstOrDefault(p => p.Type.IsGenericType &&
                                p.Type.GetGenericTypeDefinition() == typeof(JsonPatchDocument<>));

            if (patchDocType == null)
            {
                return; // Not a JSON Patch operation
            }

            operation.RequestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["application/json-patch+json"] = new OpenApiMediaType
                {
                    Schema = new OpenApiSchema
                    {
                        Type = JsonSchemaType.Array,
                        Items = new OpenApiSchema
                        {
                            Type = JsonSchemaType.Object,
                            Required = new HashSet<string> { "op", "path" },
                            Properties = new Dictionary<string, IOpenApiSchema>
                            {
                                { 
                                    "op", 
                                    new OpenApiSchema 
                                    { 
                                        Type = JsonSchemaType.String, 
                                        Description = "The operation to perform.", 
                                        Enum = new List<JsonNode> 
                                        { 
                                            JsonValue.Create("add")!, 
                                            JsonValue.Create("remove")!, 
                                            JsonValue.Create("replace")!, 
                                            JsonValue.Create("move")!, 
                                            JsonValue.Create("copy")!, 
                                            JsonValue.Create("test")! 
                                        } 
                                    } 
                                },
                                { 
                                    "path", 
                                    new OpenApiSchema 
                                    { 
                                        Type = JsonSchemaType.String, 
                                        Description = "The JSON Pointer path to the target location." 
                                    } 
                                },
                                { 
                                    "from", 
                                    new OpenApiSchema 
                                    { 
                                        Type = JsonSchemaType.String, 
                                        Description = "A JSON Pointer path to the source location for 'move' or 'copy' operations." 
                                    } 
                                },
                                { 
                                    "value", 
                                    new OpenApiSchema 
                                    { 
                                        Description = "The value to be used for 'add', 'replace', or 'test' operations." 
                                        // Type is intentionally not specified to allow any type
                                    } 
                                }
                            }
                        },
                        Example = new JsonArray
                        {
                            new JsonObject
                            {
                                ["op"] = "replace",
                                ["path"] = "/name",
                                ["value"] = "New Product Name"
                            }
                        }
                    }
                }
            }
            };
        }
    }
}