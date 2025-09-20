using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Presentation.CustomBinding;

[AttributeUsage(AttributeTargets.Parameter)]
public sealed class FromHybridAttribute : ModelBinderAttribute
{
    public FromHybridAttribute()
        : base(typeof(HybridBinderFactory))
    {
        BindingSource = BindingSource.Custom;
    }
}