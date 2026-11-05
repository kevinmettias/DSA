using DSAExperimentation.Domain.Mathematics.Combinatorics.LatinSquares;

namespace DSAExperimentation.Domain.Games.Sudoku.Regions;

// Regions that are boxes: rectangles BoxRows tall and BoxColumns wide, tiling a board whose side is
// their product. Classic Sudoku's boxes are 3x3, a 6x6 board's 2x3 and a 16x16 board's 4x4. The
// board's side follows from the box, so a grid states only the box. The two sides are static
// abstract members of a struct, so BoxPartition's divisions fold to constants for each grid, and
// the members IRegionPartition asks for are answered here from them, so a grid need not repeat
// them.
/// <summary>A partition into boxes, each <c>BoxRows</c> x <c>BoxColumns</c> cells.</summary>
/// <typeparam name="TSelf">The grid itself.</typeparam>
public interface IBoxPartition<TSelf> : IRegionPartition
    where TSelf : struct, IBoxPartition<TSelf>
{
    /// <summary>How many rows a box spans.</summary>
    static abstract int BoxRows { get; }

    /// <summary>How many columns a box spans.</summary>
    static abstract int BoxColumns { get; }

    /// <summary>The board's side, a box's rows times its columns.</summary>
    static int ILatinSquare.Order => TSelf.BoxRows * TSelf.BoxColumns;

    /// <inheritdoc/>
    static int IRegionPartition.RegionOf(int row, int col) =>
        BoxPartition<TSelf>.RegionOf(row, col);

    /// <inheritdoc/>
    static (int Row, int Col) IRegionPartition.CellOf(int region, int index) =>
        BoxPartition<TSelf>.CellOf(region, index);

    /// <inheritdoc/>
    static bool IRegionPartition.IsInRegion(char[][] board, int row, int col, char symbol) =>
        BoxPartition<TSelf>.IsInRegion(board, row, col, symbol);
}
