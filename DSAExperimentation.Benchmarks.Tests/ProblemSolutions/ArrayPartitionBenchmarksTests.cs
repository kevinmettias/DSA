using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ArrayPartitionBenchmarks (ARCHITECTURE 17.9): its two arms are ArrayPartitionSolution's
// competing strategies for the same question - repeatedly scanning for the two smallest remaining values against
// sorting once and summing every even index - so a harness whose arms disagree has maximized two different sums.
// LC 561's answer is a single int, so the arms are compared directly rather than through a rendering; Setup draws
// the pair values from one seed over a range centred on zero, so the same Length must rebuild the same values.
public sealed partial class ArrayPartitionBenchmarksTests
{
    // The smaller of Setup's [Params(200, 5_000)] lengths.
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().RepeatedSmallestPairScan(), BuildHarness().RepeatedSmallestPairScan());

    [Fact]
    public void RepeatedSmallestPairScan_TwoHundredValueArray_AgreesWithMergeSortPairSum()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MergeSortPairSum(), harness.RepeatedSmallestPairScan());
    }

    [Fact]
    public void MergeSortPairSum_TwoHundredValueArray_AgreesWithRepeatedSmallestPairScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RepeatedSmallestPairScan(), harness.MergeSortPairSum());
    }

    private static ArrayPartitionBenchmarks BuildHarness()
    {
        var harness = new ArrayPartitionBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
