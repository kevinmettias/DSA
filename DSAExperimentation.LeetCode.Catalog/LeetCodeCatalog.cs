using DSAExperimentation.LeetCode.Catalog.Questions;
using DSAExperimentation.LeetCode.Catalog.Storage;

namespace DSAExperimentation.LeetCode.Catalog;

// The read side of the catalog: what LeetCode says about every covered problem, and every case a
// harness runs for one. It reads the checked-in snapshot under Snapshot/ and never the network, so a
// test or benchmark built on it is as deterministic as one with its cases written inline; the
// snapshot is kept current by DSAExperimentation.LeetCode.Catalog.Sync, run weekly by
// .github/workflows/leetcode-catalog-sync.yml and on demand by hand.
//
// Deliberately plain lists rather than a query language: "every Hard graph problem" or "problems
// by acceptance rate" are a Where or an OrderBy over Problems().
/// <summary>What LeetCode says about every problem the repository covers, and the cases each one is run on.</summary>
public static class LeetCodeCatalog
{
    private static readonly Lazy<StatisticsFile> StatisticsSnapshot = new(StatisticsFile.Read);
    private static readonly Lazy<ContestRatingsFile> ContestRatings = new(ContestRatingsFile.Read);

    /// <summary>The day the statistics were last read from LeetCode.</summary>
    public static DateOnly StatisticsAsOf => StatisticsSnapshot.Value.AsOf;

    /// <summary>The title slugs of every problem in the catalog, in ordinal order.</summary>
    /// <returns>Every enrolled problem's slug.</returns>
    public static IReadOnlyList<string> TitleSlugs() => QuestionFiles.Slugs();

    /// <summary>Loads one problem's question as LeetCode publishes it.</summary>
    /// <param name="titleSlug">The problem's URL slug, such as "two-sum".</param>
    /// <returns>The question.</returns>
    /// <exception cref="KeyNotFoundException">The catalog does not hold the problem.</exception>
    public static LeetCodeQuestion Question(string titleSlug)
    {
        if (!QuestionFiles.TryRead(titleSlug, out var question))
        {
            throw new KeyNotFoundException(
                $"The catalog holds no question '{titleSlug}'. Enrol it with the sync tool's `questions {titleSlug}` command.");
        }

        return question;
    }

    /// <summary>Loads everything the catalog knows about one problem.</summary>
    /// <param name="titleSlug">The problem's URL slug, such as "two-sum".</param>
    /// <returns>The problem's question, statistics and contest rating.</returns>
    /// <exception cref="KeyNotFoundException">The catalog does not hold the problem, or holds no statistics for it.</exception>
    public static LeetCodeProblem Problem(string titleSlug)
    {
        var question = Question(titleSlug);

        if (!StatisticsSnapshot.Value.Questions.TryGetValue(titleSlug, out var statistics))
        {
            throw new KeyNotFoundException($"The catalog holds no statistics for '{titleSlug}'. Run the sync tool.");
        }

        var rating = ContestRatings.Value.Questions.GetValueOrDefault(titleSlug);
        return new LeetCodeProblem(question, statistics, rating);
    }

    /// <summary>Loads every problem in the catalog, ordered by LeetCode's frontend number.</summary>
    /// <returns>Every enrolled problem.</returns>
    public static IReadOnlyList<LeetCodeProblem> Problems()
        => TitleSlugs()
            .Select(Problem)
            .OrderBy(problem => problem.Question.FrontendId)
            .ToList();

    /// <summary>Every case the catalog holds for one problem: its examples, then the dataset's, then LeetCode Run's.</summary>
    /// <param name="titleSlug">The problem's URL slug, such as "two-sum".</param>
    /// <returns>The problem's cases, each tagged with its source.</returns>
    /// <exception cref="KeyNotFoundException">The catalog does not hold the problem.</exception>
    public static IReadOnlyList<LeetCodeCase> Cases(string titleSlug)
    {
        var examples = Question(titleSlug).Examples.Select(example => example with { Source = LeetCodeCaseSource.Example });
        var dataset = CaseFiles.Read(LeetCodeCaseSource.Dataset, titleSlug);
        var run = CaseFiles.Read(LeetCodeCaseSource.LeetCodeRun, titleSlug);

        return examples.Concat(dataset).Concat(run).ToList();
    }
}
