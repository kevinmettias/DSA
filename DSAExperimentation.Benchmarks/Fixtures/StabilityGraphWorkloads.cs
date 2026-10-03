namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 3600 - a random spanning tree's own edges are
// marked must (guaranteeing they never form a cycle, so Feasible's binary search
// always has real work to do rather than short-circuiting to -1) plus extra
// random optional edges layered on top for the algorithm to actually choose
// among. An optional edge that would repeat a pair already joined is dropped,
// keeping LC 3600's "no duplicate edges" guarantee; its strength is still drawn, so
// every later draw lands where it always did.
internal static class StabilityGraphWorkloads
{
    private const int ExtraOptionalEdgesPerNode = 2;
    private const int StrengthCeilingExclusive = 100_000;
    private const int UpgradeBudgetDivisor = 4;

    public static (int[][] Edges, int K) Build(int nodeCount, int seed)
    {
        var random = new Random(seed);
        var edges = new List<int[]>();
        var joinedPairs = new HashSet<(int Low, int High)>();

        AddSpanningTreeEdges(edges, joinedPairs, random, nodeCount);
        AddOptionalEdges(edges, joinedPairs, random, nodeCount);

        return ([.. edges], nodeCount / UpgradeBudgetDivisor);
    }

    // One must-marked edge per node past the first, each hanging off an earlier node,
    // so the marked edges form a spanning tree and can never close a cycle.
    private static void AddSpanningTreeEdges(
        List<int[]> edges, HashSet<(int Low, int High)> joinedPairs, Random random, int nodeCount)
    {
        for (var node = 1; node < nodeCount; node++)
        {
            var parent = random.Next(node);
            joinedPairs.Add((parent, node));
            edges.Add([parent, node, random.Next(1, StrengthCeilingExclusive), 1]);
        }
    }

    // The optional edges the algorithm gets to choose among, skipping the self-loops
    // a random target can produce and any pair an earlier edge already joined.
    private static void AddOptionalEdges(
        List<int[]> edges, HashSet<(int Low, int High)> joinedPairs, Random random, int nodeCount)
    {
        for (var node = 0; node < nodeCount; node++)
        {
            for (var extra = 0; extra < ExtraOptionalEdgesPerNode; extra++)
            {
                var target = random.Next(nodeCount);

                if (target != node)
                {
                    var strength = random.Next(1, StrengthCeilingExclusive);
                    AddOptionalEdge(edges, joinedPairs, (node, target), strength);
                }
            }
        }
    }

    private static void AddOptionalEdge(
        List<int[]> edges, HashSet<(int Low, int High)> joinedPairs, (int From, int To) edge, int strength)
    {
        var pair = (Math.Min(edge.From, edge.To), Math.Max(edge.From, edge.To));

        if (joinedPairs.Add(pair))
        {
            edges.Add([edge.From, edge.To, strength, 0]);
        }
    }
}
