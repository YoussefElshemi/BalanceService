using System.Collections;
using System.ComponentModel;
using System.Net.Mime;
using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Presentation.CustomBinding;

public class HybridOperationFilter(IOptions<JsonOptions> jsonOptions) : IOperationFilter
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = jsonOptions.Value.JsonSerializerOptions;

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var toRemove = new List<OpenApiParameter>();

        foreach (var param in context.ApiDescription.ParameterDescriptions)
        {
            var hasHybrid = param.CustomAttributes().Any(a => a is FromHybridAttribute);
            if (!hasHybrid)
            {
                continue;
            }

            var existing = operation.Parameters
                .FirstOrDefault(p => p.Name == param.Name && p.In == ParameterLocation.Query);
            
            if (existing != null)
            {
                toRemove.Add(existing);
            }

            var bodyProps = new List<PropertyInfo>();

            foreach (var prop in param.Type.GetProperties())
            {
                if (prop.GetCustomAttribute<FromQueryAttribute>() != null)
                {
                    var existingQuery = operation.Parameters
                        .FirstOrDefault(p => p.Name.Equals(prop.Name, StringComparison.OrdinalIgnoreCase) &&
                                             p.In == ParameterLocation.Query);

                    if (existingQuery != null)
                    {
                        toRemove.Add(existingQuery);
                    }

                    var defaultValue = GetDefaultValue(prop);

                    var queryParamParam = context.ApiDescription.ParameterDescriptions
                        .FirstOrDefault(p => p.Source == BindingSource.Query &&
                                             string.Equals(p.Name, prop.Name, StringComparison.OrdinalIgnoreCase));

                    var queryParamName = prop.GetCustomAttribute<FromQueryAttribute>()?.Name ?? queryParamParam?.Name ?? prop.Name;

                    operation.Parameters.Add(new OpenApiParameter
                    {
                        Name = queryParamName,
                        In = ParameterLocation.Query,
                        Required = !prop.IsNullable() || prop.IsRequired(),
                        Schema = context.SchemaGenerator.GenerateSchema(prop.PropertyType, context.SchemaRepository),
                        Example = GetOpenApiDefaultValue(defaultValue, prop.PropertyType)
                    });
                }

                if (prop.GetCustomAttribute<FromRouteAttribute>() != null)
                {
                    var existingRoute = operation.Parameters
                        .FirstOrDefault(p => p.Name.Equals(prop.Name, StringComparison.OrdinalIgnoreCase) &&
                                             p.In == ParameterLocation.Path);

                    if (existingRoute != null)
                    {
                        toRemove.Add(existingRoute);
                    }

                    var defaultValue = GetDefaultValue(prop);

                    var routeParam = context.ApiDescription.ParameterDescriptions
                        .FirstOrDefault(p => p.Source == BindingSource.Path &&
                                             string.Equals(p.Name, prop.Name, StringComparison.OrdinalIgnoreCase));

                    var routeName = prop.GetCustomAttribute<FromRouteAttribute>()?.Name ?? routeParam?.Name ?? prop.Name;

                    operation.Parameters.Add(new OpenApiParameter
                    {
                        Name = routeName,
                        In = ParameterLocation.Path,
                        Required = true,
                        Schema = context.SchemaGenerator.GenerateSchema(prop.PropertyType, context.SchemaRepository),
                        Example = GetOpenApiDefaultValue(defaultValue, prop.PropertyType)
                    });
                }

                if (prop.GetCustomAttribute<FromHeaderAttribute>() != null)
                {
                    var existingHeader = operation.Parameters
                        .FirstOrDefault(p => p.Name.Equals(prop.Name, StringComparison.OrdinalIgnoreCase) &&
                                             p.In == ParameterLocation.Header);

                    if (existingHeader != null)
                    {
                        toRemove.Add(existingHeader);
                    }
                    
                    var headerParam = context.ApiDescription.ParameterDescriptions
                        .FirstOrDefault(p => p.Source == BindingSource.Header &&
                                             string.Equals(p.Name, prop.Name, StringComparison.OrdinalIgnoreCase));

                    var headerName = prop.GetCustomAttribute<FromHeaderAttribute>()?.Name ?? headerParam?.Name ?? prop.Name;

                    var defaultValue = GetDefaultValue(prop);

                    operation.Parameters.Add(new OpenApiParameter
                    {
                        Name = headerName,
                        In = ParameterLocation.Header,
                        Required = !prop.IsNullable() || prop.IsRequired(),
                        Schema = context.SchemaGenerator.GenerateSchema(prop.PropertyType, context.SchemaRepository),
                        Example = GetOpenApiDefaultValue(defaultValue, prop.PropertyType)
                    });
                }

                if (prop.GetCustomAttribute<FromFormAttribute>() != null)
                {
                    operation.RequestBody ??= new OpenApiRequestBody
                    {
                        Content = new Dictionary<string, OpenApiMediaType>()
                    };

                    if (!operation.RequestBody.Content.TryGetValue(MediaTypeNames.Multipart.FormData, out var value))
                    {
                        value = new OpenApiMediaType
                        {
                            Schema = new OpenApiSchema
                            {
                                Type = "object",
                                Properties = new Dictionary<string, OpenApiSchema>()
                            }
                        };
                        operation.RequestBody.Content[MediaTypeNames.Multipart.FormData] = value;
                    }
                    
                    var defaultValue = GetDefaultValue(prop);
                    var propSchema = context.SchemaGenerator.GenerateSchema(prop.PropertyType, context.SchemaRepository);

                    propSchema.Default = GetOpenApiDefaultValue(defaultValue, prop.PropertyType);
                    value.Schema.Properties[prop.Name] = propSchema;

                    value.Schema.Properties[prop.Name] =
                        context.SchemaGenerator.GenerateSchema(prop.PropertyType, context.SchemaRepository);
                }

                if (prop.GetCustomAttribute<FromBodyAttribute>() != null)
                {
                    bodyProps.Add(prop);
                }
            }

            if (bodyProps.Any())
            {
                var schema = new OpenApiSchema
                {
                    Title = param.Type.Name,
                    Type = "object",
                    Properties = new Dictionary<string, OpenApiSchema>(),
                    Required = new HashSet<string>()
                };

                foreach (var bodyProp in bodyProps)
                {
                    var propSchema = context.SchemaGenerator.GenerateSchema(
                        bodyProp.PropertyType,
                        context.SchemaRepository
                    );

                    if (bodyProp.PropertyType.IsEnum)
                    {
                        propSchema.Enum = Enum.GetNames(bodyProp.PropertyType)
                            .Select(IOpenApiAny (n) => new OpenApiString(n)).ToList();
                    }

                    var defaultValue = GetDefaultValue(bodyProp);
                    propSchema.Default = GetOpenApiDefaultValue(defaultValue, bodyProp.PropertyType);
                    schema.Properties[GetParameterName(bodyProp.Name)] = propSchema;

                    if (bodyProp.IsRequired())
                    {
                        schema.Required.Add(bodyProp.Name);
                    }
                }

                context.SchemaRepository.Schemas[param.Type.Name] = schema;
                operation.RequestBody = new OpenApiRequestBody
                {
                    Required = true,
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        [MediaTypeNames.Application.Json] = new()
                        {
                            Schema = new OpenApiSchema
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.Schema,
                                    Id = param.Type.Name
                                }
                            }
                        }
                    }
                };
            }
        }

        foreach (var p in toRemove)
        {
            operation.Parameters.Remove(p);
        }
    }

    private string GetParameterName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return name;
        }

        return _jsonSerializerOptions.PropertyNamingPolicy != null
            ?  _jsonSerializerOptions.PropertyNamingPolicy.ConvertName(name)
            : name;
    }

    private static object? GetDefaultValue(PropertyInfo prop)
    {
        var defaultAttr = prop.GetCustomAttribute<DefaultValueAttribute>();
        return defaultAttr?.Value;
    }

    private static IOpenApiAny? GetOpenApiDefaultValue(object? value, Type type)
    {
        if (value == null)
        {
            return null;
        }

        type = Nullable.GetUnderlyingType(type) ?? type;

        if (type == typeof(string)) return new OpenApiString((string)value);
        if (type == typeof(char)) return new OpenApiString(value.ToString());
        if (type == typeof(TimeSpan)) return new OpenApiString(value.ToString());
        if (type == typeof(Guid)) return new OpenApiString(value.ToString());
        if (type == typeof(Uri)) return new OpenApiString(value.ToString());
        if (type.IsEnum) return new OpenApiString(value.ToString());
        if (type == typeof(bool)) return new OpenApiBoolean((bool)value);
        if (type == typeof(byte)) return new OpenApiByte((byte)value);
        if (type == typeof(short)) return new OpenApiInteger((short)value);
        if (type == typeof(ushort)) return new OpenApiInteger((ushort)value);
        if (type == typeof(int)) return new OpenApiInteger((int)value);
        if (type == typeof(uint)) return new OpenApiLong((uint)value);
        if (type == typeof(long)) return new OpenApiLong((long)value);
        if (type == typeof(ulong)) return new OpenApiLong((long)(ulong)value);
        if (type == typeof(float)) return new OpenApiFloat((float)value);
        if (type == typeof(double)) return new OpenApiDouble((double)value);
        if (type == typeof(decimal)) return new OpenApiDouble(Convert.ToDouble(value));
        if (type == typeof(DateTime)) return new OpenApiDateTime((DateTime)value);
        if (type == typeof(DateTimeOffset)) return new OpenApiDateTime(((DateTimeOffset)value).DateTime);

        if (typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string))
        {
            var arr = new OpenApiArray();
            foreach (var item in (IEnumerable)value)
            {
                arr.Add(GetOpenApiDefaultValue(item, item?.GetType() ?? typeof(object))!);
            }
            return arr;
        }

        var obj = new OpenApiObject();
        foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var propValue = prop.GetValue(value);
            obj[prop.Name] = GetOpenApiDefaultValue(propValue, prop.PropertyType) ?? new OpenApiNull();
        }

        return obj;
    }
}
