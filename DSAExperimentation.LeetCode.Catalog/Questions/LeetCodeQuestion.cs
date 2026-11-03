using System.Text.Json.Serialization;

namespace DSAExperimentation.LeetCode.Catalog.Questions;

// What LeetCode says about a question that changes only when LeetCode edits the question: its
// identity, filing, statement, signature and published examples. The figures that move daily are
// LeetCodeStatistics', and the contest rating comes from another source, so a weekly refresh
// leaves this file alone unless the question itself changed.
//
// Constraints are the statement's constraint lines as text, with <sup> written as ^ - the fact
// LeetCode states. The bounds a harness checks against are read from that text by
// LeetCodeBounds.TryParse when asked for, rather than stored beside it, so an improvement to the
// parser applies to every question at once and the two cannot disagree.
/// <summary>A LeetCode question as LeetCode publishes it: identity, topics, statement, signature and examples.</summary>
public sealed record LeetCodeQuestion
{
    private const string ProblemsUrlPrefix = "https://leetcode.com/problems/";

    /// <summary>LeetCode's internal question id.</summary>
    public required int QuestionId { get; init; }

    /// <summary>The number LeetCode shows beside the title, as in "1. Two Sum".</summary>
    public required int FrontendId { get; init; }

    /// <summary>The question's title.</summary>
    public required string Title { get; init; }

    /// <summary>The question's URL slug, which also names its files in the catalog.</summary>
    public required string TitleSlug { get; init; }

    /// <summary>The difficulty LeetCode files the question under.</summary>
    public required LeetCodeDifficulty Difficulty { get; init; }

    /// <summary>LeetCode's category for the question, such as "Algorithms" or "Database".</summary>
    public required string Category { get; init; }

    /// <summary>Whether the question needs a LeetCode subscription.</summary>
    public required bool IsPaidOnly { get; init; }

    /// <summary>The topic tags LeetCode files the question under.</summary>
    public required IReadOnlyList<LeetCodeTopic> Topics { get; init; }

    /// <summary>The questions LeetCode lists as similar.</summary>
    public required IReadOnlyList<LeetCodeRelatedQuestion> SimilarQuestions { get; init; }

    /// <summary>LeetCode's hints for the question, as HTML fragments.</summary>
    public required IReadOnlyList<string> Hints { get; init; }

    /// <summary>The statement's constraint lines as text, such as "2 &lt;= nums.length &lt;= 10^4".</summary>
    public required IReadOnlyList<string> Constraints { get; init; }

    /// <summary>The shape of the question's inputs and answer.</summary>
    public required LeetCodeSignature Signature { get; init; }

    /// <summary>LeetCode's C# starter code, which names the method and types its solution must have; null if none.</summary>
    public required string? CSharpStarterCode { get; init; }

    /// <summary>The question's published examples.</summary>
    public required IReadOnlyList<LeetCodeCase> Examples { get; init; }

    /// <summary>The question's statement as LeetCode's HTML.</summary>
    public required string ContentHtml { get; init; }

    /// <summary>The question's page on leetcode.com.</summary>
    [JsonIgnore]
    public Uri Url => new($"{ProblemsUrlPrefix}{TitleSlug}/");
}
