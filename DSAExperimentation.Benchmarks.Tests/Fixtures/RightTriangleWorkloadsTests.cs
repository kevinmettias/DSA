using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for RightTriangleWorkloads (ARCHITECTURE 17.7). A square 0/1 grid at a fixed
// one-density is the shape both LC 3128 strategies count triangles over, and the reading depends
// on that density being neither empty nor solid: an all-zero or all-one grid makes the count a
// closed form rather than a search.
public sealed partial class RightTriangleWorkloadsTests
{
    private const int Size = 20;
    private const int Seed = 3128; // LC problem number
    private const int ZeroCell = 0;
    private const int OneCell = 1;

    [Fact]
    public void BuildGrid_Size_ReturnsASquareGridOfThatSide()
    {
        var grid = RightTriangleWorkloads.BuildGrid(Size, Seed);

        Assert.Equal(Size, grid.Length);
        Assert.All(grid, row => Assert.Equal(Size, row.Length));
    }

    [Fact]
    public void BuildGrid_EveryCell_IsEitherZeroOrOne() =>
        Assert.All(
            RightTriangleWorkloads.BuildGrid(Size, Seed).SelectMany(row => row),
            cell => Assert.True(cell == ZeroCell || cell == OneCell));

    [Fact]
    public void BuildGrid_Grid_LeavesBothZeroAndOneCellsForTheCountingToSearch()
    {
        var cells = RightTriangleWorkloads.BuildGrid(Size, Seed).SelectMany(row => row).ToList();

        Assert.Contains(cells, cell => cell == ZeroCell);
        Assert.Contains(cells, cell => cell == OneCell);
    }

    [Fact]
    public void BuildGrid_SameSeed_ReturnsTheSameGrid() =>
        Assert.Equal(
            AnswerText.Of(RightTriangleWorkloads.BuildGrid(Size, Seed)),
            AnswerText.Of(RightTriangleWorkloads.BuildGrid(Size, Seed)));
}
