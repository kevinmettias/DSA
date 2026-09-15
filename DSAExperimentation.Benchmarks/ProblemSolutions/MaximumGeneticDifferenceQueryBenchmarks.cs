using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.MaximumGeneticDifferenceQuery;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumGeneticDifferenceQuerySolution's, the same
// methods MaximumGeneticDifferenceQueryTests proves correct.
//
// The workload is a straight chain (CheckIfDfsStringsArePalindromesBenchmarks' own
// precedent for "worst depth shape", built the same way via ParentArrayTree.Build
// over a hand-built chain parent[]), which forces the naive per-query ancestor climb
// to pay its full O(depth) worst case on every query instead of letting an early exit
// make it look artificially competitive - the same convention TwoSumBenchmarks' own
// doc comment states.
//
// The DFS arm is handed the prepared tree root its hoisted overload takes, so
// ParentArrayTree.Build is charged to [GlobalSetup] rather than to the search being
// measured; the baseline reads the parent array directly and so needs nothing built.
[MemoryDiagnoser]
public class MaximumGeneticDifferenceQueryBenchmarks
{
    private const int RandomSeed = 1;

    private int[] _parents = [];

    private RootedTreeNode _root = null!;
    private int[][] _queries = [];
    [Params(200, 2_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _parents = new int[NodeCount];
        _parents[0] = -1;

        for (var i = 1; i < NodeCount; i++)
        {
            _parents[i] = i - 1;
        }

        _root = ParentArrayTree.Build(_parents)[0];

        var random = new Random(RandomSeed);
        _queries = new int[NodeCount][];

        for (var i = 0; i < NodeCount; i++)
        {
            _queries[i] = [random.Next(NodeCount), random.Next(NodeCount)];
        }
    }

    [Benchmark(Baseline = true)]
    public int[] NaiveAncestorWalk() =>
        MaximumGeneticDifferenceQuerySolution.MaxGeneticDifferenceByAncestorWalk(_parents, _queries);

    [Benchmark]
    public int[] BitTrieOfflineDfs() =>
        MaximumGeneticDifferenceQuerySolution.MaxGeneticDifferenceByBitTrieDfs(_root, NodeCount, _queries);
}
