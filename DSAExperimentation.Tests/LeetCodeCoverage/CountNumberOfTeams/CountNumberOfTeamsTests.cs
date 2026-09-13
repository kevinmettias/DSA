using DSAExperimentation.LeetCode.CountNumberOfTeams;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountNumberOfTeams;

// Harness only. Both strategies are CountNumberOfTeamsSolution's - the cubic
// triple loop and the pair of coordinate-compressed Fenwick sweeps - pinned here
// to LeetCode's published examples plus the degenerate cases (equal ratings, too
// few soldiers) the sweep has to answer without a middle soldier existing.
public sealed class CountNumberOfTeamsTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [2, 5, 3, 4, 1], 3 },
            { [2, 1, 3], 0 },
            { [1, 2, 3, 4], 4 },
            { [1, 2, 3], 1 },
            { [3, 2, 1], 1 },
            { [1, 1, 1], 0 },
            { [1, 2], 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountTeamsByTripleLoop_LeetCodeExamples_ReturnsMonotonicTripleCount(
        int[] rating, int expected) =>
        Assert.Equal(expected, CountNumberOfTeamsSolution.CountTeamsByTripleLoop(rating));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountTeamsByFenwickSweeps_LeetCodeExamples_ReturnsMonotonicTripleCount(
        int[] rating, int expected) =>
        Assert.Equal(expected, CountNumberOfTeamsSolution.CountTeamsByFenwickSweeps(rating));
}
