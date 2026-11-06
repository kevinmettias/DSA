namespace DSAExperimentation.Domain.Games.PositionalGames;

// Which cells of a board are there to play on. Cells are numbered from 0, and a hole is a numbered
// cell that is not part of the board: nobody can claim it and no winning set may use it. A board
// drawn on a grid of any other shape (a cross, a ring, a diamond) is its bounding rectangle less
// some holes, so the mask is how a board takes another shape. A position records each cell's owner:
// a player's seat, Unclaimed for an open cell, or Hole for a cell off the board.
/// <summary>
/// A board as numbered cells less its holes, and the position nobody has played on yet.
/// </summary>
public sealed class BoardMask
{
    /// <summary>What a position records for a cell nobody has claimed.</summary>
    public const int Unclaimed = -1;

    /// <summary>What a position records for a cell that is not on the board.</summary>
    public const int Hole = -2;

    private readonly bool[] _isHole;

    /// <summary>Gets how many cells are numbered, holes included.</summary>
    public int CellCount => _isHole.Length;

    /// <summary>Gets how many cells are on the board: the numbered cells less the holes.</summary>
    public int PlayableCount { get; }

    /// <summary>Initializes a board of numbered cells with some of them left out.</summary>
    /// <param name="cellCount">How many cells are numbered, holes included.</param>
    /// <param name="holes">
    /// The cells that are not on the board; a cell named twice is one hole.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// A hole is not one of the numbered cells.
    /// </exception>
    public BoardMask(int cellCount, params ReadOnlySpan<int> holes)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(cellCount);
        _isHole = new bool[cellCount];

        foreach (var hole in holes)
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(
                (uint)hole, (uint)cellCount, nameof(holes));
            _isHole[hole] = true;
        }

        PlayableCount = _isHole.Count(isHole => !isHole);
    }

    /// <summary>Makes a board with no holes.</summary>
    /// <param name="cellCount">How many cells it has.</param>
    /// <returns>The board.</returns>
    public static BoardMask Full(int cellCount) => new(cellCount);

    /// <summary>Whether a numbered cell is on the board.</summary>
    /// <param name="cell">The cell's number.</param>
    /// <returns><see langword="true"/> when the cell is numbered and not a hole.</returns>
    public bool HasCell(int cell) => (uint)cell < (uint)_isHole.Length && !_isHole[cell];

    /// <summary>
    /// Makes the position before anyone has moved: every cell open, the holes marked.
    /// </summary>
    /// <returns>
    /// A new array of each cell's owner, <see cref="Unclaimed"/> or <see cref="Hole"/>.
    /// </returns>
    public int[] EmptyPosition()
    {
        var owners = new int[_isHole.Length];

        for (var cell = 0; cell < owners.Length; cell++)
        {
            var isHole = _isHole[cell];
            owners[cell] = isHole ? Hole : Unclaimed;
        }

        return owners;
    }
}
