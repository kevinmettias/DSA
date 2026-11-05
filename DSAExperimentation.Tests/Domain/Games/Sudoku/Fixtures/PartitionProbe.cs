using DSAExperimentation.Domain.Games.Sudoku.Regions;

namespace DSAExperimentation.Tests.Domain.Games.Sudoku.Fixtures;

// What a partition into n regions of n cells must satisfy, read through the contract alone, so the
// box grids and the jigsaw are held to the same facts: the regions' cell lists cover each cell once
// and agree with RegionOf, RegionOf gives n regions of n cells, and IsInRegion finds a symbol from
// exactly the cells RegionOf puts in its region.
internal static class PartitionProbe
{
    private const char Symbol = 'x';

    // The facts for one grid.
    public static PartitionFacts Of<TGrid>()
        where TGrid : struct, IRegionPartition =>
        new(
            CoverCounts<TGrid>(),
            CellsListedOutsideTheirRegion<TGrid>(),
            RegionSizes<TGrid>(),
            IsInRegionDisagreements<TGrid>());

    // How many times each cell, row by row, appears among all the regions' cells.
    private static int[] CoverCounts<TGrid>()
        where TGrid : struct, IRegionPartition
    {
        var counts = new int[TGrid.Order * TGrid.Order];

        foreach (var (row, col, _) in ListedCells<TGrid>())
        {
            counts[(row * TGrid.Order) + col]++;
        }

        return counts;
    }

    // Every region's cells, as listed by CellOf, each with the region listing it.
    private static IEnumerable<(int Row, int Col, int Region)> ListedCells<TGrid>()
        where TGrid : struct, IRegionPartition =>
        Enumerable.Range(0, TGrid.Order).SelectMany(region => Enumerable.Range(0, TGrid.Order)
            .Select(index => TGrid.CellOf(region, index))
            .Select(cell => (cell.Row, cell.Col, region)));

    // The listed cells whose RegionOf is not the region that lists them.
    private static (int Row, int Col, int Region)[] CellsListedOutsideTheirRegion<TGrid>()
        where TGrid : struct, IRegionPartition =>
        [.. ListedCells<TGrid>().Where(cell => TGrid.RegionOf(cell.Row, cell.Col) != cell.Region)];

    // How many cells RegionOf puts in each region, the regions numbered from 0.
    private static int[] RegionSizes<TGrid>()
        where TGrid : struct, IRegionPartition
    {
        var sizes = new int[TGrid.Order];

        for (var row = 0; row < TGrid.Order; row++)
        {
            for (var col = 0; col < TGrid.Order; col++)
            {
                sizes[TGrid.RegionOf(row, col)]++;
            }
        }

        return sizes;
    }

    // The pairs of cells, the first holding a symbol on an otherwise empty board and the second
    // asking IsInRegion, on which IsInRegion's answer differs from whether RegionOf puts the two in
    // one region.
    private static (int Placed, int Asked)[] IsInRegionDisagreements<TGrid>()
        where TGrid : struct, IRegionPartition
    {
        var cells = TGrid.Order * TGrid.Order;
        var disagreements = new List<(int Placed, int Asked)>();

        for (var placed = 0; placed < cells; placed++)
        {
            var board = BoardWithOneSymbol<TGrid>(placed);
            var askers = Enumerable.Range(0, cells)
                .Where(asked => IsInRegionDisagrees<TGrid>(board, placed, asked));
            disagreements.AddRange(askers.Select(asked => (placed, asked)));
        }

        return [.. disagreements];
    }

    private static char[][] BoardWithOneSymbol<TGrid>(int cell)
        where TGrid : struct, IRegionPartition
    {
        var board = Enumerable.Range(0, TGrid.Order).Select(_ => new char[TGrid.Order]).ToArray();
        board[cell / TGrid.Order][cell % TGrid.Order] = Symbol;

        return board;
    }

    private static bool IsInRegionDisagrees<TGrid>(char[][] board, int placed, int asked)
        where TGrid : struct, IRegionPartition
    {
        var (placedRow, placedCol) = (placed / TGrid.Order, placed % TGrid.Order);
        var (askedRow, askedCol) = (asked / TGrid.Order, asked % TGrid.Order);
        var placedRegion = TGrid.RegionOf(placedRow, placedCol);
        var isSameRegion = placedRegion == TGrid.RegionOf(askedRow, askedCol);

        return TGrid.IsInRegion(board, askedRow, askedCol, Symbol) != isSameRegion;
    }
}
