using DSAExperimentation.Domain.Games.Sudoku;
using DSAExperimentation.Domain.Games.Sudoku.Regions;
using DSAExperimentation.Tests.Domain.Games.Sudoku.Fixtures;

namespace DSAExperimentation.Tests.Domain.Games.Sudoku.Regions;

// The 6x6 grid's boxes are two rows tall and three columns wide, the one preset whose box is not
// square, so it is the grid that tells a box's rows from its columns. Every grid of boxes is held
// to the partition's facts through the contract (PartitionProbe).
public sealed partial class BoxPartitionTests
{
    public static TheoryData<string> BoxGrids => new()
    {
        nameof(ClassicSudoku),
        nameof(SixBySixSudoku),
        nameof(SixteenBySixteenSudoku),
        nameof(FourByFourGrid),
    };

    // Two boxes to a band, three bands: the corners of each box.
    public static TheoryData<int, int, int> SixBySixCellsToBoxNumbers => new()
    {
        { 0, 0, 0 }, { 1, 2, 0 }, { 0, 3, 1 }, { 1, 5, 1 },
        { 2, 0, 2 }, { 3, 4, 3 }, { 4, 2, 4 }, { 5, 5, 5 },
    };

    [Theory]
    [MemberData(nameof(SixBySixCellsToBoxNumbers))]
    public void RegionOf_SixBySix_NumbersTwoByThreeBoxesRowByRow(int row, int col, int expected) =>
        Assert.Equal(expected, BoxPartition<SixBySixSudoku>.RegionOf(row, col));

    // Box 3 is the second box of the second band: rows 2 and 3, columns 3 to 5.
    [Fact]
    public void CellOf_SixBySix_ListsABoxsCellsRowByRow() =>
        Assert.Equal(
            [(2, 3), (2, 4), (2, 5), (3, 3), (3, 4), (3, 5)],
            Enumerable.Range(0, Sudoku<SixBySixSudoku>.Size)
                .Select(index => BoxPartition<SixBySixSudoku>.CellOf(3, index)));

    [Theory]
    [MemberData(nameof(BoxGrids))]
    public void CellOf_AllBoxesTogether_ListEveryCellOnce(string grid) =>
        Assert.All(Probe(grid).CoverCounts, count => Assert.Equal(1, count));

    [Theory]
    [MemberData(nameof(BoxGrids))]
    public void CellOf_EveryListedCell_LiesInTheBoxListingIt(string grid) =>
        Assert.Empty(Probe(grid).CellsListedOutsideTheirRegion);

    [Theory]
    [MemberData(nameof(BoxGrids))]
    public void RegionOf_EveryGrid_GivesNBoxesOfNCells(string grid)
    {
        var sizes = Probe(grid).RegionSizes;

        Assert.All(sizes, size => Assert.Equal(sizes.Length, size));
    }

    [Theory]
    [MemberData(nameof(BoxGrids))]
    public void IsInRegion_EveryGrid_FindsASymbolFromItsOwnBoxOnly(string grid) =>
        Assert.Empty(Probe(grid).IsInRegionDisagreements);

    private static PartitionFacts Probe(string grid) => grid switch
    {
        nameof(ClassicSudoku) => PartitionProbe.Of<ClassicSudoku>(),
        nameof(SixBySixSudoku) => PartitionProbe.Of<SixBySixSudoku>(),
        nameof(SixteenBySixteenSudoku) => PartitionProbe.Of<SixteenBySixteenSudoku>(),
        _ => PartitionProbe.Of<FourByFourGrid>(),
    };
}
