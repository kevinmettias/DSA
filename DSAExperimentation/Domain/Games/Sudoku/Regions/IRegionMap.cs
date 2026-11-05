namespace DSAExperimentation.Domain.Games.Sudoku.Regions;

// Regions drawn as a puzzle likes, as jigsaw Sudoku draws them: a table giving each cell's region,
// row by row. The table is how a puzzle is written down, so a grid states it and RegionMap reads it
// once into each region's list of cells. It is a witness of its own, so a grid of boxes carries no
// table and pays nothing for it. A jigsaw puzzle draws each region as one connected shape, but the
// rule never reads that, so the map does not require it; it does require n regions of n cells, and
// refuses a table that is not one. A byte numbers a region, so the order is at most 256.
/// <summary>A partition given as a table: the region of each cell, row by row.</summary>
/// <typeparam name="TSelf">The grid itself.</typeparam>
public interface IRegionMap<TSelf> : IRegionPartition
    where TSelf : struct, IRegionMap<TSelf>
{
    /// <summary>Each cell's region, row by row: n x n entries, each from 0 to n - 1.</summary>
    static abstract ReadOnlySpan<byte> RegionOfCell { get; }

    /// <inheritdoc/>
    static int IRegionPartition.RegionOf(int row, int col) => RegionMap<TSelf>.RegionOf(row, col);

    /// <inheritdoc/>
    static (int Row, int Col) IRegionPartition.CellOf(int region, int index) =>
        RegionMap<TSelf>.CellOf(region, index);

    /// <inheritdoc/>
    static bool IRegionPartition.IsInRegion(char[][] board, int row, int col, char symbol) =>
        RegionMap<TSelf>.IsInRegion(board, row, col, symbol);
}
