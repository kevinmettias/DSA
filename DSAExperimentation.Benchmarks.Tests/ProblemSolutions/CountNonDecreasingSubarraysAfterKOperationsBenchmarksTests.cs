using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountNonDecreasingSubarraysAfterKOperationsBenchmarks (ARCHITECTURE 17.9):
// its two arms are competing strategies for the same question - a backward rescan from every right
// endpoint against the monotonic sliding window - so a harness whose arms disagree is timing two
// different problems. Setup draws nums from a fixed seed through NonDecreasingSubarrayWorkloads, so
// the same Size must rebuild the same array.
public sealed partial class CountNonDecreasingSubarraysAfterKOperationsBenchmarksTests
{
    private const int SmallestSize = 200;

    // Mirrors the benchmark's own seed, so the workload asserted here is the one Setup builds.
    private const int Seed = 3420;

    // The fixture's own value bound: values are drawn from [1, ValueUpperBound).
    private const int MaxValue = 999;

    [Fact]
    public void Setup_SmallestSize_RebuildsTheSameWorkload()
    {
        var nums = NonDecreasingSubarrayWorkloads.BuildNums(SmallestSize, Seed);

        // The documented shape: a Size-long array of random values from a range tight enough that
        // subarrays land on both sides of the budget, instead of every one of them blowing it.
        Assert.Equal(SmallestSize, nums.Length);
        Assert.All(nums, value => Assert.InRange(value, 1, MaxValue));
        Assert.Equal(BuildHarness().PrefixMaxBruteForce(), BuildHarness().PrefixMaxBruteForce());
    }

    [Fact]
    public void PrefixMaxBruteForce_SmallestSize_AgreesWithMonotonicDequeWindow()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MonotonicDequeWindow(), harness.PrefixMaxBruteForce());
    }

    [Fact]
    public void MonotonicDequeWindow_SmallestSize_AgreesWithPrefixMaxBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.PrefixMaxBruteForce(), harness.MonotonicDequeWindow());
    }

    private static CountNonDecreasingSubarraysAfterKOperationsBenchmarks BuildHarness()
    {
        var harness = new CountNonDecreasingSubarraysAfterKOperationsBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
