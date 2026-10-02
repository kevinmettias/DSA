using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.RedundantConnection;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RedundantConnectionSolution's, the same methods
// RedundantConnectionTests proves correct. The workload is a random tree over the node set plus
// exactly one extra edge, which is LC 684's own precondition; the edges are then shuffled so
// neither arm can rely on the answer being last. Both must name the same edge - the first one
// whose endpoints are already connected.
[MemoryDiagnoser]
public class RedundantConnectionBenchmarks
{
    private const int RandomSeed = 684; // LC problem number

    private int[][] _edges = [];

    [Params(64, 256)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var edges = new List<int[]>(NodeCount);

        for (var node = 2; node <= NodeCount; node++)
        {
            edges.Add([random.Next(1, node), node]);
        }

        edges.Add(ExtraEdge(random, edges));

        _edges = [.. edges.OrderBy(_ => random.Next())];
    }

    // Any edge between two distinct existing nodes closes a cycle, because the tree above
    // already connects every one of them - so the only work here is avoiding a duplicate of an
    // edge the tree already carries.
    private static int[] ExtraEdge(Random random, List<int[]> edges)
    {
        var existing = edges.Select(edge => (edge[0], edge[1])).ToHashSet();

        while (true)
        {
            var first = random.Next(1, edges.Count + 1);
            var second = random.Next(1, edges.Count + 1);

            if (first != second && !existing.Contains((first, second)) && !existing.Contains((second, first)))
            {
                return [first, second];
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int[] PathSearch() =>
        RedundantConnectionSolution.FindRedundantEdgeByPathSearch(_edges);

    [Benchmark]
    public int[] DisjointSet() =>
        RedundantConnectionSolution.FindRedundantEdgeByDisjointSet(_edges);
}
