using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for RottingOrangesWorkloads (ARCHITECTURE 17.7). The LC 994 reading depends on
// the grid holding a majority of fresh oranges for the rot to spread through - that reachability is
// what makes the baseline pay for a BFS per cell - and on the top-left corner being rotten, since
// without it the sweep has no source and every arm answers immediately.
public sealed partial class RottingOrangesWorkloadsTests
{
    private const int Size = 10;
    private const int Seed = 994; // LC problem number
    private const int EmptyCellState = 0;
    private const int FreshOrangeState = 1;
    private const int RottenOrangeState = 2;
    private const int FirstRow = 0;
    private const int FirstColumn = 0;
    private const int FreshMajorityDivisor = 2;

    [Fact]
    public void BuildGrid_Size_ReturnsASquareGridOfThatSide()
    {
        var grid = RottingOrangesWorkloads.BuildGrid(Size, Seed);

        Assert.Equal(Size, grid.Length);
        Assert.All(grid, row => Assert.Equal(Size, row.Length));
    }

    [Fact]
    public void BuildGrid_EveryCell_IsOneOfTheThreeDocumentedStates() =>
        Assert.All(
            RottingOrangesWorkloads.BuildGrid(Size, Seed).SelectMany(row => row),
            cell => Assert.True(
                cell == EmptyCellState || cell == FreshOrangeState || cell == RottenOrangeState));

    [Fact]
    public void BuildGrid_TopLeftCell_IsAlwaysRottenSoTheSweepHasASource() =>
        Assert.Equal(
            RottenOrangeState,
            RottingOrangesWorkloads.BuildGrid(Size, Seed)[FirstRow][FirstColumn]);

    [Fact]
    public void BuildGrid_Grid_KeepsFreshOrangesInTheMajorityForTheRotToReach()
    {
        var cells = RottingOrangesWorkloads.BuildGrid(Size, Seed).SelectMany(row => row).ToList();

        Assert.True(cells.Count(cell => cell == FreshOrangeState) > cells.Count / FreshMajorityDivisor);
    }

    [Fact]
    public void BuildGrid_SameSeed_ReturnsTheSameGrid() =>
        Assert.Equal(
            AnswerText.Of(RottingOrangesWorkloads.BuildGrid(Size, Seed)),
            AnswerText.Of(RottingOrangesWorkloads.BuildGrid(Size, Seed)));
}
