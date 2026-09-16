using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for GraphConnectivityWithThresholdBenchmarks (ARCHITECTURE 17.9): both arms are
// GraphConnectivityWithThresholdSolution's - the divisor-sieve sweep over a naive uncompressed
// union-find against this repo's own DisjointSet - so a harness whose arms disagree is timing two
// different problems. The threshold is CityCount / 20, low enough that the sieve chains most cities
// together through small divisors, which is the shape the naive arm's uncompressed chains degrade
// on. Both arms answer with one bool per query in query order, which is LeetCode's own output
// shape, so AnswerText.Of compares them position by position. Setup derives the threshold from
// CityCount and draws the query stream off one seed, so the same CityCount must rebuild the same
// threshold, the same queries and the same answers.
public sealed partial class GraphConnectivityWithThresholdBenchmarksTests
{
    private const int SmallestCityCount = 500;

    [Fact]
    public void Setup_SameCityCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().NaiveUnionFind()),
            AnswerText.Of(BuildHarness().NaiveUnionFind()));

    [Fact]
    public void NaiveUnionFind_SeededQueryStream_AgreesWithDisjointSetUnionFind()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.DisjointSetUnionFind()), AnswerText.Of(harness.NaiveUnionFind()));
    }

    [Fact]
    public void DisjointSetUnionFind_SeededQueryStream_AgreesWithNaiveUnionFind()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.NaiveUnionFind()), AnswerText.Of(harness.DisjointSetUnionFind()));
    }

    private static GraphConnectivityWithThresholdBenchmarks BuildHarness()
    {
        var harness = new GraphConnectivityWithThresholdBenchmarks { CityCount = SmallestCityCount };
        harness.Setup();

        return harness;
    }
}
