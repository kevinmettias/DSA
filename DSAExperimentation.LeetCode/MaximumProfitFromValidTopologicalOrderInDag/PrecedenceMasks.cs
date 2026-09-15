namespace DSAExperimentation.LeetCode.MaximumProfitFromValidTopologicalOrderInDag;

// LC 3530's DAG, reduced to exactly what the bitmask DP needs: for each node,
// which OTHER nodes must already be placed before it can be placed - one bitmask
// per node, built once so the composed strategy's recursion never re-scans the
// raw edge list.
internal sealed class PrecedenceMasks
{
    public int[] Masks { get; }

    private PrecedenceMasks(int[] masks) => Masks = masks;

    public static PrecedenceMasks Build(int n, int[][] edges)
    {
        var masks = new int[n];

        foreach (var edge in edges)
        {
            masks[edge[1]] |= 1 << edge[0];
        }

        return new PrecedenceMasks(masks);
    }
}
