using System.Text.Json;

namespace DSAExperimentation.LeetCode.Catalog.Sync;

// How the sync tool reads JSON written by someone else - leetcode.com's GraphQL responses and
// the nested documents inside them, zerotrac's ratings, LeetCodeDataset's rows. Their members are
// camelCase or snake_case where the DTOs are PascalCase, so names match without regard to case;
// a DTO that needs an exact name says so with [JsonPropertyName].
internal static class WireJson
{
    public static JsonSerializerOptions Options { get; } = new() { PropertyNameCaseInsensitive = true };
}
