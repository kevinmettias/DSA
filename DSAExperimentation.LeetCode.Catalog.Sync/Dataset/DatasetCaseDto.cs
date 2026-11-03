using System.Text.Json.Serialization;

namespace DSAExperimentation.LeetCode.Catalog.Sync.Dataset;

// One generated case: "nums = [3,3], target = 6" and what the reference solution printed for it.
internal sealed record DatasetCaseDto
{
    [JsonPropertyName("input")]
    public required string Input { get; init; }

    [JsonPropertyName("output")]
    public string? Output { get; init; }
}
