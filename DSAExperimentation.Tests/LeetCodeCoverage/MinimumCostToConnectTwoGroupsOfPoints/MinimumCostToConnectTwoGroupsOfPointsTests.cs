using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumCostToConnectTwoGroupsOfPoints;

// LeetCode 1595. Minimum Cost to Connect Two Groups of Points: the standard
// "iterate group-1 points outward, connected-group-2-points bitmask" DP, expressed
// as a memoized recursion over the tuple state (index, mask) via this repo's own
// Memoizer - the identical (int, int) tuple-state shape
// NumberOfWaysToWearDifferentHatsToEachOtherTests already uses for LC 1434's
// (hat, mask) recursion, just minimizing cost instead of counting ways. f(index,
// mask) = minimum cost to connect every group-1 point from index onward plus every
// group-2 point not yet in mask; once index reaches size1, a group-2 point not yet
// in mask falls back to its own cheapest single edge (minCost2), which is what lets
// a group-2 point go unconnected-by-choice-of-group-1 without going totally
// unconnected.
public sealed partial class MinimumCostToConnectTwoGroupsOfPointsTests
{
    [Fact]
    public void ConnectTwoGroups_TwoByTwoExample_ReturnsMinimumTotalCost()
    {
        int[][] cost = [[15, 96], [36, 2]];

        Assert.Equal(17, ConnectTwoGroups(cost));
    }

    [Fact]
    public void ConnectTwoGroups_ThreeByThreeExample_ReturnsMinimumTotalCost()
    {
        int[][] cost = [[1, 3, 5], [4, 1, 1], [1, 5, 3]];

        Assert.Equal(4, ConnectTwoGroups(cost));
    }

    private static int ConnectTwoGroups(int[][] cost)
    {
        var size1 = cost.Length;
        var size2 = cost[0].Length;
        var minCost2 = BuildMinCost2(cost, size1, size2);
        var context = new ConnectContext(cost, size1, size2, minCost2);

        return Memoizer.Memoize<(int Index, int Mask), int>(
            (0, 0), (state, costFor) => Recurrence(state, costFor, context));
    }

    private readonly record struct ConnectContext(int[][] Cost, int Size1, int Size2, int[] MinCost2);

    private static int[] BuildMinCost2(int[][] cost, int size1, int size2)
    {
        var minCost2 = new int[size2];
        for (var j = 0; j < size2; j++)
        {
            minCost2[j] = int.MaxValue;
            for (var i = 0; i < size1; i++)
            {
                minCost2[j] = Math.Min(minCost2[j], cost[i][j]);
            }
        }

        return minCost2;
    }

    private static int Recurrence((int Index, int Mask) state, Func<(int Index, int Mask), int> costFor, ConnectContext context)
    {
        var (index, mask) = state;

        if (index == context.Size1)
        {
            return RemainingCost(mask, context);
        }

        var best = int.MaxValue;
        for (var j = 0; j < context.Size2; j++)
        {
            var candidate = context.Cost[index][j] + costFor((index + 1, mask | (1 << j)));
            best = Math.Min(best, candidate);
        }

        return best;
    }

    private static int RemainingCost(int mask, ConnectContext context)
    {
        var remaining = 0;
        for (var j = 0; j < context.Size2; j++)
        {
            if ((mask & (1 << j)) == 0)
            {
                remaining += context.MinCost2[j];
            }
        }

        return remaining;
    }
}
