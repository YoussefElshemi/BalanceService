using System.Collections;
using System.ComponentModel;
using System.Numerics;
using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Presentation.CustomBinding;

public static class ValueConverter
{
    private static readonly Dictionary<Type, Func<object, object?>> DirectConverters = new()
    {
        [typeof(string)] = raw => raw.ToString(),
        [typeof(char)] = raw => raw is char c ? c : char.Parse(raw.ToString()!),
        [typeof(bool)] = raw => raw is bool b ? b : bool.Parse(raw.ToString()!),
        [typeof(decimal)] = raw => raw is decimal d ? d : decimal.Parse(raw.ToString()!),
        [typeof(Guid)] = raw => raw is Guid g ? g : Guid.Parse(raw.ToString()!),
        [typeof(DateTime)] = raw => raw is DateTime dt ? dt : DateTime.Parse(raw.ToString()!),
        [typeof(DateTimeOffset)] = raw => raw is DateTimeOffset dto ? dto : DateTimeOffset.Parse(raw.ToString()!),
        [typeof(TimeSpan)] = raw => raw is TimeSpan ts ? ts : TimeSpan.Parse(raw.ToString()!),
        [typeof(Uri)] = raw => raw as Uri ?? new Uri(raw.ToString()!),
        [typeof(byte[])] = raw => raw as byte[] ?? Convert.FromBase64String(raw.ToString()!),
        [typeof(JsonDocument)] = raw => raw as JsonDocument ?? JsonDocument.Parse(raw.ToString()!),
        [typeof(BigInteger)] = raw => raw is BigInteger bi ? bi : BigInteger.Parse(raw.ToString()!),
        [typeof(Version)] = raw => raw as Version ?? Version.Parse(raw.ToString()!),
        [typeof(DateOnly)] = raw => raw is DateOnly d ? d : DateOnly.Parse(raw.ToString()!),
        [typeof(TimeOnly)] = raw => raw is TimeOnly t ? t : TimeOnly.Parse(raw.ToString()!)
    };

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
            if (DirectConverters.TryGetValue(underlyingType, out var converter))
            {
                return converter(raw);
            }

            if (underlyingType.IsEnum)
            {
                return Enum.Parse(underlyingType, raw.ToString()!, ignoreCase: true);
            }

            if (underlyingType.IsPrimitive)
            {
                return Convert.ChangeType(raw, underlyingType);
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
                var args = underlyingType.GetGenericArguments();

                if ((genericDef == typeof(List<>) || genericDef == typeof(HashSet<>)) && raw is IEnumerable enumerable2)
                {
                    var elementType = args[0];
                    var listType = genericDef.MakeGenericType(elementType);
                    var collection = (IList)Activator.CreateInstance(listType)!;

                    foreach (var item in enumerable2)
                    {
                        collection.Add(ConvertValue(bindingContext, item, elementType, propertyName));
                    }

                    return collection;
                }

                if (genericDef == typeof(Dictionary<,>) && raw is IDictionary rawDict)
                {
                    var keyType = args[0];
                    var valueType = args[1];
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

            var tryParse = underlyingType.GetMethod(
                "TryParse",
                BindingFlags.Public | BindingFlags.Static,
                [typeof(string), underlyingType.MakeByRefType()]);

            if (tryParse != null && tryParse.ReturnType == typeof(bool))
            {
                var parameters = new object?[] { raw.ToString(), null };
                var success = (bool)tryParse.Invoke(null, parameters)!;
                if (success)
                {
                    return parameters[1];
                }
            }

            var typeConverter = TypeDescriptor.GetConverter(underlyingType);
            if (typeConverter.CanConvertFrom(raw.GetType()))
            {
                return typeConverter.ConvertFrom(raw);
            }

            if (typeConverter.CanConvertFrom(typeof(string)))
            {
                return typeConverter.ConvertFrom(raw.ToString()!);
            }
        }
        catch (Exception ex)
        {
            bindingContext.ModelState.AddModelError(propertyName, $"Failed to convert '{raw}' to {targetType.Name}: {ex.Message}");
            return null;
        }

        bindingContext.ModelState.AddModelError(propertyName, $"Unsupported conversion to {targetType.Name}");
        return null;
    }
}
