using DSAExperimentation.LeetCode.Solutions.FindTheWinnerOfTheCircularGame;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FindTheWinnerOfTheCircularGame;

// Harness only. Both simulations live in FindTheWinnerOfTheCircularGameSolution -
// this file pins them to the single-friend
// circle, stepSize = 1 (nobody is counted past, so the last friend survives), the
// two-friend circle, and stepSize > friendCount, where the count wraps the circle
// more than once before landing.
public sealed partial class FindTheWinnerOfTheCircularGameSolutionTests
{
    public static TheoryData<int, int, int> Cases =>
        new()
        {
            { 5, 1, 5 },
            { 6, 2, 5 },
            { 2, 2, 1 },
            { 3, 5, 1 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindTheWinnerByListRemoval_Cases_ReturnsTheLastFriendStanding(
        int friendCount, int stepSize, int expected)
    {
        var actual = FindTheWinnerOfTheCircularGameSolution.FindTheWinnerByListRemoval(friendCount, stepSize);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindTheWinnerByQueueRotation_Cases_ReturnsTheLastFriendStanding(
        int friendCount, int stepSize, int expected)
    {
        var actual = FindTheWinnerOfTheCircularGameSolution.FindTheWinnerByQueueRotation(friendCount, stepSize);

        Assert.Equal(expected, actual);
    }
}
