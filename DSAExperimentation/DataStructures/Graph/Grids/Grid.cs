using System.Collections.Immutable;
using DSAExperimentation.Domain.Mathematics.Lattices;

namespace DSAExperimentation.DataStructures.Graph.Grids;

// The shared, read-only context a GridNode carries a reference to - the same
// pattern every other topology relies on (a node knows how to find its own
// neighbors), just for adjacency computed from geometry instead of stored as an
// explicit list. GetChildren stays a pure function of the node because the node
// carries this reference; GridTopology itself stays a stateless witness like every
// other topology in this library.
//
// The board can also keep one node per cell, made the first time the cell is asked for, so a walk that
// reaches a cell from up to four neighbours allocates it once rather than once per step (NodePerCell).
// The table is allocated on first use, so a board walked with NodePerStep never has one. A node made
// elsewhere for the same cell is equal to it, since GridNode is a record, so a caller that builds its
// own start node still meets the board's nodes in a visited set.
//
// The board also says which steps join a cell to its neighbours: the square lattice's orthogonal
// steps unless it is given others, such as the eight octilinear ones for a problem that moves
// diagonally too. GridChildren lists a cell's neighbours in this table's order.
/// <summary>
/// A rectangular board of open and blocked cells, shared by every <see cref="GridNode"/> on it.
/// </summary>
/// <param name="passable">Which cells are open, indexed [row, col]; its dimensions are the board's.</param>
/// <param name="steps">The steps that join a cell to its neighbours, in the order its neighbours are listed.</param>
public sealed class Grid(bool[,] passable, ImmutableArray<GridStep> steps)
{
    // GridChildren marks which steps lead to an open cell in one bit each of a 64-bit mask.
    private const int MostSteps = 64;

    private const string OffTheBoardMessage = "Expected a cell on the board.";

    private const string TooManyStepsMessage = "A board joins a cell to at most 64 neighbours.";

    private GridNode?[]? _nodes;

    /// <summary>The board's extent: its row and column counts.</summary>
    public GridSize Size { get; } = new(passable.GetLength(0), passable.GetLength(1));

    /// <summary>How many rows the board has.</summary>
    public int Rows => Size.Rows;

    /// <summary>How many columns the board has.</summary>
    public int Cols => Size.Cols;

    /// <summary>The steps that join a cell to its neighbours, in the order its neighbours are listed.</summary>
    public ImmutableArray<GridStep> Steps { get; } = AtMostMostSteps(steps);

    /// <summary>A board whose cells are joined by the four orthogonal steps.</summary>
    /// <param name="passable">Which cells are open, indexed [row, col]; its dimensions are the board's.</param>
    public Grid(bool[,] passable)
        : this(passable, GridDirections.Orthogonal)
    {
    }

    // Every cell open: a board whose only walls are its edges.
    public Grid(int rows, int cols)
        : this(AllOpen(rows, cols))
    {
    }

    /// <summary>Reports whether a cell is on the board and open.</summary>
    /// <param name="row">The cell's row, which may lie off the board.</param>
    /// <param name="col">The cell's column, which may lie off the board.</param>
    /// <returns>True when the cell is on the board and open; false when it is off the board or blocked.</returns>
    public bool IsPassable(int row, int col) => Size.HasCell(row, col) && passable[row, col];

    /// <summary>The board's node for a cell, the same node every time the cell is asked for.</summary>
    /// <param name="row">The cell's row.</param>
    /// <param name="col">The cell's column.</param>
    /// <returns>The cell's node on this board.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The cell is off the board.</exception>
    public GridNode NodeAt(int row, int col)
    {
        if (!Size.HasCell(row, col))
        {
            throw new ArgumentOutOfRangeException(nameof(row), (row, col), OffTheBoardMessage);
        }

        var nodes = _nodes ??= new GridNode?[Rows * Cols];

        return nodes[(row * Cols) + col] ??= new GridNode(row, col, this);
    }

    // The board a filter opens: a flood fill over a problem's own cells - land, water, a cell
    // holding fish - asks its IGridCellFilter about each cell once, here, and walks the fixed map
    // afterwards. The filter is a struct witness, so each rule is its own instantiation and
    // CanEnter a direct call; every cell it is asked about is on the board.
    /// <summary>Builds the board whose open cells are the ones a filter accepts.</summary>
    /// <param name="size">The board's extent.</param>
    /// <param name="filter">The rule a cell must pass to be open, asked once about each cell.</param>
    /// <returns>A board of <paramref name="size"/>, open exactly where <paramref name="filter"/> accepted.</returns>
    public static Grid Where<TFilter>(GridSize size, TFilter filter)
        where TFilter : struct, IGridCellFilter
        => Where(size, filter, GridDirections.Orthogonal);

    /// <summary>Builds the board whose open cells are the ones a filter accepts, joined by the given steps.</summary>
    /// <param name="size">The board's extent.</param>
    /// <param name="filter">The rule a cell must pass to be open, asked once about each cell.</param>
    /// <param name="steps">The steps that join a cell to its neighbours, in the order its neighbours are listed.</param>
    /// <returns>A board of <paramref name="size"/>, open exactly where <paramref name="filter"/> accepted.</returns>
    public static Grid Where<TFilter>(GridSize size, TFilter filter, ImmutableArray<GridStep> steps)
        where TFilter : struct, IGridCellFilter
    {
        var open = new bool[size.Rows, size.Cols];

        for (var row = 0; row < size.Rows; row++)
        {
            for (var col = 0; col < size.Cols; col++)
            {
                open[row, col] = filter.CanEnter(row, col);
            }
        }

        return new Grid(open, steps);
    }

    // Every open cell as a node of this grid, row by row: the roots a whole-board flood fill scans.
    /// <summary>Lists every open cell as a node of this board, row by row and left to right.</summary>
    /// <typeparam name="TNodes">Which node each cell is handed as.</typeparam>
    /// <returns>A node for each open cell, each on this board.</returns>
    public IEnumerable<GridNode> PassableNodes<TNodes>()
        where TNodes : struct, IGridNodeSource
    {
        for (var row = 0; row < Rows; row++)
        {
            for (var col = 0; col < Cols; col++)
            {
                if (IsPassable(row, col))
                {
                    yield return TNodes.NodeAt(this, row, col);
                }
            }
        }
    }

    private static ImmutableArray<GridStep> AtMostMostSteps(ImmutableArray<GridStep> steps)
    {
        if (steps.Length > MostSteps)
        {
            throw new ArgumentOutOfRangeException(nameof(steps), steps.Length, TooManyStepsMessage);
        }

        return steps;
    }

    private static bool[,] AllOpen(int rows, int cols)
    {
        var passable = new bool[rows, cols];

        for (var row = 0; row < rows; row++)
        {
            for (var col = 0; col < cols; col++)
            {
                passable[row, col] = true;
            }
        }

        return passable;
    }
}
