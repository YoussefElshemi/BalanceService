using System.Text.Json;

namespace Core.Extensions;

public static class JsonDocumentExtensions
{
    public static bool BeValidJsonObject(this JsonDocument? json)
    {
        if (json != null)
        {
            return json.RootElement.ValueKind == JsonValueKind.Object;
        }

        return true;
    }
}