using DSAExperimentation.LeetCode.CombinationSumIII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CombinationSumIII;

// Harness only: both strategies live in CombinationSumIIISolution and are asserted
// against the same examples, so a failure names the strategy that broke.
public sealed class CombinationSumIIITests
{
    public static TheoryData<int, int, int[][]> Examples =>
        new()
        {
            { 3, 7, [[1, 2, 4]] },
            { 3, 9, [[1, 2, 6], [1, 3, 5], [2, 3, 4]] },
            { 4, 1, [] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CombinationsByBruteForce_LeetCodeExamples_ReturnsExpectedCombinations(
        int combinationSize, int targetSum, int[][] expected)
    {
        var actual = CombinationSumIIISolution.CombinationsByBruteForce(combinationSize, targetSum);

        AssertSameCombinations(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CombinationsByBacktrackEngine_LeetCodeExamples_ReturnsExpectedCombinations(
        int combinationSize, int targetSum, int[][] expected)
    {
        var actual = CombinationSumIIISolution.CombinationsByBacktrackEngine(combinationSize, targetSum);

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
