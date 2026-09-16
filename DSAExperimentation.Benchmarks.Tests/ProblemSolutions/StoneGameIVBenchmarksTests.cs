using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for StoneGameIVBenchmarks (ARCHITECTURE 17.9): both arms answer the same
// question - whether the player to move wins LC 1510 from a given stone count - one by plain
// un-memoized recursion, one by memoizing the remaining count, so a harness whose arms disagree
// is timing two different problems. This class has no [GlobalSetup]: the workload is the
// [Params] stone count itself.
public sealed partial class StoneGameIVBenchmarksTests
{
    private const int SmallestStoneCount = 16;

    // SmallestStoneCount is a perfect square, so the player to move can take the whole pile at
    // once and leave the opponent an empty pile - a decisive win rather than a bare agreement.
    private const bool ExpectedAliceWins = true;

    [Fact]
    public void CanAliceWinByUnmemoizedRecursion_AgreesWithCanAliceWinByMemoizedRecursion()
    {
        var harness = BuildHarness();
        var unmemoized = harness.CanAliceWinByUnmemoizedRecursion();

        Assert.Equal(unmemoized, harness.CanAliceWinByMemoizedRecursion());
        Assert.Equal(ExpectedAliceWins, unmemoized);
    }

    [Fact]
    public void CanAliceWinByMemoizedRecursion_AgreesWithCanAliceWinByUnmemoizedRecursion()
    {
        var harness = BuildHarness();
        var memoized = harness.CanAliceWinByMemoizedRecursion();

        Assert.Equal(memoized, harness.CanAliceWinByUnmemoizedRecursion());
        Assert.Equal(ExpectedAliceWins, memoized);
    }

    private static StoneGameIVBenchmarks BuildHarness() =>
        new() { StoneCount = SmallestStoneCount };
}
