using System.Text.Json.Serialization;

namespace DSAExperimentation.LeetCode.Catalog.Sync.LeetCode.GraphQl;

// The POST body leetcode.com/graphql expects. Its property names are load-bearing - the GraphQL
// server reads them literally - unlike the response DTOs', which only need case-insensitive
// matching.
internal sealed record LeetCodeGraphQlRequestDto
{
    [JsonPropertyName("operationName")]
    public required string OperationName { get; init; }

    [JsonPropertyName("variables")]
    public required Dictionary<string, string> Variables { get; init; }

    [JsonPropertyName("query")]
    public required string Query { get; init; }
}
