namespace DSAExperimentation.Tests.LeetCodeCatalog.Client;

internal sealed record LeetCodeGraphQlDataDto
{
    public LeetCodeQuestionDto? Question { get; init; }
}
