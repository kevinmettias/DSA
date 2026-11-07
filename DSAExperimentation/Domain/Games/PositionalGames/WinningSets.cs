namespace DSAExperimentation.Domain.Games.PositionalGames;

// The winning sets of a positional game: a hypergraph on the board's cells, each edge a set of
// cells that one player must claim all of to win. Tic-tac-toe's are its eight lines and an m,n,k
// game's every k in a row, but any family of cell sets makes a game (Hales-Jewett, Beck): the four
// corners, the lines of a finite projective plane, a pattern a variant names. The sets are held one
// after another in one array and the sets through each cell in another (the incidence), both built
// once, so a move is judged by reading only the sets it lies on.
/// <summary>
/// The winning sets of a positional game: sets of cells, and the sets through each cell.
/// </summary>
public sealed class WinningSets
{
    // Internal, and read by InARowLines, which refuses the same mismatch.
    internal const string DifferentBoard = "The board numbers a different count of cells.";

    // Every set's cells, set after set; set s runs from _setStarts[s] to _setStarts[s + 1].
    private readonly int[] _cells;
    private readonly int[] _setStarts;

    // Every cell's sets, cell after cell, in set order; cell c runs from _cellStarts[c] to
    // _cellStarts[c + 1].
    private readonly int[] _incidence;
    private readonly int[] _cellStarts;

    /// <summary>Gets how many cells the board numbers.</summary>
    public int CellCount { get; }

    /// <summary>Gets how many winning sets there are.</summary>
    public int Count => _setStarts.Length - 1;

    /// <summary>Initializes the winning sets on a board's numbered cells.</summary>
    /// <param name="cellCount">How many cells the board numbers.</param>
    /// <param name="sets">The winning sets, each a list of distinct cells.</param>
    /// <exception cref="ArgumentException">
    /// A set is empty, names a cell twice, or names a cell the board does not number.
    /// </exception>
    public WinningSets(int cellCount, IReadOnlyList<int[]> sets)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(cellCount);
        CheckSets(cellCount, sets);
        CellCount = cellCount;
        _cells = [.. sets.SelectMany(set => set)];
        _setStarts = RunStarts([.. sets.Select(set => set.Length)]);
        var setsPerCell = SetsPerCell(_cells, cellCount);
        _cellStarts = RunStarts(setsPerCell);
        _incidence = IncidenceOf(_cells, _setStarts, _cellStarts);
    }

    /// <summary>Lists the cells of one winning set.</summary>
    /// <param name="set">The set's number, from 0 below <see cref="Count"/>.</param>
    /// <returns>Its cells, in the order it was given.</returns>
    public ReadOnlySpan<int> CellsOf(int set) =>
        _cells.AsSpan(_setStarts[set].._setStarts[set + 1]);

    /// <summary>Lists the winning sets a cell lies on.</summary>
    /// <param name="cell">The cell's number.</param>
    /// <returns>The sets' numbers, smallest first.</returns>
    public ReadOnlySpan<int> SetsThrough(int cell) =>
        _incidence.AsSpan(_cellStarts[cell].._cellStarts[cell + 1]);

    /// <summary>
    /// Keeps only the winning sets that lie wholly on a board: a hole removes every set through it.
    /// </summary>
    /// <param name="board">The board, numbering as many cells as these sets do.</param>
    /// <returns>
    /// The sets with no cell in a hole, in their order; these sets themselves when none has one.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// The board numbers a different count of cells.
    /// </exception>
    public WinningSets Avoiding(BoardMask board)
    {
        var isSameBoard = board.CellCount == CellCount;

        if (!isSameBoard)
        {
            throw new ArgumentException(DifferentBoard, nameof(board));
        }

        var kept = new List<int[]>(Count);

        for (var set = 0; set < Count; set++)
        {
            if (IsWhollyOn(board, set))
            {
                kept.Add(CellsOf(set).ToArray());
            }
        }

        return kept.Count == Count ? this : new WinningSets(CellCount, kept);
    }

    private bool IsWhollyOn(BoardMask board, int set)
    {
        foreach (var cell in CellsOf(set))
        {
            if (!board.HasCell(cell))
            {
                return false;
            }
        }

        return true;
    }

    private static void CheckSets(int cellCount, IReadOnlyList<int[]> sets)
    {
        for (var set = 0; set < sets.Count; set++)
        {
            if (!IsDistinctCells(sets[set], cellCount))
            {
                var problem = $"Winning set {set} is empty, repeats a cell or leaves the board.";

                throw new ArgumentException(problem, nameof(sets));
            }
        }
    }

    // A set is at least one cell, each numbered on the board and none named twice.
    private static bool IsDistinctCells(int[] cells, int cellCount)
    {
        if (cells.Length == 0)
        {
            return false;
        }

        for (var index = 0; index < cells.Length; index++)
        {
            var cell = cells[index];
            var isNumbered = (uint)cell < (uint)cellCount;
            var isRepeat = cells.AsSpan(0, index).Contains(cell);

            if (!isNumbered || isRepeat)
            {
                return false;
            }
        }

        return true;
    }

    // Where each run begins when runs of the given lengths are laid end to end, and one more entry
    // where the last ends.
    private static int[] RunStarts(int[] lengths)
    {
        var starts = new int[lengths.Length + 1];

        for (var run = 0; run < lengths.Length; run++)
        {
            starts[run + 1] = starts[run] + lengths[run];
        }

        return starts;
    }

    private static int[] SetsPerCell(int[] cells, int cellCount)
    {
        var counts = new int[cellCount];

        foreach (var cell in cells)
        {
            counts[cell]++;
        }

        return counts;
    }

    // The sets through each cell, cell after cell, each cell's in set order: every set's cells are
    // walked in set order, and each lands at the next free place in its cell's run.
    private static int[] IncidenceOf(int[] cells, int[] setStarts, int[] cellStarts)
    {
        var incidence = new int[cells.Length];
        var next = cellStarts[..^1];

        for (var set = 0; set + 1 < setStarts.Length; set++)
        {
            for (var at = setStarts[set]; at < setStarts[set + 1]; at++)
            {
                incidence[next[cells[at]]++] = set;
            }
        }

        return incidence;
    }
}
