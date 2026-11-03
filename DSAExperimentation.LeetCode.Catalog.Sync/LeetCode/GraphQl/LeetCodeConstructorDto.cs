using System.Text.Json.Serialization;

namespace DSAExperimentation.LeetCode.Catalog.Sync.LeetCode.GraphQl;

internal sealed record LeetCodeConstructorDto
{
    [JsonPropertyName("params")]
    public List<LeetCodeParameterDto>? Parameters { get; init; }
}
