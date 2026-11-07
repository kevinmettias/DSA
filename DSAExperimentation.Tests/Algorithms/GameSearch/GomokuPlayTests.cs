using DSAExperimentation.Algorithms.GameSearch;
using DSAExperimentation.Algorithms.GameSearch.PositionalGames;
using DSAExperimentation.Domain.Games.PositionalGames;
using DSAExperimentation.Domain.Games.PositionalGames.InARow;
using DSAExperimentation.Tests.Algorithms.GameSearch.Fixtures;
using DSAExperimentation.Tests.Algorithms.GameSearch.PositionalGames.Fixtures;
using Gomoku = DSAExperimentation.Domain.Games.PositionalGames.InARow.InARowGame<
    DSAExperimentation.Domain.Games.PositionalGames.InARow.GomokuBoard,
    DSAExperimentation.Domain.Games.PositionalGames.InARow.KOrMore>;

namespace DSAExperimentation.Tests.Algorithms.GameSearch;

// Gomoku's strong play, alpha-beta with the shifted open-lines evaluation and the threats tried
// first, which these tests drive together on positions drawn by hand (GomokuPositions), X to move:
// it takes a win at hand and blocks the opponent's, and sees a double four's win three plies deep.
// Against victory by continuous fours, what threat search buys: alpha-beta needs tens of thousands
// of positions where the search needs a handful, and a win thirteen plies deep is out of its sight.
public sealed partial class GomokuPlayTests
{
    private const int DoubleFourDepth = 3;
    private const int FarFewer = 1000;

    public static TheoryData<int> Depths => new() { 1, 2, 3 };

    public static TheoryData<int> DepthsFromTwo => new() { 2, 3 };

    [Theory]
    [MemberData(nameof(Depths))]
    public void BestMove_ABrokenFour_TakesTheGap(int depth) =>
        Assert.Equal(GomokuDiagram.Cell(7, 5), BestCell(GomokuPositions.BrokenFour, depth));

    [Theory]
    [MemberData(nameof(DepthsFromTwo))]
    public void BestMove_TheOpponentsFour_IsBlocked(int depth) =>
        Assert.Equal(GomokuDiagram.Cell(11, 2), BestCell(GomokuPositions.OpponentsFour, depth));

    [Fact]
    public void Value_ADoubleFourAtItsDepth_IsAWin() =>
        Assert.True(Searched(GomokuPositions.DoubleFour, DoubleFourDepth).Value > Gomoku.WinValue / 2);

    [Fact]
    public void Value_SixFoursDeepAtTheDoubleFoursDepth_SeesNoWin() =>
        Assert.True(Searched(GomokuPositions.SixFoursDeep, DoubleFourDepth).Value < Gomoku.WinValue / 2);

    [Fact]
    public void Positions_ADoubleFour_FarFewerForContinuousFoursThanForAlphaBeta() =>
        Assert.True(
            PositionsByContinuousFours(GomokuPositions.DoubleFour) * FarFewer
            < Searched(GomokuPositions.DoubleFour, DoubleFourDepth).Positions);

    private static Gomoku Game() => new(MnkGame<GomokuBoard>.Board);

    private static int BestCell(string[] rows, int depth)
    {
        var game = Game();
        var position = GomokuDiagram.Position(rows);
        var (move, _) = AlphaBeta.BestMove<
            Gomoku, ShiftedOpenLinesEvaluation<GomokuBoard, KOrMore>,
            ThreatsFirstCells<GomokuBoard, KOrMore>, Bitboard256Position, int>(
            game, new(game), new(game), position, depth);

        return game.ChoiceOf(position, move);
    }

    // Alpha-beta to a depth, the threats tried first, and how many positions it reached.
    private static (long Positions, int Value) Searched(string[] rows, int depth)
    {
        var game = Game();
        var moves = new long[1];
        var value = AlphaBeta.Value<
            CountingGame<Gomoku, Bitboard256Position, int>,
            ShiftedOpenLinesEvaluation<GomokuBoard, KOrMore>,
            ThreatsFirstCells<GomokuBoard, KOrMore>, Bitboard256Position, int>(
            new(game, moves), new(game), new(game), GomokuDiagram.Position(rows), depth);

        return (moves[0], value);
    }

    private static long PositionsByContinuousFours(string[] rows)
    {
        ContinuousFours.TryFindWin(Game(), GomokuDiagram.Position(rows), out _, out var positions);

        return positions;
    }
}
