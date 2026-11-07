using DSAExperimentation.Algorithms.GameSearch.PositionalGames;
using DSAExperimentation.Domain.Games.PositionalGames;
using DSAExperimentation.Domain.Games.PositionalGames.InARow;
using DSAExperimentation.Tests.Algorithms.GameSearch.PositionalGames.Fixtures;
using DSAExperimentation.Tests.Domain.Games.PositionalGames.InARow.Fixtures;
using Gomoku = DSAExperimentation.Domain.Games.PositionalGames.InARow.InARowGame<
    DSAExperimentation.Domain.Games.PositionalGames.InARow.GomokuBoard,
    DSAExperimentation.Domain.Games.PositionalGames.InARow.KOrMore>;

namespace DSAExperimentation.Tests.Algorithms.GameSearch.PositionalGames;

// The order on Gomoku's board, X to move, rows numbered from the top: every move once, and the
// ranks in their order: a win, then a block of the opponent's win, then a four, and the centre
// first on an empty board.
public sealed partial class ThreatsFirstCellsTests
{
    private const int RandomPositions = 60;
    private const int Seed = 1994;
    private const int MostPercent = 60;

    [Fact]
    public void Order_RandomPositions_WritesEveryMoveOnce() =>
        Assert.All(DrawnPositions(), position =>
        {
            var moves = Ordered(position);

            Assert.Equal(Enumerable.Range(0, moves.Length), moves.Order());
        });

    [Fact]
    public void Order_EmptyBoard_StartsAtTheCentre() =>
        Assert.Equal(GomokuDiagram.Cell(7, 7), CellsInOrder(default)[0]);

    [Fact]
    public void Order_AWinAtHand_ComesFirst() =>
        Assert.Equal(
            [GomokuDiagram.Cell(7, 5), GomokuDiagram.Cell(7, 10)],
            CellsInOrder(GomokuDiagram.Position(GomokuPositions.BothFours))[..2]);

    [Fact]
    public void Order_TheOpponentsWinAtHand_IsBlockedFirst() =>
        Assert.Equal(
            GomokuDiagram.Cell(11, 2),
            CellsInOrder(GomokuDiagram.Position(GomokuPositions.OpponentsFour))[0]);

    // After the block, the cells that make the mover a four: the two ends of the open three and
    // the two beyond them, which make a broken four.
    [Fact]
    public void Order_AfterTheBlock_TheFoursCome() =>
        Assert.Equal(
            [GomokuDiagram.Cell(7, 4), GomokuDiagram.Cell(7, 5), GomokuDiagram.Cell(7, 9),
                GomokuDiagram.Cell(7, 10)],
            CellsInOrder(GomokuDiagram.Position(GomokuPositions.OpponentsFour))[1..5]);

    private static Gomoku Game() => new(MnkGame<GomokuBoard>.Board);

    private static int[] Ordered(Bitboard256Position position)
    {
        var moves = new int[Game().MoveCount(position)];
        new ThreatsFirstCells<GomokuBoard, KOrMore>(Game()).Order(position, moves);

        return moves;
    }

    // The cells in the order their moves are tried.
    private static int[] CellsInOrder(Bitboard256Position position) =>
        [.. Ordered(position).Select(move => Game().ChoiceOf(position, move))];

    private static IEnumerable<Bitboard256Position> DrawnPositions()
    {
        var random = new Random(Seed);
        var board = MnkGame<GomokuBoard>.Board;

        for (var drawn = 0; drawn < RandomPositions; drawn++)
        {
            var percent = random.Next(MostPercent);
            var (mine, theirs) = LineOracle<GomokuBoard, KOrMore>.RandomCells(board, random, percent);

            yield return new(Mask(mine), Mask(theirs), IsLost: false);
        }
    }

    private static Bitboard256 Mask(bool[] cells) =>
        InARowLines<GomokuBoard, KOrMore>.MaskOf(LineOracle<GomokuBoard, KOrMore>.CellsOf(cells));
}
