using DSAExperimentation.LeetCode.Solutions.MostStonesRemovedWithSameRowOrColumn;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MostStonesRemovedWithSameRowOrColumn;

// Harness only: both strategies live in MostStonesRemovedWithSameRowOrColumnSolution
// and are pinned to the two degenerate cases the
// axis-keyed union has to agree with the pairwise scan on - a single stone, and stones
// that share no row or column at all.
public sealed partial class MostStonesRemovedWithSameRowOrColumnSolutionTests
{
    public static TheoryData<int[][], int> Cases =>
        new()
        {
            { [[0, 0], [1, 1], [2, 2]], 0 },
            { [[0, 0], [0, 1], [1, 1]], 2 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void RemoveStonesByPairwiseScan_Cases_ReturnsStonesMinusComponentCount(
        int[][] stones, int expected) =>
        Assert.Equal(expected, MostStonesRemovedWithSameRowOrColumnSolution.RemoveStonesByPairwiseScan(stones));

    [Theory]
    [MemberData(nameof(Cases))]
    public void RemoveStonesByRowColumnKeyedUnion_Cases_ReturnsStonesMinusComponentCount(
        int[][] stones, int expected) =>
        Assert.Equal(expected, MostStonesRemovedWithSameRowOrColumnSolution.RemoveStonesByRowColumnKeyedUnion(stones));
}
