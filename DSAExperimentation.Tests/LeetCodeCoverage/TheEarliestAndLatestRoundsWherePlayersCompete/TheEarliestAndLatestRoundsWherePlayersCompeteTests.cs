using DSAExperimentation.LeetCode.TheEarliestAndLatestRoundsWherePlayersCompete;

namespace DSAExperimentation.Tests.LeetCodeCoverage.TheEarliestAndLatestRoundsWherePlayersCompete;

// Harness only: both strategies live in
// TheEarliestAndLatestRoundsWherePlayersCompeteSolution and return the same
// (earliest, latest) pair - the unmemoized recursion that used to be a
// benchmark-only baseline nothing asserted, and the Memoizer-backed one. One test
// method per strategy over LeetCode's published examples plus the immediate-meeting
// and mirror-normalization edges.
public sealed partial class TheEarliestAndLatestRoundsWherePlayersCompeteTests
{
    public static TheoryData<RoundBoundsExample> Examples =>
        new()
        {
            // LC example 1: they can meet in round 3 at the earliest, round 4 at the latest.
            new RoundBoundsExample(11, 2, 4, 3, 4),

            // LC example 2: mirror positions, so they are paired in round 1 either way.
            new RoundBoundsExample(5, 1, 5, 1, 1),

            // Every branch pushes the meeting to the last round.
            new RoundBoundsExample(10, 3, 4, 4, 4),

            // The same bracket as LC example 1 with the players given in the other order.
            new RoundBoundsExample(11, 4, 2, 3, 4),

            // The smallest bracket: one match, and it is theirs.
            new RoundBoundsExample(2, 1, 2, 1, 1),

            // Mirror pair around the odd bracket's automatic-bye middle player.
            new RoundBoundsExample(5, 2, 4, 1, 1),
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void EarliestAndLatestByPlainRecursion_LeetCodeExamples_ReturnsExpectedRoundBounds(
        RoundBoundsExample example)
    {
        var actual = TheEarliestAndLatestRoundsWherePlayersCompeteSolution.EarliestAndLatestByPlainRecursion(
            example.N, example.FirstPlayer, example.SecondPlayer);

        Assert.Equal((example.ExpectedEarliest, example.ExpectedLatest), actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void EarliestAndLatestByMemoizedRecurrence_LeetCodeExamples_ReturnsExpectedRoundBounds(
        RoundBoundsExample example)
    {
        var actual = TheEarliestAndLatestRoundsWherePlayersCompeteSolution.EarliestAndLatestByMemoizedRecurrence(
            example.N, example.FirstPlayer, example.SecondPlayer);

        Assert.Equal((example.ExpectedEarliest, example.ExpectedLatest), actual);
    }

    // Nested because it is only ever used inside this test class and has no
    // independent identity: this harness's own vocabulary for one LeetCode example,
    // and the bracket, the two players and the expected round bounds are one thing
    // that travels together rather than five arguments a caller must count.
    public readonly record struct RoundBoundsExample(
        int N, int FirstPlayer, int SecondPlayer, int ExpectedEarliest, int ExpectedLatest);
}
