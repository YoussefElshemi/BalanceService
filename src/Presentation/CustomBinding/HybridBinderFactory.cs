using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Presentation.CustomBinding;

public class HybridBinderFactory : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var binderType = typeof(HybridSourceBinder<>).MakeGenericType(bindingContext.ModelType);
        var binder = (IModelBinder)Activator.CreateInstance(binderType)!;
        return binder.BindModelAsync(bindingContext);
    }
}