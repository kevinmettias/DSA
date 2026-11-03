using System.Text.Json.Serialization;

namespace DSAExperimentation.LeetCode.Catalog.Sync.Dataset;

// One line of LeetCodeDataset's JSONL: a question, by slug, and its generated cases. The line also
// carries the statement, a reference solution and prompts, none of which the catalog keeps.
internal sealed record DatasetRowDto
{
    [JsonPropertyName("task_id")]
    public required string TitleSlug { get; init; }

    [JsonPropertyName("input_output")]
    public required List<DatasetCaseDto> Cases { get; init; }
}
