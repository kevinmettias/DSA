using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountPairsWithXorInARangeBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - rescanning every earlier value against querying this
// repo's BitTrie for the range - so a harness whose arms disagree is timing two different problems.
// Setup seeds the values, so the same Length must rebuild the same array.
public sealed partial class CountPairsWithXorInARangeBenchmarksTests
{
    private const int SmallestLength = 200;

    // Every counted pair is one index pair, so the answer is bounded by how many the array holds.
    private const int MostIndexPairs = SmallestLength * (SmallestLength - 1) / 2;

    [Fact]
    public void Setup_SmallestLength_RebuildsTheSameWorkload()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        // The documented shape: values are spread over [0, 20_000) while the queried xor range is
        // [100, 5_000], so real pairs land inside it - and none of them can be counted twice.
        Assert.InRange(first.PairwiseScan(), 1, MostIndexPairs);
        Assert.Equal(first.PairwiseScan(), second.PairwiseScan());
    }

    [Fact]
    public void PairwiseScan_SmallestLength_AgreesWithBitTrieRangeCount()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BitTrieRangeCount(), harness.PairwiseScan());
    }

    [Fact]
    public void BitTrieRangeCount_SmallestLength_AgreesWithPairwiseScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.PairwiseScan(), harness.BitTrieRangeCount());
    }

    private static CountPairsWithXorInARangeBenchmarks BuildHarness()
    {
        var harness = new CountPairsWithXorInARangeBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
