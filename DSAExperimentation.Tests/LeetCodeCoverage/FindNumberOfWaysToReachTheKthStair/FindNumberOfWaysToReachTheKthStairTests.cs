using DSAExperimentation.LeetCode.FindNumberOfWaysToReachTheKthStair;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindNumberOfWaysToReachTheKthStair;

// Harness only. Both strategies are FindNumberOfWaysToReachTheKthStairSolution's -
// this file just pins them to LeetCode's published examples.
public sealed partial class FindNumberOfWaysToReachTheKthStairTests
{
    public static TheoryData<int, int> Examples =>
        new()
        {
            { 0, 2 },
            { 1, 4 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void WaysByBruteRecursion_LeetCodeExamples_ReturnsWayCount(int targetStair, int expected) =>
        Assert.Equal(expected, FindNumberOfWaysToReachTheKthStairSolution.WaysByBruteRecursion(targetStair));

    [Theory]
    [MemberData(nameof(Examples))]
    public void WaysByMemoizedRecurrence_LeetCodeExamples_ReturnsWayCount(int targetStair, int expected) =>
        Assert.Equal(expected, FindNumberOfWaysToReachTheKthStairSolution.WaysByMemoizedRecurrence(targetStair));
}
