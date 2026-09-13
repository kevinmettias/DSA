using DSAExperimentation.LeetCode.NumberOfValidWordsForEachPuzzle;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfValidWordsForEachPuzzle;

// Harness only. Both strategies are NumberOfValidWordsForEachPuzzleSolution's -
// this file just pins them to LeetCode's published examples, plus the degenerate
// cases the mask encoding has to get right (a puzzle matching nothing, and a word
// whose letters are all present but which still misses the first letter).
public sealed class NumberOfValidWordsForEachPuzzleTests
{
    public static TheoryData<string[], string[], int[]> Examples =>
        new()
        {
            {
                ["aaaa", "asas", "able", "ability", "actt", "actor", "access"],
                ["aboveyz", "abrodyz", "abslute", "absoryz", "actresz", "gaswxyz"],
                [1, 1, 3, 2, 4, 0]
            },
            {
                ["apple", "pleas", "please"],
                ["aelwxyz", "aelpxyz", "aelpsxy", "saelpxy", "xaelpsy"],
                [0, 1, 3, 2, 0]
            },
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
    [MemberData(nameof(Examples))]
    public void CountValidWordsByMaskComparison_LeetCodeExamples_ReturnsValidWordCountPerPuzzle(
        string[] words, string[] puzzles, int[] expected) =>
        Assert.Equal(expected, NumberOfValidWordsForEachPuzzleSolution.CountValidWordsByMaskComparison(words, puzzles));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountValidWordsByMaskSubsetEnumeration_LeetCodeExamples_ReturnsValidWordCountPerPuzzle(
        string[] words, string[] puzzles, int[] expected) =>
        Assert.Equal(
            expected,
            NumberOfValidWordsForEachPuzzleSolution.CountValidWordsByMaskSubsetEnumeration(words, puzzles));
}
