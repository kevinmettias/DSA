using DSAExperimentation.LeetCode.CountCellsInOverlappingHorizontalAndVerticalSubstrings;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountCellsInOverlappingHorizontalAndVerticalSubstrings;

// Harness only. Both strategies are
// CountCellsInOverlappingHorizontalAndVerticalSubstringsSolution's - this file
// just pins them to LeetCode's published examples, including the single-cell
// grid (Example 3) where the flattened row-major and column-major strings
// coincide.
public sealed partial class CountCellsInOverlappingHorizontalAndVerticalSubstringsTests
{
    public static TheoryData<char[][], string, int> Examples =>
        new()
        {
            {
                [
                    ['a', 'a', 'c', 'c'],
                    ['b', 'b', 'b', 'c'],
                    ['a', 'a', 'b', 'a'],
                    ['c', 'a', 'a', 'c'],
                    ['a', 'a', 'b', 'a'],
                ],
                "abaca",
                1
            },
            {
                [
                    ['c', 'a', 'a', 'a'],
                    ['a', 'a', 'b', 'a'],
                    ['b', 'b', 'a', 'a'],
                    ['a', 'a', 'b', 'a'],
                ],
                "aba",
                4
            },
            {
                [['a']],
                "a",
                1
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountCellsByBruteForce_LeetCodeExamples_ReturnsCellsCoveredByBothDirections(
        char[][] grid, string pattern, int expected)
    {
        var actual = CountCellsInOverlappingHorizontalAndVerticalSubstringsSolution.CountCellsByBruteForce(grid, pattern);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountCellsByZFunction_LeetCodeExamples_ReturnsCellsCoveredByBothDirections(
        char[][] grid, string pattern, int expected)
    {
        var actual = CountCellsInOverlappingHorizontalAndVerticalSubstringsSolution.CountCellsByZFunction(grid, pattern);
        Assert.Equal(expected, actual);
    }
}
