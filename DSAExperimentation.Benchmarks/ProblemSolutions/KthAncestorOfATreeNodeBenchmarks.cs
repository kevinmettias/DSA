using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures;
using DSAExperimentation.LeetCode.KthAncestorOfATreeNode;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are KthAncestorOfATreeNodeSolution's, the same methods
// KthAncestorOfATreeNodeTests proves correct - the textbook "walk the raw parent[]
// array k times" per query against this repo's ITopDownHooks-driven precompute,
// which fills in every node's ancestor chain in one pass so each query afterward
// is an O(1) index.
//
// The precompute deliberately stays inside the measured arm rather than moving to
// [GlobalSetup]: what this comparison is about is whether one O(n log n) pass pays
// for itself across the query batch, and hoisting it would measure only the index.
// The parent array and the query batch - the workload's sizing and seeding - are
// what [GlobalSetup] builds.
//
// The tree is heap-shaped (parent(i) = (i-1)/2) rather than a chain: a chain would
// make node i's ancestor array length i, so the precompute itself would be O(n^2)
// and no repo-only technique could beat the O(1)-space naive walk on it. A
// heap-shaped tree keeps every ancestor array at O(log n), which is also why the
// query batch below is large.
[MemoryDiagnoser]
public class KthAncestorOfATreeNodeBenchmarks
{
    private const int RandomSeed = 1483; // LC problem number
    private const int QueryCount = 1_000_000;
    private const int RootParent = -1;

    private int[] _parent = [];

    private (int Node, int K)[] _queries = [];
    [Params(2_000, 20_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _parent = new int[NodeCount];
        _parent[0] = RootParent;

        for (var i = 1; i < NodeCount; i++)
        {
            _parent[i] = (i - 1) / AlgorithmConstants.BranchingFactor;
        }

        _queries = Enumerable.Range(0, QueryCount)
            .Select(_ => (Node: random.Next(NodeCount), K: random.Next(1, NodeCount)))
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public long WalkParentArrayPerQuery()
    {
        var total = 0L;

        foreach (var (node, k) in _queries)
        {
            total += KthAncestorOfATreeNodeSolution.GetKthAncestorByParentWalk(_parent, node, k);
        }

        return total;
    }

    [Benchmark]
    public long PrecomputedAncestorChains()
    {
        var chains = KthAncestorOfATreeNodeSolution.BuildAncestorChains(_parent);
        var total = 0L;

        foreach (var (node, k) in _queries)
        {
            total += KthAncestorOfATreeNodeSolution.GetKthAncestorByAncestorChains(chains, node, k);
        }

        return total;
    }
}
