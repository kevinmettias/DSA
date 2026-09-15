using DSAExperimentation.LeetCode.MinimumCostToConvertStringII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumCostToConvertStringII;

// Harness only. The distinct-substring conversion graph is
// LeetCode.MinimumCostToConvertStringII.SubstringNetwork and both
// Floyd-Warshall strategies are MinimumCostToConvertStringIISolution's -
// this file just pins them to LeetCode's published examples: a
// single-character case identical to LC 2976's, a multi-character chain
// requiring two hops through one substring, and an unreachable case.
public sealed class MinimumCostToConvertStringIITests
{
    public static TheoryData<string, string, string[], string[], int[], long> Examples =>
        new()
        {
            {
                "abcd", "acbe",
                ["a", "b", "c", "c", "e", "d"], ["b", "c", "b", "e", "b", "e"], [2, 5, 5, 1, 2, 20],
                28
            },
            {
                "abcdefgh", "acdeeghh",
                ["bcd", "fgh", "thh"], ["cde", "thh", "ghh"], [1, 3, 5],
                9
            },
            {
                "abcdefgh", "addddddd",
                ["bcd", "defgh"], ["ddd", "ddddd"], [100, 1578],
                -1
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumCostByBruteForceFloydWarshall_LeetCodeExamples_ReturnsMinimumConversionCost(
        string source, string target, string[] original, string[] changed, int[] cost, long expected) =>
        Assert.Equal(
            expected,
            MinimumCostToConvertStringIISolution.MinimumCostByBruteForceFloydWarshall(
                new SourceText(source), new TargetText(target), (original, changed, cost)));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumCostByAllPairsShortestPaths_LeetCodeExamples_ReturnsMinimumConversionCost(
        string source, string target, string[] original, string[] changed, int[] cost, long expected) =>
        Assert.Equal(
            expected,
            MinimumCostToConvertStringIISolution.MinimumCostByAllPairsShortestPaths(
                new SourceText(source), new TargetText(target), (original, changed, cost)));
}
