using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumPairRemovalToSortArrayIBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - how many smallest-sum adjacent merges it takes to make
// the array non-decreasing, leftmost pair on a tie - so a harness whose arms disagree is timing two
// different problems. The tie-break is where these two strategies can legitimately part company, so
// agreement pins that the heap arm's original-index tie-break resolves the same way the rescan arm's
// leftmost-first search does. Both arms clone before merging, so one harness serves both. Setup draws
// the values from one fixed seed, so the same Length must rebuild the same array.
public sealed partial class MinimumPairRemovalToSortArrayIBenchmarksTests
{
    private const int SmallestLength = 10;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());

    [Fact]
    public void BruteForce_SameValueRun_AgreesWithLazyPairHeap()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LazyPairHeap(), harness.BruteForce());
    }

    [Fact]
    public void LazyPairHeap_SameValueRun_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.LazyPairHeap());
    }

    private static MinimumPairRemovalToSortArrayIBenchmarks BuildHarness()
    {
        var harness = new MinimumPairRemovalToSortArrayIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
