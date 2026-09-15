using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.KthSmallestPathXORSum;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are KthSmallestPathXORSumSolution's, the same methods
// KthSmallestPathXORSumTests proves correct.
//
// The workload is a straight chain (CheckIfDfsStringsArePalindromesBenchmarks'
// own precedent for "worst subtree-size shape") queried many times over a
// handful of repeated nodes: the per-query-walk arm redoes its full O(n) XOR
// pass every single time, while the Euler-tour-cache arm pays that cost once
// per DISTINCT node queried, so repeats are what makes the two arms diverge.
[MemoryDiagnoser]
public class KthSmallestPathXORSumBenchmarks
{
    private const int Seed = 3590;
    private const int ValueUpperBoundExclusive = 100_000;
    private const int QueryCount = 200;

    private RootedTreeNode[] _nodes = [];

    private int[] _par = [];
    private int[] _vals = [];
    private int[][] _queries = [];
    [Params(200, 2_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);

        BuildParentChain();
        BuildNodesAndValues(random);
        BuildQueries(random);
    }

    // A straight chain: every node's parent is the one before it.
    private void BuildParentChain()
    {
        _par = new int[NodeCount];
        _par[0] = -1;

        for (var i = 1; i < NodeCount; i++)
        {
            _par[i] = i - 1;
        }
    }

    private void BuildNodesAndValues(Random random)
    {
        _nodes = ParentArrayTree.Build(_par);
        _vals = new int[NodeCount];

        for (var i = 0; i < NodeCount; i++)
        {
            _vals[i] = random.Next(0, ValueUpperBoundExclusive);
        }
    }

    private void BuildQueries(Random random)
    {
        _queries = new int[QueryCount][];

        for (var i = 0; i < QueryCount; i++)
        {
            var u = random.Next(0, NodeCount);
            var k = random.Next(1, NodeCount + 1);
            _queries[i] = [u, k];
        }
    }

    [Benchmark(Baseline = true)]
    public int[] PerQueryWalk() =>
        KthSmallestPathXORSumSolution.KthSmallestXorSumByPerQueryWalk(_nodes, _vals, _queries);

    [Benchmark]
    public int[] EulerTourCache() =>
        KthSmallestPathXORSumSolution.KthSmallestXorSumByEulerTourCache(_nodes, _par, _vals, _queries);
}
