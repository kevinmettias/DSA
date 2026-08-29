namespace DSAExperimentation.Tests.LeetCodeCatalog.Client;

// Mirrors leetcode.com/graphql's `question(titleSlug:)` field shape directly -
// MetaData and Stats are themselves JSON-encoded STRINGS within this JSON
// (LeetCode's own API shape, not a choice made here), so they need a second
// deserialization pass - see LeetCodeQuestionMapper.cs.
internal sealed record LeetCodeQuestionDto
{
    public required string QuestionId { get; init; }
    public required string QuestionFrontendId { get; init; }
    public required string Title { get; init; }
    public required string TitleSlug { get; init; }
    public required string Content { get; init; }
    public required string Difficulty { get; init; }
    public required int Likes { get; init; }
    public required int Dislikes { get; init; }
    public required bool IsPaidOnly { get; init; }
    public required string ExampleTestcases { get; init; }
    public required string MetaData { get; init; }
    public required string Stats { get; init; }
    public required List<string> Hints { get; init; }
    public required List<LeetCodeTopicTagDto> TopicTags { get; init; }
}
