using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumPairRemovalToSortArrayIIBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - how many smallest-sum adjacent merges it takes to make
// the array non-decreasing, leftmost pair on a tie - so a harness whose arms disagree is timing two
// different problems. The lazy-deletion heap arm can only answer as the rescan arm does if its
// tie-break and its stale-candidate bookkeeping both hold, so agreement is the load-bearing check
// here. Both arms clone before merging, so one harness serves both. Setup draws the values from one
// fixed seed, so the same Length must rebuild the same array.
public sealed partial class MinimumPairRemovalToSortArrayIIBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForceScan(), BuildHarness().BruteForceScan());

    [Fact]
    public void BruteForceScan_SameValueRun_AgreesWithLazyHeap()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LazyHeap(), harness.BruteForceScan());
    }

    [Fact]
    public void LazyHeap_SameValueRun_AgreesWithBruteForceScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceScan(), harness.LazyHeap());
    }

    private static MinimumPairRemovalToSortArrayIIBenchmarks BuildHarness()
    {
        var harness = new MinimumPairRemovalToSortArrayIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
