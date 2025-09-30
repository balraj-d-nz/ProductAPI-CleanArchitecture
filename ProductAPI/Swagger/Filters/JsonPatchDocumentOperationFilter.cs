using Microsoft.AspNetCore.JsonPatch;
using Microsoft.OpenApi.Models;
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
                Content =
            {
                ["application/json-patch+json"] = new OpenApiMediaType
                {
                    Schema = new OpenApiSchema
                    {
                        Type = "array",
                        Items = new OpenApiSchema
                        {
                            Type = "object",
                            Required = new HashSet<string> { "op", "path" },
                            Properties = new Dictionary<string, OpenApiSchema>
                            {
                                { "op", new OpenApiSchema { Type = "string", Description = "The operation to perform.", Enum = new List<Microsoft.OpenApi.Any.IOpenApiAny> { new Microsoft.OpenApi.Any.OpenApiString("add"), new Microsoft.OpenApi.Any.OpenApiString("remove"), new Microsoft.OpenApi.Any.OpenApiString("replace"), new Microsoft.OpenApi.Any.OpenApiString("move"), new Microsoft.OpenApi.Any.OpenApiString("copy"), new Microsoft.OpenApi.Any.OpenApiString("test") } } },
                                { "path", new OpenApiSchema { Type = "string", Description = "The JSON Pointer path to the target location." } },
                                { "from", new OpenApiSchema { Type = "string", Description = "A JSON Pointer path to the source location for 'move' or 'copy' operations." } },
                                { "value", new OpenApiSchema { Type = "any", Description = "The value to be used for 'add', 'replace', or 'test' operations." } }
                            }
                        },
                        Example = new Microsoft.OpenApi.Any.OpenApiArray
                        {
                            new Microsoft.OpenApi.Any.OpenApiObject
                            {
                                ["op"] = new Microsoft.OpenApi.Any.OpenApiString("replace"),
                                ["path"] = new Microsoft.OpenApi.Any.OpenApiString("/name"),
                                ["value"] = new Microsoft.OpenApi.Any.OpenApiString("New Product Name")
                            }
                        }
                    }
                }
            }
            };
        }
    }
}
