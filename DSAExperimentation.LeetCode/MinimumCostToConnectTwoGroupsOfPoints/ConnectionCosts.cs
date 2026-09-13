namespace DSAExperimentation.LeetCode.MinimumCostToConnectTwoGroupsOfPoints;

// LC 1595's cost matrix plus the one derived table both strategies read: for every
// group-2 point, the cheapest edge reaching it from anywhere in group 1. That column
// minimum is what lets a group-2 point nobody chose still end up connected, and it is
// O(size1 * size2) to compute - work a benchmark would otherwise charge to whichever
// recursion it is measuring, so it is hoisted into the prepared-input container
// ARCHITECTURE.md §17.4 asks for.
//
// Deliberately not an IEnumerable, so the solution's LeetCode-shaped int[][] overload
// and its prepared overload can never be ambiguous. This is a witness for one problem
// and nothing else - the same reason LeetCode/NumberOfWaysToWearDifferentHatsToEachOther
// keeps HatPreferences beside its solution instead of in Domain.
internal sealed class ConnectionCosts
{
    private readonly int[][] _cost;
    private readonly int[] _cheapestFromGroupOne;

    private ConnectionCosts(int[][] cost, int groupOneSize, int groupTwoSize, int[] cheapestFromGroupOne)
    {
        _cost = cost;
        _cheapestFromGroupOne = cheapestFromGroupOne;
        GroupOneSize = groupOneSize;
        GroupTwoSize = groupTwoSize;
    }

    // Points in the first group, walked outward by both recurrences.
    public int GroupOneSize { get; }

    // Points in the second group; one bit of the connected-mask each, which is why
    // LeetCode caps this group at 12.
    public int GroupTwoSize { get; }

    public int Cost(int groupOnePoint, int groupTwoPoint) => _cost[groupOnePoint][groupTwoPoint];

    // The cheapest single edge into this group-2 point, used once the group-1 walk
    // has run out and a still-unconnected group-2 point has to be attached anyway.
    public int CheapestFromGroupOne(int groupTwoPoint) => _cheapestFromGroupOne[groupTwoPoint];

    public static ConnectionCosts Build(int[][] cost)
    {
        var groupOneSize = cost.Length;
        var groupTwoSize = cost[0].Length;

        return new ConnectionCosts(
            cost, groupOneSize, groupTwoSize, ColumnMinimums(cost, groupOneSize, groupTwoSize));
    }

    private static int[] ColumnMinimums(int[][] cost, int groupOneSize, int groupTwoSize)
    {
        var minimums = new int[groupTwoSize];

        for (var groupTwoPoint = 0; groupTwoPoint < groupTwoSize; groupTwoPoint++)
        {
            minimums[groupTwoPoint] = int.MaxValue;

            for (var groupOnePoint = 0; groupOnePoint < groupOneSize; groupOnePoint++)
            {
                minimums[groupTwoPoint] = Math.Min(minimums[groupTwoPoint], cost[groupOnePoint][groupTwoPoint]);
            }
        }

        return minimums;
    }
}
