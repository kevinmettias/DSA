using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountOfRangeSumBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - a pairwise scan over the prefix sums against the
// coordinate-compressed Fenwick sweep - so a harness whose arms disagree is timing two different
// problems. Setup sizes and seeds nums, so the same Length must rebuild the same array.
public sealed partial class CountOfRangeSumBenchmarksTests
{
    private const int SmallestLength = 200;

    // Every subarray is one index pair, so the answer is bounded by how many pairs the array holds.
    private const int MostIndexPairs = SmallestLength * (SmallestLength + 1) / 2;

    [Fact]
    public void Setup_SmallestLength_RebuildsTheSameWorkload()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        // The documented shape: values come from [-100, 100) while the queried range is [-1000,
        // 1000], so every one-element subarray lands inside it and the answer is at least the
        // array's own length - and never more than its number of subarrays.
        Assert.InRange(first.PairwisePrefixScan(), SmallestLength, MostIndexPairs);
        Assert.Equal(first.PairwisePrefixScan(), second.PairwisePrefixScan());
    }

    [Fact]
    public void PairwisePrefixScan_SeededNums_AgreesWithFenwickTreeSweep()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.FenwickTreeSweep(), harness.PairwisePrefixScan());
    }

    [Fact]
    public void FenwickTreeSweep_SeededNums_AgreesWithPairwisePrefixScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.PairwisePrefixScan(), harness.FenwickTreeSweep());
    }

    private static CountOfRangeSumBenchmarks BuildHarness()
    {
        var harness = new CountOfRangeSumBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
