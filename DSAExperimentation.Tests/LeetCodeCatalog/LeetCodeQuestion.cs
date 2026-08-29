namespace DSAExperimentation.Tests.LeetCodeCatalog;

// A point-in-time snapshot of one LeetCode question's public metadata - every
// field here is either read directly off leetcode.com's own GraphQL response
// (LeetCodeQuestionMapper.cs) or trivially derived from it (Url). Deliberately a
// plain data bag (required init-only properties, no behavior beyond Url) - this
// is a fetched/cached RECORD of what LeetCode reported, not a type with
// invariants of its own to protect.
internal sealed record LeetCodeQuestion
{
    public required int QuestionId { get; init; }
    public required int FrontendId { get; init; }
    public required string Title { get; init; }
    public required string TitleSlug { get; init; }
    public required LeetCodeDifficulty Difficulty { get; init; }
    public required IReadOnlyList<string> TopicTags { get; init; }
    public required double AcceptanceRatePercent { get; init; }
    public required long TotalAccepted { get; init; }
    public required long TotalSubmissions { get; init; }
    public required int Likes { get; init; }
    public required int Dislikes { get; init; }
    public required bool IsPaidOnly { get; init; }
    public required string ContentHtml { get; init; }
    public required IReadOnlyList<string> Hints { get; init; }
    public required IReadOnlyList<LeetCodeTestCase> ExampleTestCases { get; init; }
    public required DateTimeOffset FetchedAtUtc { get; init; }

    public string Url => $"https://leetcode.com/problems/{TitleSlug}/";
}
