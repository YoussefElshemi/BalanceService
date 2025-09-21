using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Presentation.CustomBinding;

public static class ReflectionExtensions
{
    public static bool IsNullable(this PropertyInfo propertyInfo)
    {
        var type = propertyInfo.PropertyType;

        if (Nullable.GetUnderlyingType(type) != null)
        {
            return true;
        }

        if (!type.IsValueType)
        {
            var hasNullableAttr = propertyInfo.GetCustomAttribute<NullableAttribute>() != null;

            return hasNullableAttr;
        }

        return false;
    }
    
    public static bool IsRequired(this PropertyInfo propertyInfo)
    {
        var hasRequiredKeyword = propertyInfo.GetCustomAttribute<RequiredMemberAttribute>() != null;
        var hasRequiredAttr = propertyInfo.GetCustomAttribute<RequiredAttribute>() != null;

        return hasRequiredKeyword || hasRequiredAttr;
    }
}