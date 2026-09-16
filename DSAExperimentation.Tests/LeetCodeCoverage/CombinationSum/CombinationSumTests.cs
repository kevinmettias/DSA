using DSAExperimentation.LeetCode.CombinationSum;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CombinationSum;

// Harness only: both strategies live in CombinationSumSolution and are asserted
// against the same examples, so a failure names the strategy that broke.
public sealed class CombinationSumTests
{
    public static TheoryData<int[], int, int[][]> Examples =>
        new()
        {
            { [2, 3, 6, 7], 7, [[2, 2, 3], [7]] },
            { [2, 3, 5], 8, [[2, 2, 2, 2], [2, 3, 3], [3, 5]] },
            { [2], 1, [] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindCombinationsBySpecializedRecursion_LeetCodeExamples_ReturnsExpectedCombinations(
        int[] candidates, int target, int[][] expected)
    {
        var actual = CombinationSumSolution.FindCombinationsBySpecializedRecursion(candidates, target);

        AssertSameCombinations(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindCombinationsByBacktracking_LeetCodeExamples_ReturnsExpectedCombinations(
        int[] candidates, int target, int[][] expected)
    {
        var actual = CombinationSumSolution.FindCombinationsByBacktracking(candidates, target);

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
