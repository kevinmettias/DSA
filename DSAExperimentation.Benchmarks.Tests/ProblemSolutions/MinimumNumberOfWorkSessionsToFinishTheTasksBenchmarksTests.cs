using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumNumberOfWorkSessionsToFinishTheTasksBenchmarks (ARCHITECTURE 17.9): its
// two arms are competing strategies for the same question - the fewest sessions of length sessionTime
// that fit every task - so a harness whose arms disagree is timing two different problems. Both arms
// take the FeasibleSessionMasks [GlobalSetup] already built, so the comparison also pins that the
// hoisted sum-over-subsets table is the one both strategies consulted. Setup derives that table from
// the task durations alone, so the same TaskCount must rebuild the same session set.
public sealed partial class MinimumNumberOfWorkSessionsToFinishTheTasksBenchmarksTests
{
    private const int SmallestTaskCount = 6;

    [Fact]
    public void Setup_SameTaskCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().MemoizedBitmaskDp(), BuildHarness().MemoizedBitmaskDp());

    [Fact]
    public void MemoizedBitmaskDp_SameFeasibleSessions_AgreesWithUnmemoizedRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.UnmemoizedRecursion(), harness.MemoizedBitmaskDp());
    }

    [Fact]
    public void UnmemoizedRecursion_SameFeasibleSessions_AgreesWithMemoizedBitmaskDp()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedBitmaskDp(), harness.UnmemoizedRecursion());
    }

    private static MinimumNumberOfWorkSessionsToFinishTheTasksBenchmarks BuildHarness()
    {
        var harness = new MinimumNumberOfWorkSessionsToFinishTheTasksBenchmarks { TaskCount = SmallestTaskCount };
        harness.Setup();

        return harness;
    }
}
