using System.Collections.Immutable;
using DSAExperimentation.Domain.Mathematics.Lattices;

namespace DSAExperimentation.Domain.Mathematics.Polyominoes;

// The cells of a polyomino: squares of the square lattice, joined edge to edge, so a square is
// joined to the four cells one orthogonal step away (GridDirections' Orthogonal, one orbit of the
// lattice's symmetries). Cells are listed in reading order, row by row and then column by column,
// the order GridDirections' row-major tables are in; comparing rows first and columns second is
// kept by every translation.
/// <summary>Squares joined edge to edge, listed row by row: the cells of a polyomino.</summary>
public readonly struct SquareCells : IPolyformCells<GridStep>
{
    /// <inheritdoc/>
    public static ImmutableArray<GridStep> JoiningSteps => GridDirections.Orthogonal;

    /// <inheritdoc/>
    public static int Compare(GridStep left, GridStep right)
    {
        var byRow = left.DeltaRow.CompareTo(right.DeltaRow);

        if (byRow != 0)
        {
            return byRow;
        }

        return left.DeltaCol.CompareTo(right.DeltaCol);
    }

    /// <summary>Lists a rectangle's squares row by row, the top left one at the origin.</summary>
    /// <param name="rows">How many rows tall the rectangle is.</param>
    /// <param name="cols">How many columns wide it is.</param>
    /// <returns>The rows times cols squares, in reading order.</returns>
    /// <exception cref="ArgumentOutOfRangeException">A side is negative.</exception>
    public static ImmutableArray<GridStep> Rectangle(int rows, int cols)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(rows);
        ArgumentOutOfRangeException.ThrowIfNegative(cols);
        var squares = ImmutableArray.CreateBuilder<GridStep>(rows * cols);

        for (var row = 0; row < rows; row++)
        {
            for (var col = 0; col < cols; col++)
            {
                squares.Add(new GridStep(row, col));
            }
        }

        return squares.MoveToImmutable();
    }
}
