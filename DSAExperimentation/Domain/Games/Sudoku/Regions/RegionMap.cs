namespace DSAExperimentation.Domain.Games.Sudoku.Regions;

// A region map's two tables, built once per grid: each cell's region, checked to be a partition
// into n regions of n cells, and each region's cells, region r's n cells at [r x n, r x n + n), row
// by row. A table that is not a partition fails the grid's first use, with the region it got wrong.
/// <summary>A region map's lookups: a cell's region, and a region's cells and symbols.</summary>
/// <typeparam name="TMap">The grid whose regions these are.</typeparam>
internal static class RegionMap<TMap>
    where TMap : struct, IRegionMap<TMap>
{
    private static readonly byte[] Regions = CheckedPartition(TMap.RegionOfCell);

    private static readonly (int Row, int Col)[] CellsByRegion = ListCellsByRegion(Regions);

    /// <summary>Reads the region a cell lies in from the table.</summary>
    /// <param name="row">The cell's row, from 0.</param>
    /// <param name="col">The cell's column, from 0.</param>
    /// <returns>The region's number.</returns>
    public static int RegionOf(int row, int col) => Regions[(row * TMap.Order) + col];

    /// <summary>Finds one cell of a region, its cells counted row by row.</summary>
    /// <param name="region">The region's number.</param>
    /// <param name="index">Which of the region's cells, from 0.</param>
    /// <returns>The cell's row and column.</returns>
    public static (int Row, int Col) CellOf(int region, int index) =>
        CellsByRegion[(region * TMap.Order) + index];

    /// <summary>Whether the region around a cell already holds a symbol.</summary>
    /// <param name="board">The board, one array of characters per row.</param>
    /// <param name="row">The cell's row, from 0.</param>
    /// <param name="col">The cell's column, from 0.</param>
    /// <param name="symbol">The symbol to look for.</param>
    /// <returns><see langword="true"/> when some cell of the region holds the symbol.</returns>
    public static bool IsInRegion(char[][] board, int row, int col, char symbol)
    {
        var first = RegionOf(row, col) * TMap.Order;

        for (var i = first; i < first + TMap.Order; i++)
        {
            var (r, c) = CellsByRegion[i];

            if (board[r][c] == symbol)
            {
                return true;
            }
        }

        return false;
    }

    // The table, copied once it is known to give every one of the n x n cells a region below n and
    // every region n cells.
    private static byte[] CheckedPartition(ReadOnlySpan<byte> regionOfCell)
    {
        var cells = TMap.Order * TMap.Order;

        if (regionOfCell.Length != cells)
        {
            throw new InvalidOperationException(
                $"{typeof(TMap).Name} maps {regionOfCell.Length} cells, not {cells}.");
        }

        var sizes = RegionSizes(regionOfCell);
        var wrongSize = Array.FindIndex(sizes, size => size != TMap.Order);

        if (wrongSize >= 0)
        {
            throw new InvalidOperationException(
                $"{typeof(TMap).Name}'s region {wrongSize} has {sizes[wrongSize]} cells.");
        }

        return regionOfCell.ToArray();
    }

    // How many cells the table gives each region, refusing a region past the last.
    private static int[] RegionSizes(ReadOnlySpan<byte> regionOfCell)
    {
        var sizes = new int[TMap.Order];

        foreach (var region in regionOfCell)
        {
            if (region >= TMap.Order)
            {
                throw new InvalidOperationException(
                    $"{typeof(TMap).Name} names region {region}; its last is {TMap.Order - 1}.");
            }

            sizes[region]++;
        }

        return sizes;
    }

    // Each region's cells, row by row: a cell goes into its region's next free place.
    private static (int Row, int Col)[] ListCellsByRegion(byte[] regions)
    {
        var order = TMap.Order;
        var cells = new (int Row, int Col)[regions.Length];
        Span<int> placed = stackalloc int[order];

        for (var cell = 0; cell < regions.Length; cell++)
        {
            var region = regions[cell];
            cells[(region * order) + placed[region]] = (cell / order, cell % order);
            placed[region]++;
        }

        return cells;
    }
}
