using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.NumberOfGoodPaths;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NumberOfGoodPathsSolution's, the same methods
// NumberOfGoodPathsTests proves correct - the textbook per-pair path walk (O(n^2)
// pairs, each confirmed by an O(n) tree walk) against this repo's own DisjointSet
// sweep (edges in increasing order of their higher-valued endpoint, O(n * alpha(n))
// total). [GlobalSetup] builds the tree so neither arm is charged for it.
[MemoryDiagnoser]
public class NumberOfGoodPathsBenchmarks
{
    private const int RandomSeed = 2421; private int[] _vals = [];

    private int[][] _edges = [];
    // LC problem number

    [Params(50, 500)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        // A path graph 0-1-2-...-(n-1) with values drawn from a bounded range:
        // pigeonhole guarantees repeated values along the chain, so the naive
        // approach's per-pair path walk has real same-value pairs to chase rather
        // than exiting immediately on "no duplicate value" checks.
        var random = new Random(RandomSeed);
        _vals = Enumerable.Range(0, NodeCount).Select(_ => random.Next(1, NodeCount)).ToArray();

        _edges = BuildPathEdges(NodeCount);
    }

    // The path graph 0-1-2-...-(n-1): one edge between each consecutive pair.
    private static int[][] BuildPathEdges(int nodeCount)
    {
        var edges = new int[nodeCount - 1][];

        for (var i = 0; i < nodeCount - 1; i++)
        {
            edges[i] = [i, i + 1];
        }

        return edges;
    }

    [Benchmark(Baseline = true)]
    public int PairwisePathWalk() =>
        NumberOfGoodPathsSolution.CountGoodPathsByPairwisePathWalk(_vals, _edges);

    [Benchmark]
    public int DisjointSetSweep() =>
        NumberOfGoodPathsSolution.CountGoodPathsByDisjointSetSweep(_vals, _edges);
}
