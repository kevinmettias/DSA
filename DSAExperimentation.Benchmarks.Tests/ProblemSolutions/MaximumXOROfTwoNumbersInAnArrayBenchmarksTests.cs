using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumXOROfTwoNumbersInAnArrayBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the textbook O(n^2) pairwise scan against this repo's
// own BitTrie greedy walk - so a harness whose arms disagree is timing two different problems. Both
// arms return the maximum xor as an int, so they are compared directly, and both only read the value
// array, so one harness is safe to read twice in either order. Setup draws the values from one fixed
// seed, so the same Length must rebuild the same array and with it the same maximum xor.
public sealed partial class MaximumXOROfTwoNumbersInAnArrayBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().PairwiseScan(), BuildHarness().PairwiseScan());

    [Fact]
    public void PairwiseScan_SeededValues_AgreesWithBitTrieGreedy()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BitTrieGreedy(), harness.PairwiseScan());
    }

    [Fact]
    public void BitTrieGreedy_SeededValues_AgreesWithPairwiseScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.PairwiseScan(), harness.BitTrieGreedy());
    }

    private static MaximumXOROfTwoNumbersInAnArrayBenchmarks BuildHarness()
    {
        var harness = new MaximumXOROfTwoNumbersInAnArrayBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
