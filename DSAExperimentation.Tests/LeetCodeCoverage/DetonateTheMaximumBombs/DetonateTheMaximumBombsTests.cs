using DSAExperimentation.Algorithms.Traversal.DepthFirst;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DetonateTheMaximumBombs;

// LeetCode 2101. Detonate the Maximum Bombs: bomb i's blast reaches bomb j exactly
// when j's center falls within i's radius - a directed edge i -> j, the same
// implicit-graph shape JumpGameIIITests already proves for this repo's own
// DepthFirstSearch.Traverse(start, successors). "How many bombs detonate if I
// trigger bomb i" is just the reachable-set size from i, tried once per candidate
// start. Squared-distance-vs-radius-squared comparisons (long-typed to avoid
// overflow) are plain scalar arithmetic - no repo geometry primitive applies to a
// single circle-contains-point check, the same "lighter repo-primitive fit"
// CircleAndRectangleOverlappingTests already documents.
public sealed partial class DetonateTheMaximumBombsTests
{
    [Fact]
    public void MaxDetonations_OneWayOverlapBetweenTwoBombs_DetonatesBoth()
    {
        int[][] bombs = [[2, 1, 3], [6, 1, 4]];

        var maxDetonated = MaxDetonations(bombs);

        Assert.Equal(2, maxDetonated);
    }

    [Fact]
    public void MaxDetonations_BombsOutOfEachOthersRange_OnlyDetonatesItself()
    {
        int[][] bombs = [[1, 1, 5], [10, 10, 5]];

        var maxDetonated = MaxDetonations(bombs);

        Assert.Equal(1, maxDetonated);
    }

    [Fact]
    public void MaxDetonations_LinearChainOfFiveBombs_DetonatesAllFromTheChainStart()
    {
        int[][] bombs = [[1, 2, 3], [2, 3, 1], [3, 4, 2], [4, 5, 3], [5, 6, 4]];

        var maxDetonated = MaxDetonations(bombs);

        Assert.Equal(5, maxDetonated);
    }

    private static int MaxDetonations(int[][] bombs)
    {
        var maxDetonated = 0;

        for (var i = 0; i < bombs.Length; i++)
        {
            var reached = DepthFirstSearch.Traverse(i, index => Reachable(bombs, index));
            maxDetonated = Math.Max(maxDetonated, reached.Count);
        }

        return maxDetonated;
    }

    private static IEnumerable<int> Reachable(int[][] bombs, int index)
    {
        var x = bombs[index][0];
        var y = bombs[index][1];
        var radius = bombs[index][2];

        for (var j = 0; j < bombs.Length; j++)
        {
            if (j == index)
            {
                continue;
            }

            var dx = (long)(bombs[j][0] - x);
            var dy = (long)(bombs[j][1] - y);

            if (dx * dx + dy * dy <= (long)radius * radius)
            {
                yield return j;
            }
        }
    }
}
