namespace DSAExperimentation.Domain.Games.PositionalGames.InARow;

// An m,n,k game as the game contract sees it, on a board of up to 256 cells: Gomoku's 225 and any
// m,n,k board up to 15 columns by 16 rows, any of whose cells may be holes. Two players alternate
// claiming open cells, the first to make k in a row wins, and a full board is a draw. It is the
// strategy for boards beyond BitboardGame's 64 cells, behind the same layer (IBitboardGame): the
// same moves, numbered by the open cells lowest first, the same values, WinValue less the claimed
// cells, and the same refusals. What sets it apart is how a claim is judged. BitboardGame tests the
// mask of each winning set through the cell; this game shifts the player's cells along the four
// line directions and ands them (InARowLines), so its rule is k in a row, not any family of sets,
// and it can play standard gomoku's exactly five (the ExactlyK overline rule), which no family of
// sets states. A position is Bitboard256Position, two 256-bit masks and whether the last claim won,
// a functional value each move replaces. Cells are numbered row by row as MnkGame numbers them;
// their bits are InARowLines' layout.
/// <summary>
/// An m,n,k game on up to 256 cells as an alternating game over 256-bit mask positions, its runs
/// found by shifting along the line directions.
/// </summary>
/// <typeparam name="TShape">The board's columns and rows, and how many in a row win.</typeparam>
/// <typeparam name="TOverline">Whether a run longer than k wins.</typeparam>
public readonly struct InARowGame<TShape, TOverline> : IBitboardGame<Bitboard256Position>
    where TShape : struct, IMnkShape
    where TOverline : struct, IOverlineRule
{
    /// <summary>
    /// What a win is worth before one is taken off for each claimed cell: the same as on a board
    /// of at most 64 cells, so the two forms value a position alike.
    /// </summary>
    public const int WinValue = BitboardGame<FreePlacement>.WinValue;

    /// <summary>Gets the game's lines, read on the board's masks.</summary>
    public InARowLines<TShape, TOverline> Lines { get; }

    /// <summary>Initializes a game on a board of the shape, with or without holes.</summary>
    /// <param name="board">The board's cells and holes, numbered as the shape numbers them.</param>
    /// <exception cref="ArgumentException">
    /// The shape does not fit a 256-bit mask with a column to spare, or the board numbers a
    /// different count of cells.
    /// </exception>
    public InARowGame(BoardMask board) => Lines = new(board);

    /// <summary>Finds the open cells: on the board and claimed by nobody.</summary>
    /// <param name="position">A position of this game.</param>
    /// <returns>The open cells' mask; move m claims its m-th cell, lowest first.</returns>
    public Bitboard256 OpenCells(Bitboard256Position position) => Lines.Board & ~position.Claimed;

    /// <inheritdoc/>
    public int ChoiceOf(Bitboard256Position position, int move)
    {
        var bit = OpenCells(position).NthCell(move);

        return InARowLines<TShape, TOverline>.CellOf(bit);
    }

    /// <inheritdoc/>
    public Bitboard256Position Replay(ReadOnlySpan<int> choices)
    {
        var position = default(Bitboard256Position);

        for (var index = 0; index < choices.Length; index++)
        {
            var choice = choices[index];
            var isOpen = IsOpen(position, choice);

            if (position.IsLost || !isOpen)
            {
                throw new ArgumentException(
                    $"Choice {index} {BitboardGame<FreePlacement>.NotOpen}", nameof(choices));
            }

            position = Claim(position, InARowLines<TShape, TOverline>.BitOf(choice));
        }

        return position;
    }

    private bool IsOpen(Bitboard256Position position, int choice)
    {
        if ((uint)choice >= (uint)MnkGame<TShape>.CellCount)
        {
            return false;
        }

        var bit = InARowLines<TShape, TOverline>.BitOf(choice);

        return OpenCells(position).HasCell(bit);
    }

    /// <inheritdoc/>
    public int MoveCount(Bitboard256Position position)
    {
        if (position.IsLost)
        {
            return 0;
        }

        return OpenCells(position).Count;
    }

    /// <inheritdoc/>
    public Bitboard256Position Next(Bitboard256Position position, int move)
    {
        var bit = OpenCells(position).NthCell(move);

        return Claim(position, bit);
    }

    /// <inheritdoc/>
    public int Gain(Bitboard256Position position, int move) => 0;

    /// <inheritdoc/>
    public int ValueWhenOver(Bitboard256Position position)
    {
        if (!position.IsLost)
        {
            return BitboardGame<FreePlacement>.Draw;
        }

        return position.Claimed.Count - WinValue;
    }

    // The mover claims the cell, and the other player moves next.
    private static Bitboard256Position Claim(Bitboard256Position position, int bit)
    {
        var held = position.Mover | Bitboard256.Cell(bit);
        var hasWon = InARowLines<TShape, TOverline>.HasWinningRun(held);

        return new(position.Opponent, held, hasWon);
    }
}
