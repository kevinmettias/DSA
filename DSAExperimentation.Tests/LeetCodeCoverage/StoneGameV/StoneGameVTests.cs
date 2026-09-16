using DSAExperimentation.LeetCode.StoneGameV;

namespace DSAExperimentation.Tests.LeetCodeCoverage.StoneGameV;

// Harness only. Both strategies are StoneGameVSolution's -
// MaxScoreByUnmemoizedRecursion (previously untested scaffolding inlined in the
// benchmark as its baseline arm) now gets the same examples as
// MaxScoreByMemoizedRecursion (previously the test's own private helper), so a
// failure names the strategy that broke.
public sealed partial class StoneGameVTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [6, 2, 3, 4, 5, 5], 18 },
            { [7, 7, 7, 7, 7, 7, 7], 28 },
            { [4], 0 },
            { [1, 1], 1 },
            { [1, 2], 1 },
            { [2, 1], 1 },
            { [1, 2, 3], 4 },
            { [1, 1, 1, 1], 3 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxScoreByUnmemoizedRecursion_LeetCodeExamples_ReturnsBestAchievableScore(
        int[] stoneValue, int expected) =>
        Assert.Equal(expected, StoneGameVSolution.MaxScoreByUnmemoizedRecursion(stoneValue));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxScoreByMemoizedRecursion_LeetCodeExamples_ReturnsBestAchievableScore(
        int[] stoneValue, int expected) =>
        Assert.Equal(expected, StoneGameVSolution.MaxScoreByMemoizedRecursion(stoneValue));
}
