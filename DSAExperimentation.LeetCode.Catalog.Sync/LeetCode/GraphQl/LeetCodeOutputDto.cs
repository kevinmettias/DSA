using System.Text.Json.Serialization;

namespace DSAExperimentation.LeetCode.Catalog.Sync.LeetCode.GraphQl;

// metaData's "output" member: which input an in-place question's judge reads its answer back from.
internal sealed record LeetCodeOutputDto
{
    [JsonPropertyName("paramindex")]
    public int? ParameterIndex { get; init; }
}
