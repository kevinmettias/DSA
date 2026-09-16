using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CamelcaseMatchingBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - the regex engine's backtracking state machine against a direct
// two-pointer scan - so a harness whose arms disagree is timing two different problems, and the
// disagreement shows up per query, not as a summary. AnswerText.Of, not OfUnorderedSet: the answers
// come back one per query in query order, and a set rendering would score a result against the wrong
// query. Setup builds the queries and compiles the matcher, so the same QueryCount must rebuild the
// same both; the fixture makes every query a genuine match, so neither arm short-circuits.
public sealed partial class CamelcaseMatchingBenchmarksTests
{
    private const int SmallestQueryCount = 500;

    [Fact]
    public void Setup_SameQueryCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().RegexPerQuery()),
            AnswerText.Of(BuildHarness().RegexPerQuery()));

    [Fact]
    public void RegexPerQuery_FiveHundredMatchingQueries_AgreesWithTwoPointerPerQuery()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.TwoPointerPerQuery()), AnswerText.Of(harness.RegexPerQuery()));
    }

    [Fact]
    public void TwoPointerPerQuery_FiveHundredMatchingQueries_AgreesWithRegexPerQuery()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.RegexPerQuery()), AnswerText.Of(harness.TwoPointerPerQuery()));
    }

    private static CamelcaseMatchingBenchmarks BuildHarness()
    {
        var harness = new CamelcaseMatchingBenchmarks { QueryCount = SmallestQueryCount };
        harness.Setup();

        return harness;
    }
}
