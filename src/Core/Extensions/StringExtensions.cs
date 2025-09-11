using System.Text.Json;

namespace Core.Extensions;

public static class StringExtensions
{
    public static bool BeValidJsonObject(this string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return true;
        }

        try
        {
            var jsonDocument = JsonDocument.Parse(json);
            return jsonDocument.RootElement.ValueKind == JsonValueKind.Object;
        }
        catch
        {
            return false;
        }
    }
}