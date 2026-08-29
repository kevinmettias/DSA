using System.Text.Json.Serialization;

namespace DSAExperimentation.Tests.LeetCodeCatalog.Client;

// The outgoing POST body shape leetcode.com/graphql expects - property names here
// are load-bearing (the GraphQL server reads them literally), unlike the response
// DTOs' property names, which only need case-insensitive matching.
internal sealed record LeetCodeGraphQlRequestDto
{
    [JsonPropertyName("operationName")]
    public required string OperationName { get; init; }

    [JsonPropertyName("variables")]
    public required Dictionary<string, string> Variables { get; init; }

    [JsonPropertyName("query")]
    public required string Query { get; init; }
}
