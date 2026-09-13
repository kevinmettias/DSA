using DSAExperimentation.LeetCode.MostStonesRemovedWithSameRowOrColumn;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MostStonesRemovedWithSameRowOrColumn;

// Harness only: both strategies live in MostStonesRemovedWithSameRowOrColumnSolution
// and are pinned to LeetCode's published examples, plus the two degenerate cases the
// axis-keyed union has to agree with the pairwise scan on - a single stone, and stones
// that share no row or column at all.
public sealed class MostStonesRemovedWithSameRowOrColumnTests
{
    public static TheoryData<int[][], int> Examples =>
        new()
        {
            { [[0, 0], [0, 1], [1, 0], [1, 2], [2, 1], [2, 2]], 5 },
            { [[0, 0], [0, 2], [1, 1], [2, 0], [2, 2]], 3 },
            { [[0, 0]], 0 },
            { [[0, 0], [1, 1], [2, 2]], 0 },
            { [[0, 0], [0, 1], [1, 1]], 2 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void RemoveStonesByPairwiseScan_LeetCodeExamples_ReturnsStonesMinusComponentCount(
        int[][] stones, int expected) =>
        Assert.Equal(expected, MostStonesRemovedWithSameRowOrColumnSolution.RemoveStonesByPairwiseScan(stones));

    [Theory]
    [MemberData(nameof(Examples))]
    public void RemoveStonesByRowColumnKeyedUnion_LeetCodeExamples_ReturnsStonesMinusComponentCount(
        int[][] stones, int expected) =>
        Assert.Equal(expected, MostStonesRemovedWithSameRowOrColumnSolution.RemoveStonesByRowColumnKeyedUnion(stones));
}
