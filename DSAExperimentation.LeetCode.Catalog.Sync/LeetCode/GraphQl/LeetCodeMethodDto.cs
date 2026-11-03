using System.Text.Json.Serialization;

namespace DSAExperimentation.LeetCode.Catalog.Sync.LeetCode.GraphQl;

internal sealed record LeetCodeMethodDto
{
    public required string Name { get; init; }

    [JsonPropertyName("params")]
    public List<LeetCodeParameterDto>? Parameters { get; init; }

    public LeetCodeReturnDto? Return { get; init; }
}
