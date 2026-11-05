using DSAExperimentation.LeetCode.Solutions.CountNumberOfTeams;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CountNumberOfTeams;

// Harness only. Both strategies are CountNumberOfTeamsSolution's - the cubic
// triple loop and the pair of coordinate-compressed Fenwick sweeps - pinned here
// to the degenerate cases (equal ratings, too
// few soldiers) the sweep has to answer without a middle soldier existing.
public sealed partial class CountNumberOfTeamsSolutionTests
{
    public static TheoryData<int[], int> Cases =>
        new()
        {
            { [1, 2, 3], 1 },
            { [3, 2, 1], 1 },
            { [1, 1, 1], 0 },
            { [1, 2], 0 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountTeamsByTripleLoop_Cases_ReturnsMonotonicTripleCount(
        int[] rating, int expected) =>
        Assert.Equal(expected, CountNumberOfTeamsSolution.CountTeamsByTripleLoop(rating));

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountTeamsByFenwickSweeps_Cases_ReturnsMonotonicTripleCount(
        int[] rating, int expected) =>
        Assert.Equal(expected, CountNumberOfTeamsSolution.CountTeamsByFenwickSweeps(rating));
}
