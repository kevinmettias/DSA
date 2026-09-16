using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.RemoveMaxNumberOfEdgesToKeepGraphFullyTraversable;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// RemoveMaxNumberOfEdgesToKeepGraphFullyTraversableSolution's, the same strategies
// RemoveMaxNumberOfEdgesToKeepGraphFullyTraversableTests proves correct. A spanning
// tree of type-3 edges is generated first so both traversers are always fully
// connected in the end (the interesting, non-trivial case), then extra random
// single-owner edges give both strategies real redundant-edge-rejecting work to do.
[MemoryDiagnoser]
public class RemoveMaxNumberOfEdgesToKeepGraphFullyTraversableBenchmarks
{
    private const int RandomSeed = 1579; // LC problem number
    private const int FirstNonRootNode = 2;
    private const int ExtraEdgeMultiplier = 2;
    private const int EdgeTypeUpperBoundExclusive = 4;
    private const int BothOwnersEdgeType = 3;

    private int[][] _edges = [];

    [Params(50, 300)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var edges = new List<int[]>();

        for (var i = FirstNonRootNode; i <= NodeCount; i++)
        {
            edges.Add([BothOwnersEdgeType, random.Next(1, i), i]);
        }

        for (var e = 0; e < NodeCount * ExtraEdgeMultiplier; e++)
        {
            var u = random.Next(1, NodeCount + 1);
            var v = random.Next(1, NodeCount + 1);

            if (u == v)
            {
                continue;
            }

            edges.Add([random.Next(1, EdgeTypeUpperBoundExclusive), u, v]);
        }

        _edges = [.. edges];
    }

    [Benchmark(Baseline = true)]
    public int BreadthFirstReachabilityCheck() =>
        RemoveMaxNumberOfEdgesToKeepGraphFullyTraversableSolution.MaxNumberOfEdgesToRemoveByFloodFill(
            NodeCount,
            _edges);

    [Benchmark]
    public int DisjointSetUnionFind() =>
        RemoveMaxNumberOfEdgesToKeepGraphFullyTraversableSolution.MaxNumberOfEdgesToRemoveByDisjointSet(
            NodeCount,
            _edges);
}
