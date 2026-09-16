using DSAExperimentation.LeetCode.CombinationSumII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CombinationSumII;

// Harness only: both strategies live in CombinationSumIISolution and are asserted
// against the same examples, so a failure names the strategy that broke.
public sealed class CombinationSumIITests
{
    public static TheoryData<int[], int, int[][]> Examples =>
        new()
        {
            { [10, 1, 2, 7, 6, 1, 5], 8, [[1, 1, 6], [1, 2, 5], [1, 7], [2, 6]] },
            { [2, 5, 2, 1, 2], 5, [[1, 2, 2], [5]] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindCombinationsBySpecializedRecursion_LeetCodeExamples_ReturnsUniqueCombinations(
        int[] candidates, int target, int[][] expected)
    {
        var actual = CombinationSumIISolution.FindCombinationsBySpecializedRecursion(candidates, target);

        AssertSameCombinations(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindCombinationsByBacktracking_LeetCodeExamples_ReturnsUniqueCombinations(
        int[] candidates, int target, int[][] expected)
    {
        var actual = CombinationSumIISolution.FindCombinationsByBacktracking(candidates, target);

        AssertSameCombinations(expected, actual);
    }

    private static void AssertSameCombinations(int[][] expected, List<List<int>> actual)
    {
        var actualArrays = actual.Select(x => x.ToArray()).ToArray();
        Assert.Equal(expected.Length, actualArrays.Length);

        foreach (var combination in expected)
        {
            Assert.Contains(actualArrays, x => x.SequenceEqual(combination));
        }
    }
}
