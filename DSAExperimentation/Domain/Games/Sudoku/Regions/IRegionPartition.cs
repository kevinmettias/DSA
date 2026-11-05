using DSAExperimentation.Domain.Mathematics.Combinatorics.LatinSquares;

namespace DSAExperimentation.Domain.Games.Sudoku.Regions;

// Sudoku is a Latin square whose cells are split into n regions of n cells, each region holding
// every symbol once. Classic Sudoku's regions are its 3x3 boxes; a box may be any rectangle whose
// two sides multiply to n (IBoxPartition), and a jigsaw puzzle draws its regions as it likes
// (IRegionMap). A region is numbered from 0 and its cells are listed row by row. IsInRegion is a
// member of its own rather than a walk over CellOf, because a box is a rectangle its two loops
// cover with constant bounds, which a walk over a list of cells would turn into a division per
// cell.
/// <summary>A partition of a Latin square's cells into n regions of n cells.</summary>
public interface IRegionPartition : ILatinSquare
{
    /// <summary>Numbers the region a cell lies in.</summary>
    /// <param name="row">The cell's row, from 0.</param>
    /// <param name="col">The cell's column, from 0.</param>
    /// <returns>The region's number, from 0 to n - 1.</returns>
    static abstract int RegionOf(int row, int col);

    /// <summary>Finds one cell of a region, its cells counted row by row.</summary>
    /// <param name="region">The region's number, from 0 to n - 1.</param>
    /// <param name="index">Which of the region's cells, from 0 to n - 1.</param>
    /// <returns>The cell's row and column.</returns>
    static abstract (int Row, int Col) CellOf(int region, int index);

    /// <summary>Whether the region a cell lies in already holds a symbol.</summary>
    /// <param name="board">The board, one array of n characters per row.</param>
    /// <param name="row">The cell's row, from 0.</param>
    /// <param name="col">The cell's column, from 0.</param>
    /// <param name="symbol">The symbol to look for.</param>
    /// <returns><see langword="true"/> when some cell of the region holds the symbol.</returns>
    static abstract bool IsInRegion(char[][] board, int row, int col, char symbol);
}
