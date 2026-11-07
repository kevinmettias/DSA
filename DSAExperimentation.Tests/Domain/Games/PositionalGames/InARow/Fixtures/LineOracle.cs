using DSAExperimentation.Domain.Games.PositionalGames;
using DSAExperimentation.Domain.Games.PositionalGames.InARow;

namespace DSAExperimentation.Tests.Domain.Games.PositionalGames.InARow.Fixtures;

// The rules of an m,n,k game read off MnkGame's winning lines one line at a time, test-only: the
// generic winning-set rule the shifting forms are held to. A player's cells are a bool per cell. A
// line wins when the player holds all of it, and under exactly k also neither cell just beyond its
// ends, found by stepping from its first cell to its second.
internal static class LineOracle<TShape, TOverline>
    where TShape : struct, IMnkShape
    where TOverline : struct, IOverlineRule
{
    private const int Percent = 100;

    // Whether some line on the board is all the player's, and under exactly k no longer run.
    public static bool HasRun(BoardMask board, bool[] mine) =>
        LinesOn(board).Any(line => line.All(cell => mine[cell]) && IsFlankFree(line, mine));

    // The open cells of lines holding none of theirs and all but `lacking` of mine, smallest
    // first, the overline rule observed.
    public static int[] CellsLacking(BoardMask board, bool[] mine, bool[] theirs, int lacking)
    {
        var cells = new SortedSet<int>();

        foreach (var line in LinesOn(board))
        {
            if (IsHeldAloneLacking(line, mine, theirs, lacking))
            {
                cells.UnionWith(line.Where(cell => !mine[cell]));
            }
        }

        return [.. cells];
    }

    // How many lines hold none of theirs, by how many of mine they hold.
    public static int[] CountLines(BoardMask board, bool[] mine, bool[] theirs)
    {
        var counts = new int[TShape.InARow + 1];

        foreach (var line in LinesOn(board).Where(line => !line.Any(cell => theirs[cell])))
        {
            counts[line.Count(cell => mine[cell])]++;
        }

        return counts;
    }

    // Two players' cells drawn at random on the board, each cell open, the first's or the second's.
    public static (bool[] Mine, bool[] Theirs) RandomCells(
        BoardMask board, Random random, int percent)
    {
        var mine = new bool[MnkGame<TShape>.CellCount];
        var theirs = new bool[MnkGame<TShape>.CellCount];

        for (var cell = 0; cell < mine.Length; cell++)
        {
            var isTaken = board.HasCell(cell) && random.Next(Percent) < percent;
            var isMine = random.Next(2) == 0;
            mine[cell] = isTaken && isMine;
            theirs[cell] = isTaken && !isMine;
        }

        return (mine, theirs);
    }

    public static int[] CellsOf(bool[] cells) =>
        [.. Enumerable.Range(0, cells.Length).Where(cell => cells[cell])];

    // The player alone holds the line, all but `lacking` of it, and no longer run if exactly k.
    private static bool IsHeldAloneLacking(int[] line, bool[] mine, bool[] theirs, int lacking)
    {
        var isMineAlone = !line.Any(cell => theirs[cell]);
        var isShortBy = line.Count(cell => mine[cell]) == line.Length - lacking;

        return isMineAlone && isShortBy && IsFlankFree(line, mine);
    }

    private static IEnumerable<int[]> LinesOn(BoardMask board)
    {
        var lines = MnkGame<TShape>.Lines.Avoiding(board);

        return Enumerable.Range(0, lines.Count).Select(line => lines.CellsOf(line).ToArray());
    }

    private static bool IsFlankFree(int[] line, bool[] mine)
    {
        if (TOverline.IsOverlineAWin)
        {
            return true;
        }

        var (rowStep, columnStep) = StepOf(line);

        return !IsMine(line[0], -rowStep, -columnStep, mine)
            && !IsMine(line[^1], rowStep, columnStep, mine);
    }

    private static (int Rows, int Columns) StepOf(int[] line) =>
        ((line[1] / TShape.Columns) - (line[0] / TShape.Columns),
            (line[1] % TShape.Columns) - (line[0] % TShape.Columns));

    // Whether the cell one step on from a cell is on the board and the player's.
    private static bool IsMine(int cell, int rowStep, int columnStep, bool[] mine)
    {
        var row = (cell / TShape.Columns) + rowStep;
        var column = (cell % TShape.Columns) + columnStep;
        var isOnBoard = (uint)row < (uint)TShape.Rows && (uint)column < (uint)TShape.Columns;

        return isOnBoard && mine[MnkGame<TShape>.CellAt(row, column)];
    }
}
