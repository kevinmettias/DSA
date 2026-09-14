namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 2368 - only how big the tree is and how much of it
// is fenced off; what the counting means is
// ReachableNodesWithRestrictionsSolution's job.
//
// Attaching each node to a uniformly random earlier one gives a genuinely irregular
// tree rather than a single long chain, so the flood fill walks a real branching
// structure, and restricting a fixed fraction of the non-root nodes leaves both a
// substantial reachable component and a substantial pruned remainder at every size.
internal static class RestrictedTreeWorkloads
{
    private const double RestrictedFraction = 0.2;

    public static (int[][] Edges, int[] Restricted) Build(int nodeCount, int seed)
    {
        var random = new Random(seed);
        var edges = BuildTreeEdges(random, nodeCount);
        var restricted = BuildRestricted(random, nodeCount);

        return (edges, restricted);
    }

    private static int[][] BuildTreeEdges(Random random, int nodeCount)
    {
        var edges = new int[nodeCount - 1][];

        for (var node = 1; node < nodeCount; node++)
        {
            var parent = random.Next(node);
            edges[node - 1] = [parent, node];
        }

        return edges;
    }

    // Node 0 is never a candidate: LeetCode guarantees the walk's own root is
    // unrestricted, and restricting it would make every arm answer zero.
    private static int[] BuildRestricted(Random random, int nodeCount)
    {
        var restricted = new List<int>();

        for (var node = 1; node < nodeCount; node++)
        {
            if (random.NextDouble() < RestrictedFraction)
            {
                restricted.Add(node);
            }
        }

        return [.. restricted];
    }
}
