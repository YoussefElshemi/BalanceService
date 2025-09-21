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

        if (fromBodyProps.Count > 0)
        {
            request.EnableBuffering();

            using var reader = new StreamReader(request.Body, leaveOpen: true);
            var bodyString = await reader.ReadToEndAsync();
            request.Body.Position = 0;

            bodyDict = string.IsNullOrWhiteSpace(bodyString)
                ? new Dictionary<string, object?>()
                : JsonSerializer.Deserialize<Dictionary<string, object?>>(bodyString, jsonSerializerOptions);
        }

        foreach (var property in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var propertyType = property.PropertyType;
            var propertyName = property.Name;
            object? value = null;

            var fromRoute = property.GetCustomAttribute<FromRouteAttribute>();
            if (fromRoute != null)
            {
                var key = fromRoute.Name ?? propertyName;
                var routeVal = request.RouteValues
                    .FirstOrDefault(kvp => string.Equals(kvp.Key, key, caseSensitivity)).Value;

                value = ValueConverter.ConvertValue<T>(bindingContext, routeVal, propertyType, propertyName, propertyName);
            }

            var fromHeader = property.GetCustomAttribute<FromHeaderAttribute>();
            if (fromHeader != null)
            {
                var key = fromHeader.Name ?? propertyName;
                var headerValues = request.Headers
                    .FirstOrDefault(h => string.Equals(h.Key, key, caseSensitivity)).Value;

                var headerValue = headerValues.Count > 0
                    ? headerValues.ToString()
                    : null;

                value = ValueConverter.ConvertValue<T>(bindingContext, headerValue, propertyType, propertyName, propertyName);
            }

            var fromQuery = property.GetCustomAttribute<FromQueryAttribute>();
            if (fromQuery != null)
            {
                var key = fromQuery.Name ?? propertyName;
                var queryVal = request.Query
                    .FirstOrDefault(q => string.Equals(q.Key, key, caseSensitivity)).Value;

                if (!string.IsNullOrEmpty(queryVal))
                {
                    value = ValueConverter.ConvertValue<T>(bindingContext, queryVal.ToString(), propertyType, propertyName, propertyName);
                }
            }

            var fromForm = property.GetCustomAttribute<FromFormAttribute>();
            if (fromForm != null && request.HasFormContentType)
            {
                var key = fromForm.Name ?? propertyName;
                var formVal = request.Form
                    .FirstOrDefault(f => string.Equals(f.Key, key, caseSensitivity)).Value;

                value = ValueConverter.ConvertValue<T>(bindingContext, formVal.ToString(), propertyType, propertyName, propertyName);
            }

            var fromBody = property.GetCustomAttribute<FromBodyAttribute>();
            if (fromBody != null && bodyDict != null)
            {
                var fieldName = jsonSerializerOptions.PropertyNamingPolicy?.ConvertName(property.Name) ?? property.Name;
                bodyDict.TryGetValue(fieldName, out var bodyVal);

                if (bodyVal is JsonElement jsonElement)
                {
                    value = JsonSerializer.Deserialize(jsonElement.GetRawText(), property.PropertyType, jsonSerializerOptions);
                }
                else
                {
                    value = ValueConverter.ConvertValue<T>(bindingContext, bodyVal, property.PropertyType, property.Name, fieldName);
                }
            }

            if (value != null)
            {
                property.SetValue(dto, value);
            }
        }

        bindingContext.Result = ModelBindingResult.Success(dto);
    }
}