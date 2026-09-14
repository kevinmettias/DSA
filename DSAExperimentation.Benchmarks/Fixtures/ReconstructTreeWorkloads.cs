namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 1719 - a root with several disjoint chains
// hanging off it. Every node ends up related to every node on its own chain plus
// the root, and to no node on any other chain, so every non-root node clears the
// initial degree check and forces both strategies through the full
// candidate-parent/subset-check walk instead of short-circuiting early.
internal static class ReconstructTreeWorkloads
{
    // Parent pointer of the root, and of any node no chain reached.
    private const int Rootless = -1;

    // Pairs are built from parent links, so every node is paired with every one of
    // its own ancestors (root down to itself) - which is LC 1719's own "pairs is
    // the FULL ancestor/descendant relation" precondition.
    public static int[][] BuildStarOfChainsPairs(int nodeCount, int chainCount)
    {
        var parent = BuildRootedChainParents(nodeCount, chainCount);

        return BuildAncestorPairs(parent);
    }

    private static int[] BuildRootedChainParents(int nodeCount, int chainCount)
    {
        var parent = new int[nodeCount];

        for (var i = 0; i < nodeCount; i++)
        {
            parent[i] = Rootless;
        }

        var chainStart = 1;

        for (var c = 0; c < chainCount && chainStart < nodeCount; c++)
        {
            chainStart = AppendChain(parent, nodeCount, chainCount, chainStart);
        }

        return parent;
    }

    // Extends the chain that starts at chainStart by chainLength nodes, wiring each
    // node's parent pointer to the previous node on the chain (or 0, the root, for
    // the first node). Returns the start of the next chain.
    private static int AppendChain(int[] parent, int nodeCount, int chainCount, int chainStart)
    {
        var chainLength = (nodeCount - 1) / chainCount;
        var previous = 0;

        for (var i = 0; i < chainLength && chainStart + i < nodeCount; i++)
        {
            var node = chainStart + i;
            parent[node] = previous;
            previous = node;
        }

        return chainStart + chainLength;
    }

    private static int[][] BuildAncestorPairs(int[] parent)
    {
        var fullPairs = new List<int[]>();

        for (var node = 1; node < parent.Length; node++)
        {
            for (var ancestor = parent[node]; ancestor != Rootless; ancestor = parent[ancestor])
            {
                fullPairs.Add([ancestor, node]);
            }
        }

        return [.. fullPairs];
    }
}
