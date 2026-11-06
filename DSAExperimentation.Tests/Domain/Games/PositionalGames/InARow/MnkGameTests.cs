using DSAExperimentation.Algorithms.GameSearch;
using DSAExperimentation.Domain.Games.PositionalGames.InARow;
using DSAExperimentation.Domain.Games.TicTacToe;
using DSAExperimentation.Domain.Mathematics.Lattices;
using DSAExperimentation.Tests.Domain.Games.PositionalGames.InARow.Fixtures;

namespace DSAExperimentation.Tests.Domain.Games.PositionalGames.InARow;

// The lines are checked three ways: tic-tac-toe's eight literally; every preset's count against
// the rule (each row holds M - K + 1 runs, each column N - K + 1, each diagonal direction
// (M - K + 1)(N - K + 1)) and against a count over all eight directions from every cell, which
// sees each line once from each end; and the games' known values, solved by negamax from the lines
// alone.
public sealed partial class MnkGameTests
{
    private const int Draw = 0;
    private const int Win = 1;
    private const long EmptyBoard = 0;

    public static TheoryData<int, int, int> ConnectFourCells => new()
    {
        { 0, 0, 0 },
        { 0, 6, 6 },
        { 1, 2, 9 },
        { 5, 6, 41 },
    };

    [Fact]
    public void CellCount_IsColumnsTimesRows() =>
        Assert.Equal(
            [9, 225, 42],
            [
                MnkGame<TicTacToeBoard>.CellCount,
                MnkGame<GomokuBoard>.CellCount,
                MnkGame<ConnectFourBoard>.CellCount,
            ]);

    [Theory]
    [MemberData(nameof(ConnectFourCells))]
    public void CellAt_NumbersCellsRowByRowFromTheTopLeft(int row, int column, int expected) =>
        Assert.Equal(expected, MnkGame<ConnectFourBoard>.CellAt(row, column));

    [Fact]
    public void Board_HasEveryCellAndNoHole() =>
        Assert.Equal(MnkGame<GomokuBoard>.CellCount, MnkGame<GomokuBoard>.Board.PlayableCount);

    [Fact]
    public void Lines_TicTacToe_AreItsEightLines_RowsThenColumnsThenTheDiagonals() =>
        Assert.Equal(
            [
                [0, 1, 2], [3, 4, 5], [6, 7, 8],
                [0, 3, 6], [1, 4, 7], [2, 5, 8],
                [0, 4, 8], [2, 4, 6],
            ],
            AllLines<TicTacToeBoard>());

    [Fact]
    public void Lines_ConnectFour_AreSixtyNine() =>
        Assert.Equal(69, MnkGame<ConnectFourBoard>.Lines.Count);

    [Fact]
    public void Lines_Gomoku_CountIsTheRule() =>
        Assert.Equal(
            CountByRule(GomokuBoard.Columns, GomokuBoard.Rows, GomokuBoard.InARow),
            MnkGame<GomokuBoard>.Lines.Count);

    [Fact]
    public void Lines_EveryPreset_CountsAsEveryRunSeenFromBothEnds() =>
        Assert.Equal(
            [
                CountFromBothEnds<TicTacToeBoard>(),
                CountFromBothEnds<GomokuBoard>(),
                CountFromBothEnds<ConnectFourBoard>(),
            ],
            [
                MnkGame<TicTacToeBoard>.Lines.Count,
                MnkGame<GomokuBoard>.Lines.Count,
                MnkGame<ConnectFourBoard>.Lines.Count,
            ]);

    [Fact]
    public void Lines_ConnectFour_EachIsFourCellsInAStraightRun() =>
        Assert.All(
            AllLines<ConnectFourBoard>(),
            line => Assert.True(IsStraightRun<ConnectFourBoard>(line)));

    [Fact]
    public void Lines_Gomoku_AreAllDifferent() =>
        Assert.Equal(
            MnkGame<GomokuBoard>.Lines.Count,
            AllLines<GomokuBoard>().Select(SortedCells).Distinct().Count());

    [Fact]
    public void Lines_TicTacToe_SolvedByNegamax_IsADraw() =>
        Assert.Equal(Draw, ValueOf(new LineGame(MnkGame<TicTacToeBoard>.Lines)));

    [Fact]
    public void Lines_FourByThreeThreeInARow_SolvedByNegamax_IsAFirstPlayerWin() =>
        Assert.Equal(Win, ValueOf(new LineGame(MnkGame<FourByThreeBoard>.Lines)));

    private static int ValueOf(LineGame game) =>
        Negamax.Value<LineGame, long, int>(game, EmptyBoard);

    private static string SortedCells(int[] line) => string.Join(',', line.Order());

    private static int[][] AllLines<TShape>()
        where TShape : struct, IMnkShape
    {
        var lines = MnkGame<TShape>.Lines;

        return [.. Enumerable.Range(0, lines.Count).Select(line => lines.CellsOf(line).ToArray())];
    }

    // Rows, columns, and the two diagonal directions, each counted by where a run may start.
    private static int CountByRule(int columns, int rows, int inARow)
    {
        var startsAlong = columns - inARow + 1;
        var startsDown = rows - inARow + 1;

        return (rows * startsAlong) + (columns * startsDown) + (2 * startsAlong * startsDown);
    }

    // Every cell and every one of the eight neighbour steps whose run of K stays on the board; each
    // line is reached from both of its ends, so the count is twice the lines.
    private static int CountFromBothEnds<TShape>()
        where TShape : struct, IMnkShape
    {
        var runs = 0;

        for (var row = 0; row < TShape.Rows; row++)
        {
            for (var column = 0; column < TShape.Columns; column++)
            {
                var start = (row, column);
                runs += GridDirections.Octilinear.Count(step => IsRunOnBoard<TShape>(start, step));
            }
        }

        return runs / 2;
    }

    private static bool IsRunOnBoard<TShape>((int Row, int Column) start, GridStep step)
        where TShape : struct, IMnkShape
    {
        var lastRow = start.Row + ((TShape.InARow - 1) * step.DeltaRow);
        var lastColumn = start.Column + ((TShape.InARow - 1) * step.DeltaCol);

        return (uint)lastRow < (uint)TShape.Rows && (uint)lastColumn < (uint)TShape.Columns;
    }

    // K cells, each one octilinear step from the one before, always the same step.
    private static bool IsStraightRun<TShape>(int[] line)
        where TShape : struct, IMnkShape
    {
        var steps = line.Zip(line.Skip(1), StepBetween<TShape>).Distinct().ToList();
        var isOneStep = steps.Count == 1 && GridDirections.Octilinear.Contains(steps[0]);

        return line.Length == TShape.InARow && isOneStep;
    }

    private static GridStep StepBetween<TShape>(int from, int to)
        where TShape : struct, IMnkShape
    {
        var rows = (to / TShape.Columns) - (from / TShape.Columns);
        var columns = (to % TShape.Columns) - (from % TShape.Columns);

        return new(rows, columns);
    }
}
