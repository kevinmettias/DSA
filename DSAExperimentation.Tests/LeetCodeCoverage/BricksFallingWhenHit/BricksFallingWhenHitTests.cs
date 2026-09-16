using DSAExperimentation.LeetCode.BricksFallingWhenHit;

namespace DSAExperimentation.Tests.LeetCodeCoverage.BricksFallingWhenHit;

// Harness only. Both strategies are BricksFallingWhenHitSolution's - the forward
// BFS replay and the reverse-time DisjointSet walk - pinned here to LeetCode's
// published examples plus the cases the two disagree about most easily: a hit on
// a cell that was never a brick, a hit whose own brick is the only thing that
// falls, and a hit that severs a whole slab from the roof.
public sealed partial class BricksFallingWhenHitTests
{
    public static TheoryData<int[][], int[][], int[]> Examples =>
        new()
        {
            { [[1, 0, 0, 0], [1, 1, 1, 0]], [[1, 0]], [2] },
            { [[1, 0, 0, 0], [1, 1, 0, 0]], [[1, 1], [1, 0]], [0, 0] },
            { [[1, 1, 1]], [[0, 1]], [0] },
            { [[1, 0], [1, 1]], [[0, 1]], [0] },
            { [[1, 1], [1, 0]], [[1, 0]], [0] },
            { [[1, 1, 1], [0, 0, 1], [1, 1, 1]], [[1, 2]], [3] },
            { [[1, 1]], [], [] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void HitBricksByForwardReplayBfs_LeetCodeExamples_ReturnsBricksFallenPerHit(
        int[][] grid, int[][] hits, int[] expected)
    {
        var bricksFallen = BricksFallingWhenHitSolution.HitBricksByForwardReplayBfs(grid, hits);

        Assert.Equal(expected, bricksFallen);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void HitBricksByReverseTimeDisjointSet_LeetCodeExamples_ReturnsBricksFallenPerHit(
        int[][] grid, int[][] hits, int[] expected)
    {
        var bricksFallen = BricksFallingWhenHitSolution.HitBricksByReverseTimeDisjointSet(grid, hits);

        Assert.Equal(expected, bricksFallen);
    }
}
