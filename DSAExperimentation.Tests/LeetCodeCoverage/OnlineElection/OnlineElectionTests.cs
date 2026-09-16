using DSAExperimentation.LeetCode.OnlineElection;

namespace DSAExperimentation.Tests.LeetCodeCoverage.OnlineElection;

// Harness only: both strategies live in OnlineElectionSolution and are asserted
// against the same examples - LeetCode's published vote log with its six queries
// (including ones that land exactly on a vote time and ones that fall between
// two), the two-vote tie the old test drove by hand, a lead that changes hands
// only when the challenger draws level, and a single-candidate log queried after
// the last vote.
public sealed class OnlineElectionTests
{
    public static TheoryData<int[], int[], int[], int[]> Examples =>
        new()
        {
            {
                [0, 1, 1, 0, 0, 1, 0],
                [0, 5, 10, 15, 20, 25, 30],
                [3, 12, 25, 15, 24, 8],
                [0, 1, 1, 0, 0, 1]
            },
            { [0, 1], [0, 1], [0, 1], [0, 1] },
            { [0, 0, 1, 1], [0, 1, 2, 3], [0, 1, 2, 3], [0, 0, 0, 1] },
            { [5], [0], [0, 7], [5, 5] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void LeadersByPerQueryRescan_LeetCodeExamples_ReturnsLeaderAtEachQueriedTime(
        int[] persons, int[] times, int[] queries, int[] expected)
    {
        var actual = OnlineElectionSolution.LeadersByPerQueryRescan(persons, times, queries);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void LeadersByPrecomputedBinarySearch_LeetCodeExamples_ReturnsLeaderAtEachQueriedTime(
        int[] persons, int[] times, int[] queries, int[] expected)
    {
        var actual = OnlineElectionSolution.LeadersByPrecomputedBinarySearch(persons, times, queries);

        Assert.Equal(expected, actual);
    }
}
