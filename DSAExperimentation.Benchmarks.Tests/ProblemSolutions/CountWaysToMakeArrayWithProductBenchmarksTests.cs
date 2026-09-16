using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountWaysToMakeArrayWithProductBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - trial division re-paid on every query against one
// smallest-prime-factor sieve shared across them all - so a harness whose arms disagree is timing two
// different problems, not two ways of answering one. Setup draws the queries from one fixed seed, so
// the same QueryCount must rebuild the same queries; otherwise two published numbers were never
// comparable in the first place.
//
// The query array is private, but its length is exactly what the answer reports: both arms answer one
// way count per query, in query order, so the documented QueryCount pins the result's own length.
// AnswerText.Of rather than OfUnorderedSet - a value's position is the query it answers, and a set
// rendering would score an answer against the wrong query.
public sealed partial class CountWaysToMakeArrayWithProductBenchmarksTests
{
    private const int SmallestQueryCount = 200;

    [Fact]
    public void Setup_SameQueryCount_RebuildsTheSameQueries()
    {
        Assert.Equal(SmallestQueryCount, BuildHarness().TrialDivisionPerQuery().Length);
        Assert.Equal(
            AnswerText.Of(BuildHarness().TrialDivisionPerQuery()),
            AnswerText.Of(BuildHarness().TrialDivisionPerQuery()));
    }

    [Fact]
    public void TrialDivisionPerQuery_SeededQueries_AgreesWithSmallestPrimeFactorSieve()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.SmallestPrimeFactorSieve()),
            AnswerText.Of(harness.TrialDivisionPerQuery()));
    }

    [Fact]
    public void SmallestPrimeFactorSieve_SeededQueries_AgreesWithTrialDivisionPerQuery()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.TrialDivisionPerQuery()),
            AnswerText.Of(harness.SmallestPrimeFactorSieve()));
    }

    private static CountWaysToMakeArrayWithProductBenchmarks BuildHarness()
    {
        var harness = new CountWaysToMakeArrayWithProductBenchmarks { QueryCount = SmallestQueryCount };
        harness.Setup();

        return harness;
    }
}
