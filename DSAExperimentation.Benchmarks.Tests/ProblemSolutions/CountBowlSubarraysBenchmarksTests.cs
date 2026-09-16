using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountBowlSubarraysBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - reading the bowl definition off every (l, r) pair against
// counting the pinning next/previous strictly-greater boundaries a monotonic sweep produces - so a
// harness whose arms disagree is timing two different problems. Both arms return an int, so they are
// compared directly. Setup builds the array from one fixed seed, so the same Size must rebuild the
// same workload: the shared fixture shuffles a contiguous range, which is what keeps every value
// distinct the way LC 3676 specifies.
public sealed partial class CountBowlSubarraysBenchmarksTests
{
    private const int SmallestSize = 200;

    [Fact]
    public void Setup_SameSize_RebuildsTheSameDistinctValuedWorkload() =>
        Assert.Equal(BuildHarness().PairScan(), BuildHarness().PairScan());

    [Fact]
    public void PairScan_ShuffledRange_AgreesWithMonotonicStack()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MonotonicStack(), harness.PairScan());
    }

    [Fact]
    public void MonotonicStack_ShuffledRange_AgreesWithPairScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.PairScan(), harness.MonotonicStack());
    }

    private static CountBowlSubarraysBenchmarks BuildHarness()
    {
        var harness = new CountBowlSubarraysBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
