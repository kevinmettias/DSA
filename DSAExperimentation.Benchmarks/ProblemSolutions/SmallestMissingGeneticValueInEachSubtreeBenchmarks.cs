using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.SmallestMissingGeneticValueInEachSubtree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SmallestMissingGeneticValueInEachSubtreeSolution's, the
// same methods SmallestMissingGeneticValueInEachSubtreeTests proves correct. The
// workload is a chain-shaped family tree with genetic value 1 planted at the deepest
// leaf - the worst case for rescanning every node's subtree from scratch (O(n^2), each
// of the n nodes paying for its own DFS) and the best case for the ancestor-chain walk
// (O(n) total). Both arms are handed the RootedTreeNode[] their hoisted overloads take,
// so ParentArrayTree.Build is charged to [GlobalSetup] rather than to the arm.
[MemoryDiagnoser]
public class SmallestMissingGeneticValueInEachSubtreeBenchmarks
{
    private const int BaseGeneValue = 2;
    private const int SmallestGeneValue = 1;

    private int[] _parents = [];

    private int[] _nums = [];
    private RootedTreeNode[] _nodes = [];
    [Params(200, 2000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _parents = new int[NodeCount];
        _nums = new int[NodeCount];
        _parents[0] = -1;
        _nums[0] = BaseGeneValue;

        for (var i = 1; i < NodeCount; i++)
        {
            _parents[i] = i - 1;
            _nums[i] = i + BaseGeneValue;
        }

        _nums[NodeCount - 1] = SmallestGeneValue;
        _nodes = ParentArrayTree.Build(_parents);
    }

    [Benchmark(Baseline = true)]
    public int[] BruteForce() =>
        SmallestMissingGeneticValueInEachSubtreeSolution.SmallestMissingValuesBySubtreeRescan(_nodes, _nums);

    [Benchmark]
    public int[] AncestorChainWithSkip() =>
        SmallestMissingGeneticValueInEachSubtreeSolution.SmallestMissingValuesByAncestorChain(_nodes, _parents, _nums);
}
