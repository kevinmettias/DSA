using DSAExperimentation.LeetCode.FindTheWinnerOfTheCircularGame;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindTheWinnerOfTheCircularGame;

// Harness only. Both simulations live in FindTheWinnerOfTheCircularGameSolution -
// this file pins them to LeetCode's published examples plus the single-friend
// circle, k = 1 (nobody is counted past, so the last friend survives), the two-friend
// circle, and k > n, where the count wraps the circle more than once before landing.
public sealed class FindTheWinnerOfTheCircularGameTests
{
    public static TheoryData<int, int, int> Examples =>
        new()
        {
            { 5, 2, 3 },
            { 6, 5, 1 },
            { 1, 1, 1 },
            { 5, 1, 5 },
            { 6, 2, 5 },
            { 2, 2, 1 },
            { 4, 4, 2 },
            { 3, 5, 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindTheWinnerByListRemoval_LeetCodeExamples_ReturnsTheLastFriendStanding(
        int n, int k, int expected) =>
        Assert.Equal(expected, FindTheWinnerOfTheCircularGameSolution.FindTheWinnerByListRemoval(n, k));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindTheWinnerByQueueRotation_LeetCodeExamples_ReturnsTheLastFriendStanding(
        int n, int k, int expected) =>
        Assert.Equal(expected, FindTheWinnerOfTheCircularGameSolution.FindTheWinnerByQueueRotation(n, k));
}
