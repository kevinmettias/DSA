using DSAExperimentation.LeetCode.Harness;
using DSAExperimentation.Tests.LeetCodeCatalog;

namespace DSAExperimentation.Tests.LeetCodeCoverage;

// The join between the two halves that until now described the same problems
// without ever consulting each other: a registration states its slug and its cases
// by hand, while the catalog holds what leetcode.com itself published for that
// slug. Nothing checked that they agreed, so a registration could name a slug that
// is not a real problem, or quietly assert fewer cases than LeetCode's own
// worked examples, and every run would stay green.
//
// This lives in the Tests project rather than beside the harness because the
// catalog fixtures are test data - the harness in DSAExperimentation.LeetCode has
// no business depending on a snapshot of a third party's website.
public sealed class LeetCodeRegistrationCatalogTests
{
    // A slug typo is the quiet failure this catches. The harness only ever uses a
    // slug as a dictionary key, so "two-sums" would register, run, and pass -
    // while describing a problem that does not exist. Resolving it against the
    // catalog is what makes the slug an assertion rather than a label.
    [Fact]
    public void EveryRegisteredProblem_NamesASlugTheCatalogKnows()
    {
        var unknown = LeetCodeProblemRegistry.All
            .Select(problem => problem.TitleSlug)
            .Where(slug => !LeetCodeQuestionCache.TryLoad(slug, out _))
            .ToList();

        Assert.Empty(unknown);
    }

    // A floor, not an equality: a registration is expected to cover LeetCode's own
    // worked examples AND to add the edge cases LeetCode never published (empty
    // input, the boundary the constraints allow, the case that separates two
    // strategies). Equality would forbid exactly the cases worth adding. What it
    // rules out is the opposite - a registration that skipped the published
    // examples and asserted only whatever its strategies happened to get right.
    [Fact]
    public void EveryRegisteredProblem_RegistersAtLeastLeetCodesPublishedExamples()
    {
        var shortfalls = LeetCodeProblemRegistry.All
            .Select(problem => (problem.TitleSlug, Registered: problem.CaseNames.Count, Published: PublishedExampleCount(problem.TitleSlug)))
            .Where(counts => counts.Registered < counts.Published)
            .Select(counts => $"{counts.TitleSlug}: {counts.Registered} registered < {counts.Published} published")
            .ToList();

        Assert.Empty(shortfalls);
    }

    // Guards the two assertions above against passing vacuously. Both are
    // "no offenders" assertions over the registry, and an empty registry - or a
    // Fixtures folder that failed to resolve - satisfies both while checking
    // nothing at all.
    [Fact]
    public void TheJoin_OverTheCurrentRegistryAndCatalog_ComparesRealProblems()
    {
        Assert.NotEmpty(LeetCodeProblemRegistry.All);
        Assert.NotEmpty(LeetCodeQuestionCache.ListCachedTitleSlugs());
    }

    private static int PublishedExampleCount(string titleSlug)
        => LeetCodeQuestionCache.TryLoad(titleSlug, out var question) ? question.ExampleTestCases.Count : 0;
}
