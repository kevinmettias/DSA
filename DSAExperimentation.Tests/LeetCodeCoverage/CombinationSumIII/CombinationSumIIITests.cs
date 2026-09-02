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
        int k, int n, int[][] expected) =>
        AssertSameCombinations(expected, CombinationSumIIISolution.CombinationsByBruteForce(k, n));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CombinationsByBacktrackEngine_LeetCodeExamples_ReturnsExpectedCombinations(
        int k, int n, int[][] expected) =>
        AssertSameCombinations(expected, CombinationSumIIISolution.CombinationsByBacktrackEngine(k, n));

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
