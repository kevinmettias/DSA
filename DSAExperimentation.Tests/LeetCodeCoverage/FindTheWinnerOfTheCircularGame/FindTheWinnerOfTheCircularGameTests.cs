using DSAExperimentation.LeetCode.FindTheWinnerOfTheCircularGame;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindTheWinnerOfTheCircularGame;

// Harness only. Both simulations live in FindTheWinnerOfTheCircularGameSolution -
// this file pins them to LeetCode's published examples plus the single-friend
// circle, stepSize = 1 (nobody is counted past, so the last friend survives), the
// two-friend circle, and stepSize > friendCount, where the count wraps the circle
// more than once before landing.
public sealed partial class FindTheWinnerOfTheCircularGameTests
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
        int friendCount, int stepSize, int expected)
    {
        var actual = FindTheWinnerOfTheCircularGameSolution.FindTheWinnerByListRemoval(friendCount, stepSize);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindTheWinnerByQueueRotation_LeetCodeExamples_ReturnsTheLastFriendStanding(
        int friendCount, int stepSize, int expected)
    {
        var actual = FindTheWinnerOfTheCircularGameSolution.FindTheWinnerByQueueRotation(friendCount, stepSize);

        Assert.Equal(expected, actual);
    }
}
