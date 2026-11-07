using System.Collections.Immutable;
using System.Numerics;
using DSAExperimentation.Domain.Mathematics.GameTheory;

namespace DSAExperimentation.Domain.Games.PositionalGames;

// A two-player positional game as the game contract sees it, on a board of at most 64 cells: two
// players alternate claiming open cells where the placement rule puts them, the first to hold a
// whole winning set wins, and a board with no choice left is a draw. A position is two bit masks
// (BitboardPosition), the functional form: a move makes a new position and nothing is undone. A
// claim is judged by the winning sets through its cell alone, each a mask laid out cell by cell
// as WinningSets lays out its incidence, so a move costs a few mask tests and the position records
// the result. Moves are numbered by their choices' bits, lowest first: under gravity, move m is
// the m-th open column from the left.
//
// A win is worth WinValue less one for every claimed cell, the winning piece included, and a loss
// the negation, so a quicker win is worth more and a longer defence less bad, and best play hurries
// a win as it puts off a loss. WinValue is far beyond any count of cells or any estimate a search
// puts in place of a value, so a forced result outranks every guess.
/// <summary>
/// A two-player positional game on at most 64 cells, played by a placement rule, as an alternating
/// game over bit-mask positions.
/// </summary>
/// <typeparam name="TPlacement">Which cell a choice claims, read on a bit mask.</typeparam>
public readonly struct BitboardGame<TPlacement> : IAlternatingGame<BitboardPosition, int>
    where TPlacement : struct, IBitboardPlacement
{
    /// <summary>What a win is worth before one is taken off for each claimed cell.</summary>
    public const int WinValue = 1_000_000;

    private const int MaskBits = 64;
    private const int Draw = 0;
    private const string TooManyCells = "A bit-mask board holds at most 64 cells.";
    private const string NotOpen = "is not open, or follows the end of the game.";

    // The cells on the board, holes left out.
    private readonly ulong _board;

    // Every cell's winning sets as masks, cell after cell; cell c runs from _cellStarts[c] to
    // _cellStarts[c + 1].
    private readonly ImmutableArray<ulong> _masksThrough;
    private readonly ImmutableArray<int> _cellStarts;

    /// <summary>Gets the winning sets, every one lying wholly on the board.</summary>
    public WinningSets WinningSets { get; }

    /// <summary>
    /// Initializes a game on a board, keeping the winning sets that lie wholly on it.
    /// </summary>
    /// <param name="board">The board's cells and holes; at most 64 cells.</param>
    /// <param name="winningSets">
    /// The winning sets on the board's cells; any through a hole is dropped.
    /// </param>
    /// <exception cref="ArgumentException">
    /// The board numbers more than 64 cells, or the sets a different count of cells.
    /// </exception>
    public BitboardGame(BoardMask board, WinningSets winningSets)
    {
        if (board.CellCount > MaskBits)
        {
            throw new ArgumentException(TooManyCells, nameof(board));
        }

        WinningSets = winningSets.Avoiding(board);
        _board = BoardCells(board);
        var cellStarts = new int[board.CellCount + 1];
        _masksThrough = MasksThroughEachCell(WinningSets, cellStarts);
        _cellStarts = [.. cellStarts];
    }

    /// <summary>Finds the choices open to the player to move, one bit each.</summary>
    /// <param name="position">A position of this game.</param>
    /// <returns>The open choices; move m is the m-th set bit, lowest first.</returns>
    public ulong ChoicesOf(BitboardPosition position) => TPlacement.Choices(OpenCells(position));

    /// <summary>Names the choice a move makes, as the placement rule names it.</summary>
    /// <param name="position">A position of this game.</param>
    /// <param name="move">The move's number, from 0 below <see cref="MoveCount"/>.</param>
    /// <returns>The choice: a column under gravity, a cell under free placement.</returns>
    public int ChoiceOf(BitboardPosition position, int move) =>
        NthSetBit(ChoicesOf(position), move);

    private static int NthSetBit(ulong bits, int index)
    {
        for (var skipped = 0; skipped < index; skipped++)
        {
            bits &= bits - 1;
        }

        return BitOperations.TrailingZeroCount(bits);
    }

    /// <summary>Plays choices from the empty board and returns the position they reach.</summary>
    /// <param name="choices">The choices in order, named as the placement rule names them.</param>
    /// <returns>The position after the last choice.</returns>
    /// <exception cref="ArgumentException">
    /// A choice is not open, or follows the end of the game.
    /// </exception>
    public BitboardPosition Replay(ReadOnlySpan<int> choices)
    {
        var position = default(BitboardPosition);

        for (var index = 0; index < choices.Length; index++)
        {
            var choice = choices[index];
            var isOpen = (uint)choice < MaskBits && ((ChoicesOf(position) >> choice) & 1) != 0;

            if (position.IsLost || !isOpen)
            {
                throw new ArgumentException($"Choice {index} {NotOpen}", nameof(choices));
            }

            position = Claim(position, choice);
        }

        return position;
    }

    /// <inheritdoc/>
    public int MoveCount(BitboardPosition position)
    {
        if (position.IsLost)
        {
            return 0;
        }

        return BitOperations.PopCount(ChoicesOf(position));
    }

    /// <inheritdoc/>
    public BitboardPosition Next(BitboardPosition position, int move)
    {
        var choice = ChoiceOf(position, move);

        return Claim(position, choice);
    }

    /// <inheritdoc/>
    public int Gain(BitboardPosition position, int move) => 0;

    /// <inheritdoc/>
    public int ValueWhenOver(BitboardPosition position)
    {
        if (!position.IsLost)
        {
            return Draw;
        }

        var claimed = BitOperations.PopCount(position.Claimed);

        return claimed - WinValue;
    }

    private static ulong BoardCells(BoardMask board)
    {
        var cells = 0UL;

        for (var cell = 0; cell < board.CellCount; cell++)
        {
            if (board.HasCell(cell))
            {
                cells |= 1UL << cell;
            }
        }

        return cells;
    }

    // Each cell's sets' masks in the cell's incidence order, filling in where each cell's run
    // starts.
    private static ImmutableArray<ulong> MasksThroughEachCell(WinningSets sets, int[] cellStarts)
    {
        var masks = new List<ulong>();

        for (var cell = 0; cell < sets.CellCount; cell++)
        {
            cellStarts[cell] = masks.Count;

            foreach (var set in sets.SetsThrough(cell))
            {
                masks.Add(BitboardPosition.MaskOf(sets.CellsOf(set)));
            }
        }

        cellStarts[sets.CellCount] = masks.Count;

        return [.. masks];
    }

    private ulong OpenCells(BitboardPosition position) => _board & ~position.Claimed;

    // The mover claims the cell the choice puts the piece in, and the other player moves next.
    private BitboardPosition Claim(BitboardPosition position, int choice)
    {
        var cell = TPlacement.CellClaimed(OpenCells(position), choice);
        var held = position.Mover | (1UL << cell);

        return new(position.Opponent, held, HasCompletedASet(held, cell));
    }

    private bool HasCompletedASet(ulong held, int cell)
    {
        foreach (var mask in _masksThrough.AsSpan(_cellStarts[cell].._cellStarts[cell + 1]))
        {
            if ((held & mask) == mask)
            {
                return true;
            }
        }

        return false;
    }
}
