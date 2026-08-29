namespace DSAExperimentation.Tests.LeetCodeCatalog.Client;

internal sealed record LeetCodeGraphQlResponseDto
{
    public LeetCodeGraphQlDataDto? Data { get; init; }
}
