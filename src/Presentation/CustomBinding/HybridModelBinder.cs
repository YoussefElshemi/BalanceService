using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Presentation.CustomBinding;

public class HybridSourceBinder<T> : IModelBinder where T : new()
{
    private readonly JsonSerializerOptions? _jsonSerializerSettings = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var httpContext = bindingContext.HttpContext;
        var request = httpContext.Request;
        var dto = new T();

        foreach (var prop in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            object? value = null;

            var fromRoute = prop.GetCustomAttribute<FromRouteAttribute>();
            if (fromRoute != null)
            {
                var key = fromRoute.Name ?? prop.Name;
                var routeVal = request.RouteValues
                    .FirstOrDefault(kvp => string.Equals(kvp.Key, key, StringComparison.OrdinalIgnoreCase)).Value;

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
                    .FirstOrDefault(h => string.Equals(h.Key, key, StringComparison.OrdinalIgnoreCase)).Value;

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
                    .FirstOrDefault(q => string.Equals(q.Key, key, StringComparison.OrdinalIgnoreCase)).Value;

                if (!string.IsNullOrEmpty(queryVal))
                {
                    value = ValueConverter.ConvertValue(bindingContext, queryVal.ToString(), prop.PropertyType, prop.Name);
                }
            }

            var fromForm = prop.GetCustomAttribute<FromFormAttribute>();
            if (fromForm != null)
            {
                if (!request.HasFormContentType)
                {
                    continue;
                }

                var key = fromForm.Name ?? prop.Name;
                var formVal = request.Form
                    .FirstOrDefault(f => string.Equals(f.Key, key, StringComparison.OrdinalIgnoreCase)).Value;

                if (!string.IsNullOrEmpty(formVal))
                {
                    value = ValueConverter.ConvertValue(bindingContext, formVal.ToString(), prop.PropertyType, prop.Name);
                }
            }

            var fromBody = prop.GetCustomAttribute<FromBodyAttribute>();
            if (fromBody != null)
            {
                Dictionary<string, object?>? bodyDict = null;

                if (bodyDict == null && request.ContentLength > 0)
                {
                    request.EnableBuffering();

                    using var reader = new StreamReader(request.Body, leaveOpen: true);
                    var bodyString = await reader.ReadToEndAsync();
                    request.Body.Position = 0;

                    bodyDict = string.IsNullOrWhiteSpace(bodyString)
                        ? new Dictionary<string, object?>()
                        : JsonSerializer.Deserialize<Dictionary<string, object?>>(bodyString, _jsonSerializerSettings);
                }

                var key = prop.Name;
                var bodyVal = bodyDict?.FirstOrDefault(kvp => string.Equals(kvp.Key, key, StringComparison.OrdinalIgnoreCase)).Value;

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
