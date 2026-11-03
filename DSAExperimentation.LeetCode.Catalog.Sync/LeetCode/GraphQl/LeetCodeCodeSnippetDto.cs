namespace DSAExperimentation.LeetCode.Catalog.Sync.LeetCode.GraphQl;

internal sealed record LeetCodeCodeSnippetDto
{
    public required string LangSlug { get; init; }

    public required string Code { get; init; }
}
