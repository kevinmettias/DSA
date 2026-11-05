using DSAExperimentation.Domain.Games.Sudoku.Regions;
using DSAExperimentation.Tests.Domain.Games.Sudoku.Fixtures;

namespace DSAExperimentation.Tests.Domain.Games.Sudoku.Regions;

// The 5x5 jigsaw's map is drawn in PentominoJigsaw. A map that is not a partition is refused when
// the grid is first used: one region too big (and so another too small), a region past the last, a
// table of the wrong length.
public sealed partial class RegionMapTests
{
    public static TheoryData<int, int, int> JigsawCellsToRegions => new()
    {
        { 0, 0, 0 }, { 1, 2, 0 }, { 1, 1, 2 }, { 2, 4, 1 }, { 3, 2, 3 }, { 3, 0, 4 }, { 4, 4, 3 },
    };

    public static TheoryData<string> MapsThatAreNotPartitions => new()
    {
        nameof(OversizedRegion),
        nameof(RegionPastTheLast),
        nameof(ShortTable),
    };

    [Theory]
    [MemberData(nameof(JigsawCellsToRegions))]
    public void RegionOf_Jigsaw_ReadsTheMapRowByRow(int row, int col, int expected) =>
        Assert.Equal(expected, RegionMap<PentominoJigsaw>.RegionOf(row, col));

    // Region 0 is the top row's first three cells and the two below its ends.
    [Fact]
    public void CellOf_Jigsaw_ListsARegionsCellsRowByRow() =>
        Assert.Equal(
            [(0, 0), (0, 1), (0, 2), (1, 0), (1, 2)],
            Enumerable.Range(0, PentominoJigsaw.Order)
                .Select(index => RegionMap<PentominoJigsaw>.CellOf(0, index)));

    [Fact]
    public void CellOf_AllRegionsTogether_ListEveryCellOnce() =>
        Assert.All(
            PartitionProbe.Of<PentominoJigsaw>().CoverCounts, count => Assert.Equal(1, count));

    [Fact]
    public void CellOf_EveryListedCell_LiesInTheRegionListingIt() =>
        Assert.Empty(PartitionProbe.Of<PentominoJigsaw>().CellsListedOutsideTheirRegion);

    [Fact]
    public void RegionOf_Jigsaw_GivesFiveRegionsOfFiveCells() =>
        Assert.Equal([5, 5, 5, 5, 5], PartitionProbe.Of<PentominoJigsaw>().RegionSizes);

    [Fact]
    public void IsInRegion_Jigsaw_FindsASymbolFromItsOwnRegionOnly() =>
        Assert.Empty(PartitionProbe.Of<PentominoJigsaw>().IsInRegionDisagreements);

    [Theory]
    [MemberData(nameof(MapsThatAreNotPartitions))]
    public void RegionOf_MapNotAPartition_Throws(string map)
    {
        var thrown = Assert.Throws<TypeInitializationException>(() => RegionOfFirstCell(map));

        Assert.IsType<InvalidOperationException>(thrown.InnerException);
    }

    private static int RegionOfFirstCell(string map) => map switch
    {
        nameof(OversizedRegion) => RegionMap<OversizedRegion>.RegionOf(0, 0),
        nameof(RegionPastTheLast) => RegionMap<RegionPastTheLast>.RegionOf(0, 0),
        _ => RegionMap<ShortTable>.RegionOf(0, 0),
    };

    // Region 0 holds three of a 2x2 board's cells and region 1 one.
    private readonly struct OversizedRegion : IRegionMap<OversizedRegion>
    {
        public static int Order => 2;

        public static ReadOnlySpan<byte> RegionOfCell => [0, 0, 0, 1];
    }

    // A 2x2 board's regions are 0 and 1, so a region 2 is past the last.
    private readonly struct RegionPastTheLast : IRegionMap<RegionPastTheLast>
    {
        public static int Order => 2;

        public static ReadOnlySpan<byte> RegionOfCell => [0, 0, 1, 2];
    }

    // Three entries for a 2x2 board's four cells.
    private readonly struct ShortTable : IRegionMap<ShortTable>
    {
        public static int Order => 2;

        public static ReadOnlySpan<byte> RegionOfCell => [0, 0, 1];
    }
}
