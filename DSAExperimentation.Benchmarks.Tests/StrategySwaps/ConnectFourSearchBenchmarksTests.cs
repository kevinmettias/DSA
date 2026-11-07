using DSAExperimentation.Benchmarks.StrategySwaps;
using DSAExperimentation.Domain.Games.PositionalGames;

namespace DSAExperimentation.Benchmarks.Tests.StrategySwaps;

// Harness coverage for ConnectFourSearchBenchmarks (ARCHITECTURE 17.9). The generic check compares
// the arms on the smallest parameters only, one opening at one depth, so every opening's arms are
// compared here at both depths. The last opening is decided within the depth, which pins the
// baseline's answer independently of every arm: the first player's open three on the bottom row
// wins with the ninth piece, so the second player, to move, holds a loss worth nine cells less
// than a win.
public sealed partial class ConnectFourSearchBenchmarksTests
{
    private const int SmallestDepth = 4;
    private const int LostOpening = 3;
    private const int CellsAtTheWin = 9;

    public static TheoryData<int, int> Workloads => new()
    {
        { 4, 0 }, { 4, 1 }, { 4, 2 }, { 4, 3 },
        { 6, 0 }, { 6, 1 }, { 6, 2 }, { 6, 3 },
    };

    [Theory]
    [MemberData(nameof(Workloads))]
    public void Arms_EveryOpeningAndDepth_AnswerAlike(int depth, int opening)
    {
        var harness = BuildHarness(depth, opening);
        var baseline = harness.PlainNegamax();

        Assert.Equal(
            (baseline, baseline, baseline),
            (harness.AlphaBetaLeftToRight(),
                harness.AlphaBetaCentreFirst(),
                harness.DeepeningWithTable()));
    }

    [Fact]
    public void PlainNegamax_TheLostOpening_IsTheLoss() =>
        Assert.Equal(
            CellsAtTheWin - BitboardGame<FreePlacement>.WinValue,
            BuildHarness(SmallestDepth, LostOpening).PlainNegamax());

    private static ConnectFourSearchBenchmarks BuildHarness(int depth, int opening)
    {
        var harness = new ConnectFourSearchBenchmarks { Depth = depth, Opening = opening };
        harness.Setup();

        return harness;
    }
}
