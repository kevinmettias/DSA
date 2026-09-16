using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for OverlappingSubstringGridWorkloads (ARCHITECTURE 17.7). The reading depends on LC
// 3529's grid being square and drawn from the low-cardinality alphabet 'a'/'b', so a naive scan spends
// real comparison work on near-misses instead of rejecting most starting positions after one character.
public sealed partial class OverlappingSubstringGridWorkloadsTests
{
    private const int GridSize = 60;
    private const int Seed = 3529;
    private const int PatternLength = 6;
    private const string Alphabet = "ab";

    [Fact]
    public void Build_GridSize_ReturnsASquareGridAndAFixedLengthPattern()
    {
        var (grid, pattern) = OverlappingSubstringGridWorkloads.Build(GridSize, Seed);

        Assert.Equal(GridSize, grid.Length);
        Assert.All(grid, row => Assert.Equal(GridSize, row.Length));
        Assert.Equal(PatternLength, pattern.Length);
    }

    [Fact]
    public void Build_EveryCellAndPatternLetter_ComesFromTheDocumentedTwoLetterAlphabet()
    {
        var (grid, pattern) = OverlappingSubstringGridWorkloads.Build(GridSize, Seed);

        Assert.All(grid.SelectMany(row => row), cell => Assert.Contains(cell, Alphabet));
        Assert.All(pattern, letter => Assert.Contains(letter, Alphabet));
    }

    // The alphabet is the whole point of this workload: a single-letter grid would let a scan decide on
    // its first character every time, so both letters have to be present for near-misses to be compared.
    [Fact]
    public void Build_Grid_LeavesBothLettersPresentSoNearMissesHaveToBeCompared()
    {
        var (grid, pattern) = OverlappingSubstringGridWorkloads.Build(GridSize, Seed);

        Assert.All(Alphabet, letter => Assert.Contains(letter, grid.SelectMany(row => row)));
        Assert.All(Alphabet, letter => Assert.Contains(letter, pattern));
    }

    [Fact]
    public void Build_SameSeed_ReturnsTheSameGridAndPattern()
    {
        var (grid, pattern) = OverlappingSubstringGridWorkloads.Build(GridSize, Seed);
        var (repeatGrid, repeatPattern) = OverlappingSubstringGridWorkloads.Build(GridSize, Seed);

        Assert.Equal(AnswerText.Of(grid), AnswerText.Of(repeatGrid));
        Assert.Equal(pattern, repeatPattern);
    }
}
