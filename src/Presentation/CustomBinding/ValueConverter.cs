using System.Collections;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Presentation.CustomBinding;

public static class ValueConverter
{
    public static object? ConvertValue(ModelBindingContext bindingContext, object? raw, Type targetType, string propertyName)
    {
        var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

        if (raw == null)
        {
            if (Nullable.GetUnderlyingType(targetType) != null || !underlyingType.IsValueType)
            {
                return null;
            }

            bindingContext.ModelState.AddModelError(propertyName, $"Cannot convert null to {targetType.Name}.");
            return null;
        }

        try
        {
            if (underlyingType == typeof(string))
            {
                return raw.ToString();
            }

            if (underlyingType == typeof(char))
            {
                var s = raw.ToString();
                if (string.IsNullOrEmpty(s))
                {
                    bindingContext.ModelState.AddModelError(propertyName, "Cannot convert empty string to char.");
                    return null;
                }

                if (s.Length > 1)
                {
                    bindingContext.ModelState.AddModelError(propertyName, "Cannot convert string to char.");
                    return null;
                }

                return s[0];
            }

            if (underlyingType == typeof(bool))
            {
                var s = raw is bool b
                    ? b.ToString()
                    : raw.ToString()!;

                if (s.Equals("1") || s.Equals("true", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                if (s.Equals("0") || s.Equals("false", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
                
                bindingContext.ModelState.AddModelError(propertyName, $"Cannot convert '{raw}' to bool.");
                return null;
            }

            if (underlyingType.IsPrimitive)
            {
                return Convert.ChangeType(raw, underlyingType);
            }

            if (underlyingType == typeof(decimal))
            {
                return raw is decimal d
                    ? d
                    : decimal.Parse(raw.ToString()!);
            }

            if (underlyingType == typeof(Guid))
            {
                return raw is Guid g 
                    ? g 
                    : Guid.Parse(raw.ToString()!);
            }

            if (underlyingType == typeof(DateTime))
            {
                return raw is DateTime dt
                    ? dt
                    : DateTime.Parse(raw.ToString()!);
            }

            if (underlyingType == typeof(DateTimeOffset))
            {
                return raw is DateTimeOffset dto
                    ? dto
                    : DateTimeOffset.Parse(raw.ToString()!);
            }

            if (underlyingType == typeof(TimeSpan))
            {
                return raw is TimeSpan ts
                    ? ts
                    : TimeSpan.Parse(raw.ToString()!);
            }

            if (underlyingType == typeof(Uri))
            {
                return raw as Uri ?? new Uri(raw.ToString()!);
            }

            if (underlyingType == typeof(byte[]))
            {
                if (raw is byte[] bytes)
                {
                    return bytes;
                }
                
                var s = raw.ToString()!;
                
                try
                {
                    return Convert.FromBase64String(s);
                }
                catch
                {
                    bindingContext.ModelState.AddModelError(propertyName, $"Cannot convert '{s}' to byte[].");
                    return null;
                }
            }

            if (underlyingType == typeof(JsonDocument))
            {
                return raw is JsonDocument jd
                    ? jd
                    : JsonDocument.Parse(raw.ToString()!);
            }

            if (underlyingType.IsEnum)
            {
                return Enum.Parse(underlyingType, raw.ToString()!, ignoreCase: true);
            }

            if (underlyingType.IsArray && raw is IEnumerable enumerableRaw)
            {
                var elementType = underlyingType.GetElementType()!;
                var list = new List<object?>();
                
                foreach (var item in enumerableRaw)
                {
                    list.Add(ConvertValue(bindingContext, item, elementType, propertyName));
                }

                var array = Array.CreateInstance(elementType, list.Count);
                for (var i = 0; i < list.Count; i++)
                {
                    array.SetValue(list[i], i);
                }

                return array;
            }

            if (underlyingType.IsGenericType)
            {
                var genericDef = underlyingType.GetGenericTypeDefinition();
                var elementType = underlyingType.GetGenericArguments()[0];

                if (genericDef == typeof(List<>) || genericDef == typeof(HashSet<>))
                {
                    var listType = genericDef.MakeGenericType(elementType);
                    var collection = (IList)Activator.CreateInstance(listType)!;

                    if (raw is IEnumerable enumerableRaw2)
                    {
                        foreach (var item in enumerableRaw2)
                        {
                            collection.Add(ConvertValue(bindingContext, item, elementType, propertyName));
                        }
                    }

                    return collection;
                }

                if (genericDef == typeof(Dictionary<,>) && raw is IDictionary rawDict)
                {
                    var keyType = underlyingType.GetGenericArguments()[0];
                    var valueType = underlyingType.GetGenericArguments()[1];
                    var dictType = typeof(Dictionary<,>).MakeGenericType(keyType, valueType);
                    var dict = (IDictionary)Activator.CreateInstance(dictType)!;

                    foreach (var key in rawDict.Keys)
                    {
                        var convertedKey = ConvertValue(bindingContext, key, keyType, propertyName);
                        var convertedValue = ConvertValue(bindingContext, rawDict[key], valueType, propertyName);
                        dict[convertedKey!] = convertedValue;
                    }

                    return dict;
                }
            }
        }
        catch (Exception ex) when (ex is not FormatException)
        {
            bindingContext.ModelState.AddModelError(propertyName, $"Failed to convert '{raw}' to type {targetType.Name}");
            return null;
        }
        
        bindingContext.ModelState.AddModelError(propertyName, $"Failed to convert '{raw}' to type {targetType.Name}");
        return null;
    }
}