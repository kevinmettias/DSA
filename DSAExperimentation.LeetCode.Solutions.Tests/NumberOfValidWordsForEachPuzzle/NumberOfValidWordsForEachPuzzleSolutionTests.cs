using DSAExperimentation.LeetCode.Solutions.NumberOfValidWordsForEachPuzzle;

namespace DSAExperimentation.LeetCode.Solutions.Tests.NumberOfValidWordsForEachPuzzle;

// Harness only. Both strategies are NumberOfValidWordsForEachPuzzleSolution's -
// this file just pins them to the degenerate
// cases the mask encoding has to get right (a puzzle matching nothing, and a word
// whose letters are all present but which still misses the first letter).
public sealed partial class NumberOfValidWordsForEachPuzzleSolutionTests
{
    public static TheoryData<string[], string[], int[]> Cases =>
        new()
        {
            {
                ["bc"],
                ["abcdefg"],
                [0]
            },
            {
                ["a"],
                ["abcdefg"],
                [1]
            },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountValidWordsByMaskComparison_Cases_ReturnsValidWordCountPerPuzzle(
        string[] words, string[] puzzles, int[] expected)
    {
        var actual = NumberOfValidWordsForEachPuzzleSolution.CountValidWordsByMaskComparison(words, puzzles);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountValidWordsByMaskSubsetEnumeration_Cases_ReturnsValidWordCountPerPuzzle(
        string[] words, string[] puzzles, int[] expected)
    {
        var actual = NumberOfValidWordsForEachPuzzleSolution.CountValidWordsByMaskSubsetEnumeration(words, puzzles);

        Assert.Equal(expected, actual);
    }
}
