using DSAExperimentation.LeetCode.MinimumCostToConvertStringI;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumCostToConvertStringI;

// Harness only. The 26-letter conversion graph is
// LeetCode.MinimumCostToConvertStringI.LetterNetwork and both Floyd-Warshall
// strategies are MinimumCostToConvertStringISolution's - this file just pins
// them to LeetCode's published examples, including the unreachable-character
// case the baseline's raw matrix and the graph strategy's distance
// dictionary both have to report as -1 without ever agreeing on a magic
// in-between value.
public sealed class MinimumCostToConvertStringITests
{
    public static TheoryData<string, string, char[], char[], int[], long> Examples =>
        new()
        {
            {
                "abcd", "acbe",
                ['a', 'b', 'c', 'c', 'e', 'd'], ['b', 'c', 'b', 'e', 'b', 'e'], [2, 5, 5, 1, 2, 20],
                28
            },
            {
                "aaaa", "bbbb",
                ['a', 'c'], ['c', 'b'], [1, 2],
                12
            },
            {
                "abcd", "abce",
                ['a'], ['e'], [10000],
                -1
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumCostByBruteForceFloydWarshall_LeetCodeExamples_ReturnsMinimumConversionCost(
        string source, string target, char[] original, char[] changed, int[] cost, long expected) =>
        Assert.Equal(expected, MinimumCostToConvertStringISolution.MinimumCostByBruteForceFloydWarshall(source, target, original, changed, cost));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumCostByAllPairsShortestPaths_LeetCodeExamples_ReturnsMinimumConversionCost(
        string source, string target, char[] original, char[] changed, int[] cost, long expected) =>
        Assert.Equal(expected, MinimumCostToConvertStringISolution.MinimumCostByAllPairsShortestPaths(source, target, original, changed, cost));
}
