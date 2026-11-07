using System.Runtime.CompilerServices;

namespace DSAExperimentation.Domain.Games.PositionalGames.InARow;

// The lines of an m,n,k game on a board held as 256-bit masks, read by shifting. A cell's bit is
// its row times 16 plus its column: a stride of 16 on a board at most 15 columns wide, so every row
// ends in a column no cell occupies. A step along one of the line directions
// (GridDirections.LineDirections: row, column, diagonal, anti-diagonal) is then a shift by its rows
// times the stride plus its columns: 1, 16, 17 and 15. A run of cells that leaves the board's right
// or left edge steps into the empty column, so a run found by shifting and anding a player's cells
// never wraps onto the next row, and no column mask is needed; a step off the top or the bottom is
// shifted out of the mask.
//
// A line is a window of k cells along a direction, named by its first cell, so a mask of window
// starts holds one bit per line. The windows a player can still complete are those of k cells that
// are on the board and not the opponent's: the AND of k shifts of those cells. How many of a
// window's cells the player holds is added up a cell at a time in three bit planes, one count per
// window at once, which is why k is at most 7. With the counts, the rule's questions are a few
// masks: a run of k (exactly k when the overline rule says only k wins, the cells beyond both ends
// not the player's), the open cells of lines a player holds all but one cell of (where the player
// wins next move), all but two (where a move makes a four), and how many lines a player alone holds
// each count of cells of, which a static evaluation weighs. Each direction is a call with its shift
// a constant, so every shift folds to the words it moves.
/// <summary>
/// The lines of an m,n,k game on a board held as 256-bit masks: its runs, and the lines a player
/// can still complete, read by shifting along the four line directions.
/// </summary>
/// <typeparam name="TShape">The board's columns and rows, and how many in a row win.</typeparam>
/// <typeparam name="TOverline">Whether a run longer than k wins.</typeparam>
public readonly struct InARowLines<TShape, TOverline>
    where TShape : struct, IMnkShape
    where TOverline : struct, IOverlineRule
{
    // The bits from one row's first cell to the next row's. Internal, as the shortfalls below are.
    internal const int Stride = 16;

    // How many of a line's cells a player lacks, as CellsLacking reads it: one, where the next
    // move wins; two, where it makes a four; three, where it makes a three. Internal, so that a
    // solution's own small numbers are not read as copies of them (ARCHITECTURE 17.9).
    internal const int OneShort = 1;
    internal const int TwoShort = 2;
    internal const int ThreeShort = 3;

    // The shift of one step along each line direction: right, down, down-right, down-left.
    private const int RowStep = 1;
    private const int ColumnStep = Stride;
    private const int DiagonalStep = Stride + 1;
    private const int AntiDiagonalStep = Stride - 1;

    // A row leaves its last bit empty, the rows fit the mask, and a window's count fits three bit
    // planes.
    private const int MostColumns = Stride - 1;
    private const int MostRows = Bitboard256.Capacity / Stride;
    private const int MostInARow = 7;

    private const string WrongShape =
        "A board held as 256-bit masks is at most 15 columns by 16 rows, with 1 to 7 in a row.";

    /// <summary>Gets the board's cells as a mask: every cell but the holes.</summary>
    public Bitboard256 Board { get; }

    /// <summary>Initializes the lines on a board of the shape, with or without holes.</summary>
    /// <param name="board">The board's cells and holes, numbered as the shape numbers them.</param>
    /// <exception cref="ArgumentException">
    /// The shape does not fit a 256-bit mask with a column to spare, or the board numbers a
    /// different count of cells.
    /// </exception>
    public InARowLines(BoardMask board)
    {
        if (!IsShapeHeld())
        {
            throw new ArgumentException(WrongShape, nameof(board));
        }

        if (board.CellCount != MnkGame<TShape>.CellCount)
        {
            throw new ArgumentException(WinningSets.DifferentBoard, nameof(board));
        }

        Board = BoardCells(board);
    }

    private static bool IsShapeHeld()
    {
        var isWithinColumns = TShape.Columns is > 0 and <= MostColumns;
        var isWithinRows = TShape.Rows is > 0 and <= MostRows;

        return isWithinColumns && isWithinRows && TShape.InARow is > 0 and <= MostInARow;
    }

    private static Bitboard256 BoardCells(BoardMask board)
    {
        var cells = Bitboard256.Empty;

        for (var cell = 0; cell < board.CellCount; cell++)
        {
            if (board.HasCell(cell))
            {
                cells |= Bitboard256.Cell(BitOf(cell));
            }
        }

        return cells;
    }

    /// <summary>Finds a cell's bit: its row times 16 plus its column.</summary>
    /// <param name="cell">The cell, numbered as <see cref="MnkGame{TShape}"/> numbers it.</param>
    /// <returns>The cell's bit in a mask.</returns>
    public static int BitOf(int cell) =>
        cell + ((cell / TShape.Columns) * (Stride - TShape.Columns));

    /// <summary>Finds the cell a bit stands for.</summary>
    /// <param name="bit">The bit of a cell on the board.</param>
    /// <returns>The cell, numbered as <see cref="MnkGame{TShape}"/> numbers it.</returns>
    public static int CellOf(int bit) => ((bit / Stride) * TShape.Columns) + (bit % Stride);

    /// <summary>Makes the mask of a list of cells.</summary>
    /// <param name="cells">Cells numbered as <see cref="MnkGame{TShape}"/> numbers them.</param>
    /// <returns>The mask with every listed cell's bit set.</returns>
    public static Bitboard256 MaskOf(ReadOnlySpan<int> cells)
    {
        var mask = Bitboard256.Empty;

        foreach (var cell in cells)
        {
            mask |= Bitboard256.Cell(BitOf(cell));
        }

        return mask;
    }

    /// <summary>
    /// Whether a player's cells hold a winning run: k in a row along a row, a column or a diagonal,
    /// and under <see cref="ExactlyK"/> a run of exactly k.
    /// </summary>
    /// <param name="stones">The player's cells.</param>
    /// <returns><see langword="true"/> when the cells hold a run that wins.</returns>
    public static bool HasWinningRun(Bitboard256 stones) =>
        !(WinningRunStarts(stones, RowStep) | WinningRunStarts(stones, ColumnStep)
            | WinningRunStarts(stones, DiagonalStep)
            | WinningRunStarts(stones, AntiDiagonalStep)).IsEmpty;

    // The first cells of the runs that win along one direction: k of the player's cells in a row,
    // and under exactly k no cell of the player's just before the first or just after the last.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Bitboard256 WinningRunStarts(Bitboard256 stones, int step)
    {
        var starts = stones;
        var along = stones;

        for (var cell = 1; cell < TShape.InARow; cell++)
        {
            along = along.ShiftDown(step);
            starts &= along;
        }

        return WithoutOverlines(starts, stones, step);
    }

    /// <summary>
    /// Finds the open cells of the lines a player alone holds all but some cells of, the overline
    /// rule observed: with one lacking, the cells where the player's next move wins; with two,
    /// the cells where it makes a four.
    /// </summary>
    /// <param name="mine">The player's cells.</param>
    /// <param name="theirs">The other player's cells.</param>
    /// <param name="lacking">How many of a line's cells the player lacks, from 1 to k.</param>
    /// <returns>
    /// Every open cell of a line on the board holding none of <paramref name="theirs"/> and all
    /// but <paramref name="lacking"/> of its cells in <paramref name="mine"/>; under
    /// <see cref="ExactlyK"/> only lines whose cells just beyond both ends are not the player's.
    /// </returns>
    public Bitboard256 CellsLacking(Bitboard256 mine, Bitboard256 theirs, int lacking)
    {
        var held = TShape.InARow - lacking;
        var available = Board & ~theirs;
        var lines = OfLinesHeld(mine, available, held, RowStep)
            | OfLinesHeld(mine, available, held, ColumnStep)
            | OfLinesHeld(mine, available, held, DiagonalStep)
            | OfLinesHeld(mine, available, held, AntiDiagonalStep);

        return lines & available & ~mine;
    }

    // Every cell of the lines along one direction that lie on available cells and hold exactly
    // `held` of the player's, the overline rule observed.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Bitboard256 OfLinesHeld(
        Bitboard256 mine, Bitboard256 available, int held, int step)
    {
        var counts = new WindowCounts(mine, available, step);
        var starts = WithoutOverlines(counts.Holding(held), mine, step);
        var cells = starts;

        for (var cell = 1; cell < TShape.InARow; cell++)
        {
            starts = starts.ShiftUp(step);
            cells |= starts;
        }

        return cells;
    }

    /// <summary>
    /// Counts the lines a player alone holds cells of, by how many: the lines on the board holding
    /// none of the other player's cells, each counted under the number of the player's.
    /// </summary>
    /// <param name="mine">The player's cells.</param>
    /// <param name="theirs">The other player's cells.</param>
    /// <param name="byHeld">
    /// At least k + 1 long; entry j receives how many lines hold exactly j of the player's cells.
    /// </param>
    public void CountLines(Bitboard256 mine, Bitboard256 theirs, Span<int> byHeld)
    {
        var available = Board & ~theirs;
        byHeld[..(TShape.InARow + 1)].Clear();
        AddLineCounts(mine, available, RowStep, byHeld);
        AddLineCounts(mine, available, ColumnStep, byHeld);
        AddLineCounts(mine, available, DiagonalStep, byHeld);
        AddLineCounts(mine, available, AntiDiagonalStep, byHeld);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void AddLineCounts(
        Bitboard256 mine, Bitboard256 available, int step, Span<int> byHeld)
    {
        var counts = new WindowCounts(mine, available, step);

        for (var held = 0; held <= TShape.InARow; held++)
        {
            byHeld[held] += counts.Holding(held).Count;
        }
    }

    /// <summary>
    /// Finds the board's cells a step from a cell of a mask in any of the eight directions.
    /// </summary>
    /// <param name="cells">A mask of cells on the board.</param>
    /// <returns>The board's cells next to one of them, the cells themselves left out.</returns>
    public Bitboard256 Around(Bitboard256 cells)
    {
        var near = cells.ShiftUp(RowStep) | cells.ShiftDown(RowStep)
            | cells.ShiftUp(ColumnStep) | cells.ShiftDown(ColumnStep)
            | cells.ShiftUp(DiagonalStep) | cells.ShiftDown(DiagonalStep)
            | cells.ShiftUp(AntiDiagonalStep) | cells.ShiftDown(AntiDiagonalStep);

        return near & Board & ~cells;
    }

    // Under exactly k, keeps only the windows whose cells just beyond both ends are not the
    // player's; under k or more, every window.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Bitboard256 WithoutOverlines(Bitboard256 starts, Bitboard256 stones, int step)
    {
        if (TOverline.IsOverlineAWin)
        {
            return starts;
        }

        var before = stones.ShiftUp(step);
        var after = stones.ShiftDown(TShape.InARow * step);

        return starts & ~before & ~after;
    }

    // How many of each window's cells a player holds, as three bit planes over the windows' first
    // cells, counting only the windows wholly on available cells.
    private readonly struct WindowCounts
    {
        // The bit of a count each plane holds.
        private const int OnesBit = 1;
        private const int TwosBit = 2;
        private const int FoursBit = 4;

        private readonly Bitboard256 _windows;
        private readonly Bitboard256 _ones;
        private readonly Bitboard256 _twos;
        private readonly Bitboard256 _fours;

        // Adds the window's cells one at a time, from its first: the player's cell at that place in
        // each window goes into the planes as a binary add with carries, and the window must be
        // available there.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public WindowCounts(Bitboard256 mine, Bitboard256 available, int step)
        {
            var windows = available;
            var placeAvailable = available;
            var place = mine;
            var ones = mine;
            var twos = Bitboard256.Empty;
            var fours = Bitboard256.Empty;

            for (var cell = 1; cell < TShape.InARow; cell++)
            {
                place = place.ShiftDown(step);
                placeAvailable = placeAvailable.ShiftDown(step);
                windows &= placeAvailable;
                var carry = ones & place;
                ones ^= place;
                fours |= twos & carry;
                twos ^= carry;
            }

            _windows = windows;
            _ones = ones;
            _twos = twos;
            _fours = fours;
        }

        // The windows holding exactly this many of the player's cells.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Bitboard256 Holding(int held) =>
            _windows & Plane(_ones, held & OnesBit) & Plane(_twos, held & TwosBit)
            & Plane(_fours, held & FoursBit);

        // A plane where the count has this bit, its complement where it has not.
        private static Bitboard256 Plane(Bitboard256 plane, int bit)
        {
            if (bit != 0)
            {
                return plane;
            }

            return ~plane;
        }
    }
}
