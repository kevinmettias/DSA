namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 3575. Values are random over a range wide enough
// that some naturally repeat a digit within themselves (excluding that node's own
// value from every good subset) while most stay digit-disjoint, so both branches of
// the per-node knapsack merge do real work.
internal static class GoodSubtreeWorkloads
{
    private const int ValueBound = 99_999;

    public static (int[] Vals, int[] Par) Build(int nodeCount, int seed)
    {
        var random = new Random(seed);
        var vals = new int[nodeCount];
        var par = new int[nodeCount];
        par[0] = -1;

        for (var i = 0; i < nodeCount; i++)
        {
            vals[i] = random.Next(1, ValueBound);

            if (i > 0)
            {
                // Every earlier node is a legal parent, biasing toward a bushy rather
                // than a straight-chain tree.
                par[i] = random.Next(0, i);
            }
        }

        return (vals, par);
    }
}
