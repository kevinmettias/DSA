using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for KthSmallestAmountWithSingleDenominationCombinationBenchmarks
// (ARCHITECTURE 17.9): its two arms are competing strategies for one question - the Rank-th
// smallest amount expressible as a sum of the fixed coin denominations - so a harness whose arms
// disagree is timing two different problems. The coins are fixed and seeded, Rank is the axis
// that grows, and both arms answer with the bare amount, so the comparison is on the one number
// the problem asks for rather than on the enumeration order each strategy produces it in.
public sealed partial class KthSmallestAmountWithSingleDenominationCombinationBenchmarksTests
{
    private const int SmallestRank = 200;

    [Fact]
    public void Setup_SameRank_RebuildsTheSameCoins() =>
        Assert.Equal(
            BuildHarness().HeapMerge(),
            BuildHarness().HeapMerge());

    [Fact]
    public void HeapMerge_SeededCoins_AgreesWithInclusionExclusionSearch()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.InclusionExclusionSearch(), harness.HeapMerge());
    }

    [Fact]
    public void InclusionExclusionSearch_SeededCoins_AgreesWithHeapMerge()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HeapMerge(), harness.InclusionExclusionSearch());
    }

    private static KthSmallestAmountWithSingleDenominationCombinationBenchmarks BuildHarness()
    {
        var harness = new KthSmallestAmountWithSingleDenominationCombinationBenchmarks { Rank = SmallestRank };
        harness.Setup();

        return harness;
    }
}
