using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.IsGraphBipartite;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are IsGraphBipartiteSolution's, the same methods
// IsGraphBipartiteSolutionTests proves correct - a hand-rolled iterative DFS 2-coloring
// over the problem's own int[][] adjacency (a plain sbyte[] color array, an
// explicit Stack<int>) against this repo's BipartiteCheck.IsBipartite, a
// multi-root BFS 2-coloring composed from IGraphTopology/ListChildren/
// NaturalChildOrder with a Dictionary<TNode,bool> color map. Both walk every
// node/edge exactly once at O(V+E); the split under [MemoryDiagnoser] is the
// dictionary/heap-object overhead the composed primitive pays for its generality
// against the raw array baseline. The generated graph (IsGraphBipartiteWorkloads)
// is genuinely bipartite - every edge crosses a fixed A/B split - so neither
// strategy short-circuits on an early color conflict; both are forced through their
// full worst-case walk.
//
// Materializing the BipartiteNode graph is input construction, so it is charged
// to [GlobalSetup] and handed to the strategy's prepared-input overload.
//
// NodeCount stops at LC 785's 100-node cap.
public class IsGraphBipartiteBenchmarks
{
    private const int RandomSeed = 1;

    private int[][] _adjacency = [];

    private BipartiteGraph _graph = null!;
    [Params(10, 100)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _adjacency = IsGraphBipartiteWorkloads.BuildAdjacency(NodeCount, RandomSeed);
        _graph = BipartiteGraph.Build(_adjacency);
    }

    [Benchmark(Baseline = true)]
    public bool IsBipartiteByColorArrayDfs() => IsGraphBipartiteSolution.IsBipartiteByColorArrayDfs(_adjacency);

    [Benchmark]
    public bool IsBipartiteByBipartiteCheck() => IsGraphBipartiteSolution.IsBipartiteByBipartiteCheck(_graph);
}
