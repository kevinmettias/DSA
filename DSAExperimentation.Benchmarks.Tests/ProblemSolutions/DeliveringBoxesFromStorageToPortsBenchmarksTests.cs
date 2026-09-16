using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DeliveringBoxesFromStorageToPortsBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - rescanning the valid window at every position against this
// repo's Deque<int> carrying the window's minimum - so a harness whose arms disagree is timing two
// different problems. [GlobalSetup] builds the boxes and their prefix scans from one fixed seed into the
// schedule both arms are handed, so only the sweep is measured; every trip carries at most MaxBoxes, so
// the reading's documented floor is one trip per full load, and the same Length must rebuild the same
// schedule and with it the same trip count.
public sealed partial class DeliveringBoxesFromStorageToPortsBenchmarksTests
{
    private const int SmallestLength = 2_000;

    // [GlobalSetup] fixes the per-trip box limit the floor is derived from.
    private const int MaxBoxes = 50;

    private const int MinimumTripCount = ((SmallestLength - 1) / MaxBoxes) + 1;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload()
    {
        Assert.InRange(BuildHarness().RescanWindowEachPosition(), MinimumTripCount, SmallestLength);
        Assert.Equal(BuildHarness().RescanWindowEachPosition(), BuildHarness().RescanWindowEachPosition());
    }

    [Fact]
    public void RescanWindowEachPosition_TwoThousandSeededBoxes_AgreesWithMonotonicDequeDp()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MonotonicDequeDp(), harness.RescanWindowEachPosition());
    }

    [Fact]
    public void MonotonicDequeDp_TwoThousandSeededBoxes_AgreesWithRescanWindowEachPosition()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RescanWindowEachPosition(), harness.MonotonicDequeDp());
    }

    private static DeliveringBoxesFromStorageToPortsBenchmarks BuildHarness()
    {
        var harness = new DeliveringBoxesFromStorageToPortsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
