using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for StoneGameIVBenchmarks (ARCHITECTURE 17.9): whether the player to move wins
// LC 1510 from the smallest shared stone count is known without running either arm, so the
// baseline is asserted against it - the generic arm check only holds the table arm to the
// baseline's answer, which two arms sharing one wrong verdict would pass.
public sealed partial class StoneGameIVBenchmarksTests
{
    private const int SmallestStoneCount = 16;

    // SmallestStoneCount is a perfect square, so the player to move can take the whole pile at
    // once and leave the opponent an empty pile - a decisive win rather than a bare agreement.
    private const bool ExpectedAliceWins = true;

    [Fact]
    public void CanAliceWinByUnmemoizedRecursion_PerfectSquareCount_AliceWins() =>
        Assert.Equal(ExpectedAliceWins, BuildHarness().CanAliceWinByUnmemoizedRecursion(SmallestStoneCount));

    private static StoneGameIVBenchmarks BuildHarness() => new();
}
