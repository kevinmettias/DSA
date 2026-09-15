using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.FindModeInBinarySearchTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindModeInBinarySearchTreeSolution's, the same
// methods FindModeInBinarySearchTreeTests proves correct.
[MemoryDiagnoser]
public class FindModeInBinarySearchTreeBenchmarks
{
    // Average node count per distinct value, so the tree has realistic duplicate runs.
    private const int NodesPerDistinctValue = 20;

    // LeetCode problem number for Find Mode in Binary Search Tree.
    private const int RandomSeed = 501;

    private BinaryTreeNode<int>? _root;

    [Params(500, 20_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup() => _root = FindModeInBinarySearchTreeWorkloads.BuildTree(NodeCount, NodesPerDistinctValue, RandomSeed);

    [Benchmark(Baseline = true)]
    public int[] HashMapFrequencyCount() =>
        FindModeInBinarySearchTreeSolution.FindModeByHashMapFrequencyCount(_root);

    [Benchmark]
    public int[] InOrderTraversalStreak() =>
        FindModeInBinarySearchTreeSolution.FindModeByInOrderTraversalStreak(_root);
}
