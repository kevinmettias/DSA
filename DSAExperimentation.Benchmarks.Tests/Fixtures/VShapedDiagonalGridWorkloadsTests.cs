using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for VShapedDiagonalGridWorkloads (ARCHITECTURE 17.7): LC 3459 walks a square grid whose
// cells are 0, 1 or 2 - 0 is a valid mid-segment value there, not a blocked cell - and both strategies need
// 1-starts with 2/0 follow-ups to extend. The draw is one call per cell over the three values, so what is
// asserted is membership in {0, 1, 2} at the requested shape rather than any weighting among them.
public sealed partial class VShapedDiagonalGridWorkloadsTests
{
    private const int Size = 32;
    private const int Seed = 3459; // LC problem number
    private const int SmallestCellValue = 0;
    private const int LargestCellValue = 2;
    private const int FewestDistinctValues = 2;

    [Fact]
    public void BuildGrid_Size_ReturnsASquareGridOfThatShape()
    {
        var grid = VShapedDiagonalGridWorkloads.BuildGrid(Size, Seed);

        Assert.Equal(Size, grid.Length);
        Assert.All(grid, row => Assert.Equal(Size, row.Length));
    }

    [Fact]
    public void BuildGrid_EveryCell_IsOneOfTheThreeDocumentedValues() =>
        Assert.All(
            VShapedDiagonalGridWorkloads.BuildGrid(Size, Seed).SelectMany(row => row),
            cell => Assert.InRange(cell, SmallestCellValue, LargestCellValue));

    // A grid of one repeated value would leave the diagonal walk nothing to extend along, so the load has
    // to contain more than one of the three values to be a real walk.
    [Fact]
    public void BuildGrid_DrawnPerCell_LeavesMoreThanOneValueToWalk() =>
        Assert.True(
            VShapedDiagonalGridWorkloads.BuildGrid(Size, Seed).SelectMany(row => row).Distinct().Count()
                >= FewestDistinctValues);

    [Fact]
    public void BuildGrid_SameSeed_ReturnsTheSameGrid() =>
        Assert.Equal(
            AnswerText.Of(VShapedDiagonalGridWorkloads.BuildGrid(Size, Seed)),
            AnswerText.Of(VShapedDiagonalGridWorkloads.BuildGrid(Size, Seed)));
}
