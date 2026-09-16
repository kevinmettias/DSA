using DSAExperimentation.LeetCode.BricksFallingWhenHit;

namespace DSAExperimentation.LeetCode.Tests.BricksFallingWhenHit;

// The seam between BricksFallingWhenHitSolution's two strategies.
// HitBricksByForwardReplayBfs drops a brick per hit and re-floods the roof from
// scratch. HitBricksByReverseTimeDisjointSet runs time backwards: it starts from the
// fully-hit grid and "un-hits" bricks back in, unioning each restored brick into
// every standing neighbour and into a virtual roof node through this repo's own
// DataStructures.DisjointSet - a structure that only ever merges and so never has to
// undo one.
//
// DisjointSet has no size query, so the solution threads a size[] array beside it,
// re-reading size[Find(roof)] before and after each restore and calling the
// difference minus the restored brick the answer. That bookkeeping is the seam: the
// forest decides which component a brick joined, and the array beside it decides how
// big that component became, and the two only agree if every Union went through the
// one helper that updates both. A brick that joined a component by a path the helper
// did not see leaves a size nobody recomputed.
public sealed partial class ReverseTimeUnionSeamTests
{
    [Fact]
    public void HitBricks_LeetCodeExample_MatchesForwardReplay()
    {
        int[][] grid =
        [
            [1, 0, 0, 0],
            [1, 1, 1, 0],
        ];
        int[][] hits = [[1, 0]];

        Assert.Equal([2], HitBricksByReverseArm(grid, hits));
        AssertSameHits(grid, hits);
    }

    // Removing a brick whose column is still braced leaves the roof component's size
    // where it was, so the difference the answer is read from is zero.
    [Fact]
    public void HitBricks_LeetCodeExampleWithoutCollapse_MatchesForwardReplay()
    {
        int[][] grid =
        [
            [1, 0, 0, 0],
            [1, 1, 0, 0],
        ];
        int[][] hits = [[1, 1], [1, 0]];

        Assert.Equal([0, 0], HitBricksByReverseArm(grid, hits));
        AssertSameHits(grid, hits);
    }

    // A hit on an empty cell: no brick was there to remove, so the hit knocked down
    // nothing and the reverse walk has nothing to restore.
    [Fact]
    public void HitBricks_HitOnAnEmptyCell_MatchesForwardReplay()
    {
        int[][] grid =
        [
            [1, 0, 0],
            [1, 1, 1],
        ];
        int[][] hits = [[0, 2], [1, 0]];

        AssertSameHits(grid, hits);
    }

    // Every brick in the top row: each is roof-connected on its own, so removing one
    // can never drop another, and the roof component's size must not move.
    [Fact]
    public void HitBricks_TopRowOfStandingBricks_MatchesForwardReplay()
    {
        int[][] grid =
        [
            [1, 1, 1, 1],
            [1, 0, 0, 1],
        ];
        int[][] hits = [[0, 0], [0, 3], [1, 0]];

        AssertSameHits(grid, hits);
    }

    // A column hanging from a single roof brick: removing that one brick drops the
    // whole column at once, so the roof component's size changes by more than one and
    // the answer is the growth minus the restored brick alone.
    [Fact]
    public void HitBricks_ColumnHangingFromOneRoofBrick_MatchesForwardReplay()
    {
        int[][] grid =
        [
            [1, 0, 0],
            [1, 0, 0],
            [1, 0, 0],
        ];
        int[][] hits = [[0, 0], [1, 0], [2, 0]];

        Assert.Equal([2, 0, 0], HitBricksByReverseArm(grid, hits));
        AssertSameHits(grid, hits);
    }

    // A hit repeated on the same cell: the second hit finds no brick, so the reverse
    // walk's own "was this cell originally a brick" guard decides whether it restores
    // anything at all.
    [Fact]
    public void HitBricks_RepeatedHitOnTheSameCell_MatchesForwardReplay()
    {
        int[][] grid =
        [
            [1, 0, 1],
            [1, 1, 1],
        ];
        int[][] hits = [[1, 1], [1, 1], [0, 0]];

        AssertSameHits(grid, hits);
    }

    // The roof node is a virtual cell one past the grid's last index, so a grid whose
    // last cell is a brick tests that the union into the roof never collides with a
    // real brick's own id.
    [Fact]
    public void HitBricks_BrickAtTheFinalCell_MatchesForwardReplay()
    {
        int[][] grid =
        [
            [1, 1],
            [0, 1],
        ];
        int[][] hits = [[0, 0]];

        AssertSameHits(grid, hits);
    }

    [Fact]
    public void HitBricks_SingleCellGrid_MatchesForwardReplay()
    {
        int[][] grid = [[1]];
        int[][] hits = [[0, 0]];

        Assert.Equal([0], HitBricksByReverseArm(grid, hits));
        AssertSameHits(grid, hits);
    }

    private static void AssertSameHits(int[][] grid, int[][] hits)
        => Assert.Equal(
            BricksFallingWhenHitSolution.HitBricksByForwardReplayBfs(grid, hits),
            HitBricksByReverseArm(grid, hits));

    private static int[] HitBricksByReverseArm(int[][] grid, int[][] hits)
        => BricksFallingWhenHitSolution.HitBricksByReverseTimeDisjointSet(grid, hits);
}
