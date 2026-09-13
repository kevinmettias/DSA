using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.MinimumCostToConnectTwoGroupsOfPoints;

// LeetCode 1595. Minimum Cost to Connect Two Groups of Points: connect every point
// in both groups using the cheapest total set of cross-group edges, where a point may
// take part in more than one edge.
//
// Both strategies are the same recurrence, iterating group-1 points outward and
// carrying a bitmask of which group-2 points some chosen edge has already covered -
// group 2 is the small side (at most 12 points), which is what makes the mask fit in
// an int. f(index, mask) = the cheapest way to connect group-1 points index..end plus
// every group-2 point not yet in mask. Once index reaches the end of group 1, each
// group-2 point still missing from mask falls back to its own cheapest incoming edge,
// which is what lets a group-2 point go unchosen without going unconnected.
//
// The two arms differ only in whether that recurrence remembers anything: many
// different group-1 orderings reach the identical "these group-2 points are already
// covered" state, so the unmemoized arm re-explores each one from scratch.
internal static class MinimumCostToConnectTwoGroupsOfPointsSolution
{
    // The textbook answer: plain exponential recursion, no cache, nothing from this
    // repo in its internals. It is the arm the memoized strategy below has to justify
    // itself against, and putting it here is what finally gets it asserted.
    public static int ConnectTwoGroupsByBruteForceRecursion(int[][] cost) =>
        ConnectTwoGroupsByBruteForceRecursion(ConnectionCosts.Build(cost));

    public static int ConnectTwoGroupsByBruteForceRecursion(ConnectionCosts costs) =>
        CheapestFromScratch(0, 0, costs);

    private static int CheapestFromScratch(int index, int mask, ConnectionCosts costs)
    {
        if (index == costs.GroupOneSize)
        {
            return UncoveredCost(mask, costs);
        }

        var best = int.MaxValue;

        for (var groupTwoPoint = 0; groupTwoPoint < costs.GroupTwoSize; groupTwoPoint++)
        {
            var candidate = costs.Cost(index, groupTwoPoint) +
                CheapestFromScratch(index + 1, mask | (1 << groupTwoPoint), costs);
            best = Math.Min(best, candidate);
        }

        return best;
    }

    // The same recurrence routed through this repo's own Memoizer, keyed on the tuple
    // state (index, coveredGroupTwoPoints) - the identical (int, int) tuple-state
    // shape NumberOfWaysToWearDifferentHatsToEachOtherSolution uses for LC 1434's
    // (hat, mask) recursion, just minimizing cost instead of counting ways.
    public static int ConnectTwoGroupsByMemoizedBitmask(int[][] cost) =>
        ConnectTwoGroupsByMemoizedBitmask(ConnectionCosts.Build(cost));

    public static int ConnectTwoGroupsByMemoizedBitmask(ConnectionCosts costs) =>
        Memoizer.Memoize<(int Index, int Mask), int>(
            (0, 0), (state, cheapestFor) => CheapestFor(state, cheapestFor, costs));

    private static int CheapestFor(
        (int Index, int Mask) state, Func<(int Index, int Mask), int> cheapestFor, ConnectionCosts costs)
    {
        var (index, mask) = state;

        if (index == costs.GroupOneSize)
        {
            return UncoveredCost(mask, costs);
        }

        var best = int.MaxValue;

        for (var groupTwoPoint = 0; groupTwoPoint < costs.GroupTwoSize; groupTwoPoint++)
        {
            var candidate = costs.Cost(index, groupTwoPoint) +
                cheapestFor((index + 1, mask | (1 << groupTwoPoint)));
            best = Math.Min(best, candidate);
        }

        return best;
    }

    // The base case both arms share: every group-2 point no chosen edge covered still
    // has to be attached, and the cheapest way to do that is its own column minimum.
    private static int UncoveredCost(int mask, ConnectionCosts costs)
    {
        var remaining = 0;

        for (var groupTwoPoint = 0; groupTwoPoint < costs.GroupTwoSize; groupTwoPoint++)
        {
            if ((mask & (1 << groupTwoPoint)) == 0)
            {
                remaining += costs.CheapestFromGroupOne(groupTwoPoint);
            }
        }

        return remaining;
    }
}
