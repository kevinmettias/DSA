using DSAExperimentation.Domain.Games.PositionalGames;
using DSAExperimentation.Domain.Games.PositionalGames.InARow;
using DSAExperimentation.Tests.Domain.Games.PositionalGames.InARow.Fixtures;
using GomokuLines = DSAExperimentation.Domain.Games.PositionalGames.InARow.InARowLines<
    DSAExperimentation.Domain.Games.PositionalGames.InARow.GomokuBoard,
    DSAExperimentation.Domain.Games.PositionalGames.InARow.KOrMore>;
using StandardLines = DSAExperimentation.Domain.Games.PositionalGames.InARow.InARowLines<
    DSAExperimentation.Domain.Games.PositionalGames.InARow.GomokuBoard,
    DSAExperimentation.Domain.Games.PositionalGames.InARow.ExactlyK>;

namespace DSAExperimentation.Tests.Domain.Games.PositionalGames.InARow;

// The shifting rule held to MnkGame's winning lines read one at a time (LineOracle), on Gomoku's
// board, rows numbered from the top: on boards drawn at random at every density, so runs reach
// every edge and corner, on boards with holes, and on runs placed by hand where a shift could wrap
// from the end of one row to the start of the next. Every read is checked under both overline
// rules, and the open cells of lines lacking one or two cells against what they mean: a move there
// wins, or leaves a winning cell.
public sealed partial class InARowLinesTests
{
    private const int RandomBoards = 120;
    private const int Seed = 1994;
    private const int Last = GomokuBoard.Size - 1;

    public static TheoryData<int> Densities => new() { 10, 30, 50, 70, 90 };

    // Five cells in a straight run touching the board's edges, each a win.
    public static TheoryData<int[]> RunsAtTheEdges => new()
    {
        { Run((0, Last - 4), 0, 1) },
        { Run((Last, 0), 0, 1) },
        { Run((Last - 4, Last), 1, 0) },
        { Run((0, 0), 1, 1) },
        { Run((Last - 4, Last - 4), 1, 1) },
        { Run((0, Last), 1, -1) },
        { Run((Last - 4, 4), 1, -1) },
    };

    // Five cells that would be a run if a row ran on into the next: none is a line.
    public static TheoryData<int[]> RunsAcrossARowsEnd => new()
    {
        { [Cell(3, Last - 2), Cell(3, Last - 1), Cell(3, Last), Cell(4, 0), Cell(4, 1)] },
        { [Cell(3, Last - 1), Cell(4, Last), Cell(5, 0), Cell(6, 1), Cell(7, 2)] },
        { [Cell(3, 1), Cell(4, 0), Cell(4, Last), Cell(5, Last - 1), Cell(6, Last - 2)] },
    };

    [Theory]
    [MemberData(nameof(Densities))]
    public void HasWinningRun_RandomBoards_IsTheLinesRule(int percent) =>
        Assert.All(Boards(percent), cells => Assert.Equal(
            (LineOracle<GomokuBoard, KOrMore>.HasRun(Full(), cells.Mine),
                LineOracle<GomokuBoard, ExactlyK>.HasRun(Full(), cells.Mine)),
            (GomokuLines.HasWinningRun(Mask(cells.Mine)),
                StandardLines.HasWinningRun(Mask(cells.Mine)))));

    [Theory]
    [MemberData(nameof(RunsAtTheEdges))]
    public void HasWinningRun_FiveAtTheEdge_Wins(int[] run) =>
        Assert.True(GomokuLines.HasWinningRun(GomokuLines.MaskOf(run)));

    [Theory]
    [MemberData(nameof(RunsAcrossARowsEnd))]
    public void HasWinningRun_FiveAcrossARowsEnd_DoesNotWin(int[] cells) =>
        Assert.False(GomokuLines.HasWinningRun(GomokuLines.MaskOf(cells)));

    [Fact]
    public void HasWinningRun_SixInARow_WinsUnderKOrMoreOnly()
    {
        var six = GomokuLines.MaskOf(Run((7, 2), 0, 1, 6));

        Assert.Equal(
            (true, false), (GomokuLines.HasWinningRun(six), StandardLines.HasWinningRun(six)));
    }

    // Six in a row along the top and an exact five down the left edge: the five wins either way.
    [Fact]
    public void HasWinningRun_ASixBesideAnExactFive_WinsUnderExactlyK()
    {
        var cells = GomokuLines.MaskOf([.. Run((0, 4), 0, 1, 6), .. Run((2, 0), 1, 0)]);

        Assert.True(StandardLines.HasWinningRun(cells));
    }

    [Theory]
    [MemberData(nameof(Densities))]
    public void CellsLacking_OneOrTwo_IsTheLinesRule(int percent) =>
        Assert.All(Boards(percent), cells => Assert.Equal(
            Expected<KOrMore>(cells),
            Found(Gomoku(), cells)));

    [Theory]
    [MemberData(nameof(Densities))]
    public void CellsLacking_OneOrTwoUnderExactlyK_IsTheLinesRule(int percent) =>
        Assert.All(Boards(percent), cells => Assert.Equal(
            Expected<ExactlyK>(cells),
            FoundExactly(Standard(), cells)));

    // A cell lacking one is where the move wins, on the boards where the player has not won yet: a
    // run already made would win after any move.
    [Theory]
    [MemberData(nameof(Densities))]
    public void CellsLacking_One_IsWhereTheMoveWins(int percent) =>
        Assert.All(Boards(percent).Where(HasNoRun), cells =>
        {
            var (mine, theirs) = (Mask(cells.Mine), Mask(cells.Theirs));
            var open = Gomoku().Board & ~mine & ~theirs;

            Assert.Equal(
                CellsWhere(open, bit => GomokuLines.HasWinningRun(mine | Bitboard256.Cell(bit))),
                Cells(Gomoku().CellsLacking(mine, theirs, 1)));
        });

    // A cell lacking two is where the move leaves a cell lacking one, a four, on the boards where
    // the player has no such cell yet: a four already made would leave one after any move.
    [Theory]
    [MemberData(nameof(Densities))]
    public void CellsLacking_TwoUnderExactlyK_IsWhereTheMoveMakesAFour(int percent) =>
        Assert.All(Boards(percent).Where(HasNoFour), cells =>
        {
            var (mine, theirs) = (Mask(cells.Mine), Mask(cells.Theirs));
            var open = Standard().Board & ~mine & ~theirs;
            var makesAFour = (int bit) =>
                !Standard().CellsLacking(mine | Bitboard256.Cell(bit), theirs, 1).IsEmpty;

            Assert.Equal(
                CellsWhere(open, makesAFour), Cells(Standard().CellsLacking(mine, theirs, 2)));
        });

    [Theory]
    [MemberData(nameof(Densities))]
    public void CountLines_RandomBoards_IsTheLinesCount(int percent) =>
        Assert.All(Boards(percent), cells =>
        {
            var counts = new int[GomokuBoard.InARow + 1];
            Gomoku().CountLines(Mask(cells.Mine), Mask(cells.Theirs), counts);

            Assert.Equal(
                LineOracle<GomokuBoard, KOrMore>.CountLines(Full(), cells.Mine, cells.Theirs), counts);
        });

    // Holes leave out every line through them, and the shifts step over them as over an empty cell
    // nobody can claim.
    [Fact]
    public void Reads_BoardsWithHoles_AreTheLinesRuleOnTheBoard()
    {
        var random = new Random(Seed);
        int[] holes =
            [.. Enumerable.Range(0, MnkGame<GomokuBoard>.CellCount).Where(_ => random.Next(8) == 0)];
        var board = new BoardMask(MnkGame<GomokuBoard>.CellCount, holes);
        var lines = new GomokuLines(board);

        Assert.All(Enumerable.Range(0, RandomBoards), _ =>
        {
            var (mine, theirs) = LineOracle<GomokuBoard, KOrMore>.RandomCells(board, random, 60);
            var counts = new int[GomokuBoard.InARow + 1];
            lines.CountLines(Mask(mine), Mask(theirs), counts);

            Assert.Equal(
                [LineOracle<GomokuBoard, KOrMore>.CellsLacking(board, mine, theirs, 1),
                    LineOracle<GomokuBoard, KOrMore>.CountLines(board, mine, theirs)],
                [Cells(lines.CellsLacking(Mask(mine), Mask(theirs), 1)), counts]);
        });
    }

    // Every board cell next to one of the cells, in all eight directions, from the corners too.
    [Fact]
    public void Around_Cells_IsEveryBoardCellAStepAway()
    {
        int[] cells = [Cell(0, 0), Cell(Last, Last), Cell(7, Last), Cell(9, 0), Cell(5, 5)];
        int[] expected =
        [
            .. Enumerable.Range(0, MnkGame<GomokuBoard>.CellCount)
                .Where(cell => !cells.Contains(cell))
                .Where(cell => cells.Any(other => IsAStepApart(cell, other))),
        ];

        Assert.Equal(expected, Cells(Gomoku().Around(GomokuLines.MaskOf(cells))));
    }

    [Fact]
    public void BitOf_ACell_IsItsRowTimesTheStridePlusItsColumn() =>
        Assert.Equal(
            [0, Last, GomokuLines.Stride, (Last * GomokuLines.Stride) + Last],
            [GomokuLines.BitOf(0), GomokuLines.BitOf(Last), GomokuLines.BitOf(GomokuBoard.Size),
                GomokuLines.BitOf(MnkGame<GomokuBoard>.CellCount - 1)]);

    [Fact]
    public void MaskOf_TheFirstCellOfEachRow_HoldsEachRowsFirstBit() =>
        Assert.Equal(
            Bitboard256.Of([.. Rows().Select(row => row * GomokuLines.Stride)]),
            GomokuLines.MaskOf([.. Rows().Select(row => Cell(row, 0))]));

    [Fact]
    public void CellOf_EveryCellsBit_IsTheCell() =>
        Assert.All(
            Enumerable.Range(0, MnkGame<GomokuBoard>.CellCount),
            cell => Assert.Equal(cell, GomokuLines.CellOf(GomokuLines.BitOf(cell))));

    [Fact]
    public void Board_Gomoku_HoldsEveryCellAndNoBitOfTheSpareColumn() =>
        Assert.Equal(
            (MnkGame<GomokuBoard>.CellCount, false),
            (Gomoku().Board.Count, Gomoku().Board.HasCell(GomokuLines.Stride - 1)));

    [Fact]
    public void Constructor_SixteenColumns_IsRefused() =>
        Assert.Throws<ArgumentException>(
            () => new InARowLines<SixteenByTwoBoard, KOrMore>(MnkGame<SixteenByTwoBoard>.Board));

    [Fact]
    public void Constructor_ABoardOfAnotherSize_IsRefused() =>
        Assert.Throws<ArgumentException>(() => new GomokuLines(BoardMask.Full(GomokuBoard.Size)));

    private static IEnumerable<int> Rows() => Enumerable.Range(0, GomokuBoard.Size);

    private static BoardMask Full() => MnkGame<GomokuBoard>.Board;

    private static GomokuLines Gomoku() => new(Full());

    private static StandardLines Standard() => new(Full());

    private static int Cell(int row, int column) => MnkGame<GomokuBoard>.CellAt(row, column);

    // Cells in a straight run from a start, a step at a time.
    private static int[] Run(
        (int Row, int Column) start, int rowStep, int columnStep, int length = 5) =>
        [.. Enumerable.Range(0, length)
            .Select(at => Cell(start.Row + (at * rowStep), start.Column + (at * columnStep)))];

    private static bool IsAStepApart(int cell, int other)
    {
        var rows = Math.Abs((cell / GomokuBoard.Size) - (other / GomokuBoard.Size));
        var columns = Math.Abs((cell % GomokuBoard.Size) - (other % GomokuBoard.Size));

        return Math.Max(rows, columns) == 1;
    }

    private static IEnumerable<(bool[] Mine, bool[] Theirs)> Boards(int percent)
    {
        var random = new Random(Seed + percent);

        return [.. Enumerable.Range(0, RandomBoards)
            .Select(_ => LineOracle<GomokuBoard, KOrMore>.RandomCells(Full(), random, percent))];
    }

    private static bool HasNoRun((bool[] Mine, bool[] Theirs) cells) =>
        !GomokuLines.HasWinningRun(Mask(cells.Mine));

    private static bool HasNoFour((bool[] Mine, bool[] Theirs) cells) =>
        Standard().CellsLacking(Mask(cells.Mine), Mask(cells.Theirs), 1).IsEmpty;

    private static Bitboard256 Mask(bool[] cells) =>
        GomokuLines.MaskOf(LineOracle<GomokuBoard, KOrMore>.CellsOf(cells));

    // A mask's cells, numbered as the board numbers them, smallest first.
    private static int[] Cells(Bitboard256 mask)
    {
        var cells = new List<int>();

        for (var rest = mask; !rest.IsEmpty; rest = rest.WithoutLowestCell())
        {
            cells.Add(GomokuLines.CellOf(rest.LowestCell));
        }

        return [.. cells];
    }

    private static int[] CellsWhere(Bitboard256 open, Func<int, bool> holds)
    {
        var cells = new List<int>();

        for (var rest = open; !rest.IsEmpty; rest = rest.WithoutLowestCell())
        {
            if (holds(rest.LowestCell))
            {
                cells.Add(GomokuLines.CellOf(rest.LowestCell));
            }
        }

        return [.. cells];
    }

    // The cells lacking one and lacking two, in that order.
    private static int[][] Expected<TOverline>((bool[] Mine, bool[] Theirs) cells)
        where TOverline : struct, IOverlineRule =>
    [
        LineOracle<GomokuBoard, TOverline>.CellsLacking(Full(), cells.Mine, cells.Theirs, 1),
        LineOracle<GomokuBoard, TOverline>.CellsLacking(Full(), cells.Mine, cells.Theirs, 2),
    ];

    private static int[][] Found(GomokuLines lines, (bool[] Mine, bool[] Theirs) cells) =>
    [
        Cells(lines.CellsLacking(Mask(cells.Mine), Mask(cells.Theirs), 1)),
        Cells(lines.CellsLacking(Mask(cells.Mine), Mask(cells.Theirs), 2)),
    ];

    private static int[][] FoundExactly(StandardLines lines, (bool[] Mine, bool[] Theirs) cells) =>
    [
        Cells(lines.CellsLacking(Mask(cells.Mine), Mask(cells.Theirs), 1)),
        Cells(lines.CellsLacking(Mask(cells.Mine), Mask(cells.Theirs), 2)),
    ];

    // The windows' counts, driven through CountLines: on the empty board every line holds
    // nothing, and a lone stone in the corner lies on three lines.
    public sealed partial class WindowCountsTests
    {
        [Fact]
        public void Holding_EmptyBoard_IsEveryLineAtNothing()
        {
            var counts = new int[GomokuBoard.InARow + 1];
            Gomoku().CountLines(Bitboard256.Empty, Bitboard256.Empty, counts);

            Assert.Equal([MnkGame<GomokuBoard>.Lines.Count, 0, 0, 0, 0, 0], counts);
        }

        [Fact]
        public void Holding_ACornerStone_IsThreeLinesAtOne()
        {
            var counts = new int[GomokuBoard.InARow + 1];
            Gomoku().CountLines(GomokuLines.MaskOf([0]), Bitboard256.Empty, counts);

            Assert.Equal(3, counts[1]);
        }
    }
}
