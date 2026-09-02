namespace DSAExperimentation.LeetCode.PathExistenceQueriesInAGraphII;

// LC 3534's whole graph is implicit: node i and node j are adjacent exactly when
// |nums[i] - nums[j]| <= maxDiff. Sorting nodes by value turns that into a single
// contiguous-range condition per node, which is the one fact
// MinDistancesByBinaryLifting's doubling walk depends on - so this witness exists
// purely to fix that ordering once, answering this problem alone (ARCHITECTURE.md
// §17.3), not a general enough shape for Domain/.
//
// PositionOf is the inverse permutation: PositionOf[originalIndex] is where that
// node landed in SortedValues, which is what a query's u/v indices are converted
// through before anything else happens.
internal sealed class SortedByValueGraph
{
    public int[] SortedValues { get; }

    public int[] PositionOf { get; }

    private SortedByValueGraph(int[] sortedValues, int[] positionOf)
    {
        SortedValues = sortedValues;
        PositionOf = positionOf;
    }

    public static SortedByValueGraph Build(int[] nums)
    {
        var order = Enumerable.Range(0, nums.Length).ToArray();
        Array.Sort(order, (x, y) => nums[x].CompareTo(nums[y]));

        var sortedValues = new int[nums.Length];
        var positionOf = new int[nums.Length];

        for (var position = 0; position < order.Length; position++)
        {
            sortedValues[position] = nums[order[position]];
            positionOf[order[position]] = position;
        }

        return new SortedByValueGraph(sortedValues, positionOf);
    }
}
