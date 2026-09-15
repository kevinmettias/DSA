using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.MinimizeMaximumComponentCost;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimizeMaximumComponentCostSolution's, the same
// methods MinimizeMaximumComponentCostTests proves correct. The Kruskal arm is
// handed a pre-built ComponentGraph so node/adjacency construction is charged to
// [GlobalSetup] rather than to the merge walk being measured; the union-find
// baseline gets LeetCode's own (n, edges, k) shape directly since sorting is
// part of the textbook cost it exists to represent, not setup to hoist away.
// Edges are reused from EdgeWeightGraphWorkloads (built for LC 3419) rather than
// a second random-connected-weighted-graph generator, since it already produces
// exactly this shape: int[][] edges guaranteed connected via a back-edge per
// node, plus extra edges for density.
[MemoryDiagnoser]
public class MinimizeMaximumComponentCostBenchmarks
{
    private const int EdgeSeed = 3613;
    private const int ExtraEdgesPerNode = 2;
    private const int ComponentTarget = 4;

    private int[][] _edges = [];

    private ComponentGraph _graph = null!;
    [Params(100, 2000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _edges = EdgeWeightGraphWorkloads.BuildEdges(NodeCount, ExtraEdgesPerNode, EdgeSeed);
        _graph = ComponentGraph.Build(NodeCount, _edges);
    }

    [Benchmark(Baseline = true)]
    public int UnionFindSort() =>
        MinimizeMaximumComponentCostSolution.MinCostByUnionFind(NodeCount, _edges, ComponentTarget);

    [Benchmark]
    public int KruskalMst() =>
        MinimizeMaximumComponentCostSolution.MinCostByKruskalMst(_graph, ComponentTarget);
}
