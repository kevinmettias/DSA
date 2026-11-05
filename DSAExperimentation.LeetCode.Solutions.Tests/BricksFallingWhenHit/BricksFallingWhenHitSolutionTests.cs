using DSAExperimentation.LeetCode.Solutions.BricksFallingWhenHit;

namespace DSAExperimentation.LeetCode.Solutions.Tests.BricksFallingWhenHit;

// Harness only. Both strategies are BricksFallingWhenHitSolution's - the forward
// BFS replay and the reverse-time DisjointSet walk - pinned here to the cases the
// two disagree about most easily: a hit on a cell that was never a brick, a hit
// whose own brick is the only thing that falls, a hit that severs a whole slab from
// the roof, and an empty list of hits. LeetCode's published examples run in
// CatalogCaseTests.
public sealed partial class BricksFallingWhenHitSolutionTests
{
    public static TheoryData<int[][], int[][], int[]> Cases =>
        new()
        {
            { [[1, 1, 1]], [[0, 1]], [0] },
            { [[1, 0], [1, 1]], [[0, 1]], [0] },
            { [[1, 1], [1, 0]], [[1, 0]], [0] },
            { [[1, 1, 1], [0, 0, 1], [1, 1, 1]], [[1, 2]], [3] },
            { [[1, 1]], [], [] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void HitBricksByForwardReplayBfs_Cases_ReturnsBricksFallenPerHit(
        int[][] grid, int[][] hits, int[] expected)
    {
        var bricksFallen = BricksFallingWhenHitSolution.HitBricksByForwardReplayBfs(grid, hits);

        Assert.Equal(expected, bricksFallen);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void HitBricksByReverseTimeDisjointSet_Cases_ReturnsBricksFallenPerHit(
        int[][] grid, int[][] hits, int[] expected)
    {
        var bricksFallen = BricksFallingWhenHitSolution.HitBricksByReverseTimeDisjointSet(grid, hits);

        Assert.Equal(expected, bricksFallen);
    }
}
