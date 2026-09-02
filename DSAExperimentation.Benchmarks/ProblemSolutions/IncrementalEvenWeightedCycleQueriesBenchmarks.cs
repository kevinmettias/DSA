using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.IncrementalEvenWeightedCycleQueries;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are IncrementalEvenWeightedCycleQueriesSolution's, the
// same methods IncrementalEvenWeightedCycleQueriesTests proves correct.
// [GlobalSetup] builds one fixed, random edge stream over EdgeCount nodes so
// stream construction is charged to setup rather than to the pass each
// [Benchmark] arm measures.
[MemoryDiagnoser]
public class IncrementalEvenWeightedCycleQueriesBenchmarks
{
    private const int Seed = 3887;

    [Params(500, 5_000)]
    public int EdgeCount;

    private int _nodeCount;
    private int[][] _edges = null!;

    [GlobalSetup]
    public void Setup()
    {
        _nodeCount = EdgeCount;
        _edges = BuildEdges(_nodeCount, EdgeCount, new Random(Seed));
    }

    [Benchmark(Baseline = true)]
    public int BruteForceBfs() =>
        IncrementalEvenWeightedCycleQueriesSolution.CountAddedEdgesByBruteForceBfs(_nodeCount, _edges);

    [Benchmark]
    public int DisjointSetPrunedBfs() =>
        IncrementalEvenWeightedCycleQueriesSolution.CountAddedEdgesByDisjointSetPrunedBfs(_nodeCount, _edges);

    // Random (u, v, w) triples with u < v (matching LC's own 0 <= ui < vi < n
    // constraint) and a random 0/1 weight - node ids drawn from the same
    // EdgeCount-sized pool as the edges themselves, so a fair share of edges land
    // inside an already-connected component (the case the pruned strategy still
    // has to fall back to BFS for) rather than always stitching together fresh
    // ones.
    private static int[][] BuildEdges(int nodeCount, int edgeCount, Random random)
    {
        var edges = new int[edgeCount][];

        for (var i = 0; i < edgeCount; i++)
        {
            var a = random.Next(0, nodeCount);
            var b = random.Next(0, nodeCount);

            while (b == a)
            {
                b = random.Next(0, nodeCount);
            }

            var u = Math.Min(a, b);
            var v = Math.Max(a, b);
            var w = random.Next(0, 2);
            edges[i] = [u, v, w];
        }

        return edges;
    }
}
