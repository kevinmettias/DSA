using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for KthSmallestInLexicographicalOrderBenchmarks (ARCHITECTURE 17.9): its two
// arms are competing strategies for one question - which number sits at the half-way rank when
// 1..UpperBound is ordered lexicographically as decimal strings - so a harness whose arms
// disagree is timing two different problems. Both arms answer with the bare number, so the
// comparison is on the rank-th element itself and not on the order either strategy walks.
public sealed partial class KthSmallestInLexicographicalOrderBenchmarksTests
{
    private const int SmallestUpperBound = 200_000;

    [Fact]
    public void Setup_SameUpperBound_RebuildsTheSameMedianRank() =>
        Assert.Equal(
            BuildHarness().GenerateAndSortStrings(),
            BuildHarness().GenerateAndSortStrings());

    [Fact]
    public void GenerateAndSortStrings_LexicographicMedian_AgreesWithDepthFirstTraversalOrder()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DepthFirstTraversalOrder(), harness.GenerateAndSortStrings());
    }

    [Fact]
    public void DepthFirstTraversalOrder_LexicographicMedian_AgreesWithGenerateAndSortStrings()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.GenerateAndSortStrings(), harness.DepthFirstTraversalOrder());
    }

    private static KthSmallestInLexicographicalOrderBenchmarks BuildHarness()
    {
        var harness = new KthSmallestInLexicographicalOrderBenchmarks { UpperBound = SmallestUpperBound };
        harness.Setup();

        return harness;
    }
}
