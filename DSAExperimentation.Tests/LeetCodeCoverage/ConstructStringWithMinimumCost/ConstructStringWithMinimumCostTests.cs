using DSAExperimentation.LeetCode.ConstructStringWithMinimumCost;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ConstructStringWithMinimumCost;

// Harness only. Both strategies are
// ConstructStringWithMinimumCostSolution's - this file just pins their
// LeetCode-shaped overloads to LeetCode's published examples, including the
// unreachable case where none of the words contain a letter target needs.
public sealed class ConstructStringWithMinimumCostTests
{
    public static TheoryData<string, string[], int[], int> Examples =>
        new()
        {
            { "abcdef", ["abdef", "abc", "d", "def", "ef"], [100, 1, 1, 10, 5], 7 },
            { "aaaa", ["z", "zz", "zzz"], [1, 10, 100], -1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinCostByBruteForceDp_LeetCodeExamples_ReturnsCheapestConstruction(
        string target, string[] words, int[] costs, int expected) =>
        Assert.Equal(expected, ConstructStringWithMinimumCostSolution.MinCostByBruteForceDp(target, words, costs));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinCostByAhoCorasickDp_LeetCodeExamples_ReturnsCheapestConstruction(
        string target, string[] words, int[] costs, int expected) =>
        Assert.Equal(expected, ConstructStringWithMinimumCostSolution.MinCostByAhoCorasickDp(target, words, costs));
}
