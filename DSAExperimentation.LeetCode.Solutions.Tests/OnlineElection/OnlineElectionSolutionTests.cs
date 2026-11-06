using DSAExperimentation.LeetCode.Solutions.OnlineElection;

namespace DSAExperimentation.LeetCode.Solutions.Tests.OnlineElection;

// Harness only: both strategies live in OnlineElectionSolution, each example's
// queries replayed one at a time on the object its factory makes, and are asserted
// against the same examples - LeetCode's published vote log with its six queries
// (including ones that land exactly on a vote time and ones that fall between
// two), the two-vote tie the old test drove by hand, a lead that changes hands
// only when the challenger draws level, and a single-candidate log queried after
// the last vote.
public sealed partial class OnlineElectionSolutionTests
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
    public void CreateByPerQueryRescan_LeetCodeExamples_ReturnsLeaderAtEachQueriedTime(
        int[] persons, int[] times, int[] queries, int[] expected)
    {
        var candidate = OnlineElectionSolution.CreateByPerQueryRescan(persons, times);
        var leaders = QueryEach(candidate, queries);

        Assert.Equal(expected, leaders);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByPrecomputedBinarySearch_LeetCodeExamples_ReturnsLeaderAtEachQueriedTime(
        int[] persons, int[] times, int[] queries, int[] expected)
    {
        var candidate = OnlineElectionSolution.CreateByPrecomputedBinarySearch(persons, times);
        var leaders = QueryEach(candidate, queries);

        Assert.Equal(expected, leaders);
    }

    // One q per queried time, in order, on the one object - LeetCode's call script.
    private static int[] QueryEach(OnlineElectionSolution.ITopVotedCandidate candidate, int[] queries) =>
        [.. queries.Select(candidate.Q)];
}
