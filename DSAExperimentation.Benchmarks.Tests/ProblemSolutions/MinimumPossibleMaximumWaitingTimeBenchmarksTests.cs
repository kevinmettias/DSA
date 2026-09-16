using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumPossibleMaximumWaitingTimeBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - the smallest maximum waiting time achievable when
// every car takes fuel from one of the two dispensers - so a harness whose arms disagree is timing
// two different problems. The fuel is fixed at the problem's own maximum, so both arms keep a real
// branching factor instead of collapsing into forced single choices; agreement is what proves the
// memoized and unmemoized searches explore the same schedule tree. Setup draws the demands from one
// fixed seed, so the same Length must rebuild the same demand run.
public sealed partial class MinimumPossibleMaximumWaitingTimeBenchmarksTests
{
    private const int SmallestLength = 10;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().RecursiveSearch(), BuildHarness().RecursiveSearch());

    [Fact]
    public void MemoizedSearch_SameDemandRun_AgreesWithRecursiveSearch()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RecursiveSearch(), harness.MemoizedSearch());
    }

    [Fact]
    public void RecursiveSearch_SameDemandRun_AgreesWithMemoizedSearch()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedSearch(), harness.RecursiveSearch());
    }

    private static MinimumPossibleMaximumWaitingTimeBenchmarks BuildHarness()
    {
        var harness = new MinimumPossibleMaximumWaitingTimeBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
