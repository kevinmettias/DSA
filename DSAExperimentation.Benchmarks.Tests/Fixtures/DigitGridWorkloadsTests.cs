using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for DigitGridWorkloads (ARCHITECTURE 17.7): a square grid of digits 1-9,
// which is the constraint both strategies search over - no zero cell can ever start a number,
// so neither arm is measured on a shape the judge would never present it.
public sealed partial class DigitGridWorkloadsTests
{
    private const int Size = 6;
    private const int Seed = 3044; // LC problem number
    private const int MinDigit = 1;
    private const int MaxDigit = 9;

    [Fact]
    public void BuildGrid_Size_ReturnsASquareGridOfThatSide()
    {
        var grid = DigitGridWorkloads.BuildGrid(Size, Seed);

        Assert.Equal(Size, grid.Length);
        Assert.All(grid, row => Assert.Equal(Size, row.Length));
    }

    [Fact]
    public void BuildGrid_EveryCell_IsADigitFromOneToNine()
    {
        var grid = DigitGridWorkloads.BuildGrid(Size, Seed);

        Assert.All(grid, row => Assert.All(row, cell => Assert.InRange(cell, MinDigit, MaxDigit)));
    }

    [Fact]
    public void BuildGrid_SameSeed_ReturnsTheSameGrid() =>
        Assert.Equal(
            AnswerText.Of(DigitGridWorkloads.BuildGrid(Size, Seed)),
            AnswerText.Of(DigitGridWorkloads.BuildGrid(Size, Seed)));
}
