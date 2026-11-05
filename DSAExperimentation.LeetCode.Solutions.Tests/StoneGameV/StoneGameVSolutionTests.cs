using DSAExperimentation.LeetCode.Solutions.StoneGameV;

namespace DSAExperimentation.LeetCode.Solutions.Tests.StoneGameV;

// Harness only. Both strategies are StoneGameVSolution's -
// MaxScoreByUnmemoizedRecursion (previously untested scaffolding inlined in the
// benchmark as its baseline arm) now gets the same cases as
// MaxScoreByMemoizedRecursion (previously the test's own private helper), so a
// failure names the strategy that broke.
public sealed partial class StoneGameVSolutionTests
{
    public static TheoryData<int[], int> Cases =>
        new()
        {
            { [1, 1], 1 },
            { [1, 2], 1 },
            { [2, 1], 1 },
            { [1, 2, 3], 4 },
            { [1, 1, 1, 1], 3 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxScoreByUnmemoizedRecursion_Cases_ReturnsBestAchievableScore(
        int[] stoneValue, int expected) =>
        Assert.Equal(expected, StoneGameVSolution.MaxScoreByUnmemoizedRecursion(stoneValue));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxScoreByMemoizedRecursion_Cases_ReturnsBestAchievableScore(
        int[] stoneValue, int expected) =>
        Assert.Equal(expected, StoneGameVSolution.MaxScoreByMemoizedRecursion(stoneValue));
}
