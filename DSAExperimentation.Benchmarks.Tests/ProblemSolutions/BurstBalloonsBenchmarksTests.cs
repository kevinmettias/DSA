using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for BurstBalloonsBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - the unmemoized interval recursion against the same recursion
// memoized on its interval - so a harness whose arms disagree is timing two different problems.
// Both arms read the same padded balloon array, so the comparison also pins that the two strategies
// were handed the same boundary padding, which the benchmark hoisted into Setup rather than paying
// for inside either arm. Setup draws the balloons from one fixed seed, so the same BalloonCount must
// rebuild the same workload.
public sealed partial class BurstBalloonsBenchmarksTests
{
    private const int SmallestBalloonCount = 10;

    [Fact]
    public void Setup_SameBalloonCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().UnmemoizedRecursion(), BuildHarness().UnmemoizedRecursion());

    [Fact]
    public void UnmemoizedRecursion_PaddedBalloonRun_AgreesWithMemoizedRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedRecursion(), harness.UnmemoizedRecursion());
    }

    [Fact]
    public void MemoizedRecursion_PaddedBalloonRun_AgreesWithUnmemoizedRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.UnmemoizedRecursion(), harness.MemoizedRecursion());
    }

    private static BurstBalloonsBenchmarks BuildHarness()
    {
        var harness = new BurstBalloonsBenchmarks { BalloonCount = SmallestBalloonCount };
        harness.Setup();

        return harness;
    }
}
