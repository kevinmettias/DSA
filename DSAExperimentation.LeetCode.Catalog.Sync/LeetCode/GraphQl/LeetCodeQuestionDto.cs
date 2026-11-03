namespace DSAExperimentation.LeetCode.Catalog.Sync.LeetCode.GraphQl;

// Mirrors leetcode.com/graphql's question(titleSlug:) field. MetaData, Stats and SimilarQuestions
// are themselves JSON-encoded STRINGS within the JSON - LeetCode's shape, not a choice made here -
// so LeetCodeQuestionMapper deserializes each a second time.
internal sealed record LeetCodeQuestionDto
{
    public required string QuestionId { get; init; }

    public required string QuestionFrontendId { get; init; }

    public required string Title { get; init; }

    public required string TitleSlug { get; init; }

    public required string Content { get; init; }

    public required string Difficulty { get; init; }

    public required string CategoryTitle { get; init; }

    public required int Likes { get; init; }

    public required int Dislikes { get; init; }

    public required bool IsPaidOnly { get; init; }

    public required double AcRate { get; init; }

    public required string Stats { get; init; }

    public required List<string> Hints { get; init; }

    public required string MetaData { get; init; }

    public required List<string> ExampleTestcaseList { get; init; }

    public required string SimilarQuestions { get; init; }

    public required List<LeetCodeTopicTagDto> TopicTags { get; init; }

    public List<LeetCodeCodeSnippetDto>? CodeSnippets { get; init; }
}
