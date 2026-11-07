using DSAExperimentation.Benchmarks.StrategySwaps;

namespace DSAExperimentation.Benchmarks.Tests.StrategySwaps;

// Harness coverage for GomokuSearchBenchmarks (ARCHITECTURE 17.9). The generic check compares the
// arms on the first position only, so both positions are compared here, and each is a forced win
// for the player to move, which pins the baseline's answer independently of either arm.
public sealed partial class GomokuSearchBenchmarksTests
{
    public static TheoryData<int> Positions => new() { 0, 1 };

    [Theory]
    [MemberData(nameof(Positions))]
    public void Arms_EveryPosition_FindTheForcedWin(int position)
    {
        var harness = new GomokuSearchBenchmarks { Position = position };
        harness.Setup();

        Assert.Equal(
            (true, true),
            (harness.IsWonByAlphaBeta(), harness.IsWonByContinuousFours()));
    }
}
