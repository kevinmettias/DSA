using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ContainsDuplicateIIIBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the O(n * indexDiff) backward window scan against
// the bucketed hashmap - so a harness whose arms disagree is timing two different problems. Both
// arms return a bool, so they are compared directly. Setup draws from one fixed seed, so the same
// Length must rebuild the same array, and its documented shape is that no pair inside the index
// window is within valueDiff: that is what forces both strategies through their full worst-case
// scan instead of an early exit letting the baseline look artificially competitive. That shape is
// load-bearing: the generator's earlier fixed pool of multiples repeated values inside the window,
// which valueDiff = 3 admits, so both arms returned on an early exit and timed the wrong path.
public sealed partial class ContainsDuplicateIIIBenchmarksTests
{
    private const int SmallestLength = 500;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameUnqualifyingWorkload()
    {
        Assert.Equal(
            BuildHarness().HasNearbyAlmostDuplicateBySlidingWindowBruteForce(),
            BuildHarness().HasNearbyAlmostDuplicateBySlidingWindowBruteForce());

        Assert.False(BuildHarness().HasNearbyAlmostDuplicateByBucketedHashMap());
    }

    [Fact]
    public void HasNearbyAlmostDuplicateBySlidingWindowBruteForce_SpacedValues_AgreesWithBucketedHashMap()
    {
        var harness = BuildHarness();

        Assert.Equal(
            harness.HasNearbyAlmostDuplicateByBucketedHashMap(),
            harness.HasNearbyAlmostDuplicateBySlidingWindowBruteForce());
    }

    [Fact]
    public void HasNearbyAlmostDuplicateByBucketedHashMap_SpacedValues_AgreesWithSlidingWindowBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(
            harness.HasNearbyAlmostDuplicateBySlidingWindowBruteForce(),
            harness.HasNearbyAlmostDuplicateByBucketedHashMap());
    }

    private static ContainsDuplicateIIIBenchmarks BuildHarness()
    {
        var harness = new ContainsDuplicateIIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
