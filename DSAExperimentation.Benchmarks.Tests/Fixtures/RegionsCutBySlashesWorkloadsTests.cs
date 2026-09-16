using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for RegionsCutBySlashesWorkloads (ARCHITECTURE 17.7). LC 959's grid is drawn
// from the three characters the problem allows, and that is what the reading rests on: a draw
// over all three keeps roughly two thirds of the cells carrying a cut, so the region count stays
// away from both degenerate ends - one region, or one region per triangle.
public sealed partial class RegionsCutBySlashesWorkloadsTests
{
    private const int GridSize = 30;
    private const int Seed = 959; // LC problem number
    private const char EmptyCell = ' ';
    private const char ForwardSlash = '/';
    private const char Backslash = '\\';

    [Fact]
    public void BuildGrid_GridSize_ReturnsASquareGridOfThatSide()
    {
        var grid = RegionsCutBySlashesWorkloads.BuildGrid(GridSize, Seed);

        Assert.Equal(GridSize, grid.Length);
        Assert.All(grid, row => Assert.Equal(GridSize, row.Length));
    }

    [Fact]
    public void BuildGrid_EveryCell_IsOneOfTheThreeCharactersTheProblemAllows() =>
        Assert.All(
            RegionsCutBySlashesWorkloads.BuildGrid(GridSize, Seed).SelectMany(row => row),
            cell => Assert.True(cell == EmptyCell || cell == ForwardSlash || cell == Backslash));

    // A grid that drew only one of the three characters would sit on a degenerate end - every
    // cell cut, or none - where the region count is a closed form rather than a traversal.
    [Fact]
    public void BuildGrid_Grid_LeavesBothCutAndUncutCellsForTheTraversalToSeparate()
    {
        var cells = RegionsCutBySlashesWorkloads.BuildGrid(GridSize, Seed).SelectMany(row => row).ToList();

        Assert.Contains(cells, cell => cell == EmptyCell);
        Assert.Contains(cells, cell => cell == ForwardSlash || cell == Backslash);
    }

    [Fact]
    public void BuildGrid_SameSeed_ReturnsTheSameGrid() =>
        Assert.Equal(
            AnswerText.Of(RegionsCutBySlashesWorkloads.BuildGrid(GridSize, Seed)),
            AnswerText.Of(RegionsCutBySlashesWorkloads.BuildGrid(GridSize, Seed)));
}
