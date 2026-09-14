using DSAExperimentation.LeetCode.TheEarliestAndLatestRoundsWherePlayersCompete;

namespace DSAExperimentation.Tests.LeetCodeCoverage.TheEarliestAndLatestRoundsWherePlayersCompete;

// Harness only: both strategies live in
// TheEarliestAndLatestRoundsWherePlayersCompeteSolution and return the same
// (earliest, latest) pair - the unmemoized recursion that used to be a
// benchmark-only baseline nothing asserted, and the Memoizer-backed one. One test
// method per strategy over LeetCode's published examples plus the immediate-meeting
// and mirror-normalization edges.
public sealed class TheEarliestAndLatestRoundsWherePlayersCompeteTests
{
    public static TheoryData<int, int, int, int, int> Examples =>
        new()
        {
            // LC example 1: they can meet in round 3 at the earliest, round 4 at the latest.
            { 11, 2, 4, 3, 4 },

            // LC example 2: mirror positions, so they are paired in round 1 either way.
            { 5, 1, 5, 1, 1 },

            // Every branch pushes the meeting to the last round.
            { 10, 3, 4, 4, 4 },

            // The same bracket as LC example 1 with the players given in the other order.
            { 11, 4, 2, 3, 4 },

            // The smallest bracket: one match, and it is theirs.
            { 2, 1, 2, 1, 1 },

            // Mirror pair around the odd bracket's automatic-bye middle player.
            { 5, 2, 4, 1, 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void EarliestAndLatestByPlainRecursion_LeetCodeExamples_ReturnsExpectedRoundBounds(
        int n, int firstPlayer, int secondPlayer, int expectedEarliest, int expectedLatest) =>
        Assert.Equal(
            (expectedEarliest, expectedLatest),
            TheEarliestAndLatestRoundsWherePlayersCompeteSolution.EarliestAndLatestByPlainRecursion(
                n, firstPlayer, secondPlayer));

    [Theory]
    [MemberData(nameof(Examples))]
    public void EarliestAndLatestByMemoizedRecurrence_LeetCodeExamples_ReturnsExpectedRoundBounds(
        int n, int firstPlayer, int secondPlayer, int expectedEarliest, int expectedLatest) =>
        Assert.Equal(
            (expectedEarliest, expectedLatest),
            TheEarliestAndLatestRoundsWherePlayersCompeteSolution.EarliestAndLatestByMemoizedRecurrence(
                n, firstPlayer, secondPlayer));
}
