namespace DSAExperimentation.LeetCode.Catalog.Sync.LeetCode.GraphQl;

// One entry of LeetCodeQuestionDto.SimilarQuestions' nested JSON string.
internal sealed record LeetCodeSimilarQuestionDto
{
    public required string Title { get; init; }

    public required string TitleSlug { get; init; }

    public required string Difficulty { get; init; }
}
