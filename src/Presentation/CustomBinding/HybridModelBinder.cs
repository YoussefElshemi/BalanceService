using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;

namespace Presentation.CustomBinding;

public class HybridSourceBinder<T> : IModelBinder where T : new()
{
    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var httpContext = bindingContext.HttpContext;
        var request = httpContext.Request;
        var dto = new T();

        var jsonSerializerOptions = httpContext.RequestServices.GetService<IOptions<JsonOptions>>()?.Value.JsonSerializerOptions
                                    ?? new JsonSerializerOptions(JsonSerializerDefaults.Web);

        var caseSensitivity = jsonSerializerOptions.PropertyNameCaseInsensitive
            ? StringComparison.OrdinalIgnoreCase
            : StringComparison.Ordinal;

        Dictionary<string, object?>? bodyDict = null;
        var fromBodyProps = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                     .Where(p => p.GetCustomAttribute<FromBodyAttribute>() != null)
                                     .ToList();

        if (fromBodyProps.Count != 0 && request.ContentLength > 0)
        {
            request.EnableBuffering();

            using var reader = new StreamReader(request.Body, leaveOpen: true);
            var bodyString = await reader.ReadToEndAsync();
            request.Body.Position = 0;

            bodyDict = string.IsNullOrWhiteSpace(bodyString)
                ? new Dictionary<string, object?>()
                : JsonSerializer.Deserialize<Dictionary<string, object?>>(bodyString, jsonSerializerOptions);
        }

        foreach (var prop in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            object? value = null;

            var fromRoute = prop.GetCustomAttribute<FromRouteAttribute>();
            if (fromRoute != null)
            {
                var key = fromRoute.Name ?? prop.Name;
                var routeVal = request.RouteValues
                    .FirstOrDefault(kvp => string.Equals(kvp.Key, key, caseSensitivity)).Value;

                if (routeVal != null)
                {
                    value = ValueConverter.ConvertValue(bindingContext, routeVal, prop.PropertyType, prop.Name);
                }
            }

            var fromHeader = prop.GetCustomAttribute<FromHeaderAttribute>();
            if (fromHeader != null)
            {
                var key = fromHeader.Name ?? prop.Name;
                var headerVal = request.Headers
                    .FirstOrDefault(h => string.Equals(h.Key, key, caseSensitivity)).Value;

                if (!string.IsNullOrEmpty(headerVal))
                {
                    value = ValueConverter.ConvertValue(bindingContext, headerVal.ToString(), prop.PropertyType, prop.Name);
                }
            }

            var fromQuery = prop.GetCustomAttribute<FromQueryAttribute>();
            if (fromQuery != null)
            {
                var key = fromQuery.Name ?? prop.Name;
                var queryVal = request.Query
                    .FirstOrDefault(q => string.Equals(q.Key, key, caseSensitivity)).Value;

                if (!string.IsNullOrEmpty(queryVal))
                {
                    value = ValueConverter.ConvertValue(bindingContext, queryVal.ToString(), prop.PropertyType, prop.Name);
                }
            }

            var fromForm = prop.GetCustomAttribute<FromFormAttribute>();
            if (fromForm != null && request.HasFormContentType)
            {
                var key = fromForm.Name ?? prop.Name;
                var formVal = request.Form
                    .FirstOrDefault(f => string.Equals(f.Key, key, caseSensitivity)).Value;

                if (!string.IsNullOrEmpty(formVal))
                {
                    value = ValueConverter.ConvertValue(bindingContext, formVal.ToString(), prop.PropertyType, prop.Name);
                }
            }

            var fromBody = prop.GetCustomAttribute<FromBodyAttribute>();
            if (fromBody != null && bodyDict != null)
            {
                var bodyVal = bodyDict.FirstOrDefault(kvp =>
                    string.Equals(
                        kvp.Key,
                        jsonSerializerOptions.PropertyNamingPolicy?.ConvertName(prop.Name) ?? prop.Name,
                        caseSensitivity
                    )
                ).Value;

                if (bodyVal != null)
                {
                    value = ValueConverter.ConvertValue(bindingContext, bodyVal, prop.PropertyType, prop.Name);
                }
            }

            if (value != null)
            {
                prop.SetValue(dto, value);
            }
        }

        bindingContext.Result = ModelBindingResult.Success(dto);
    }
}
