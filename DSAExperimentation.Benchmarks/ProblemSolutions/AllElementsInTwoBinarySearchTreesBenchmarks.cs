using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.AllElementsInTwoBinarySearchTrees;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are AllElementsInTwoBinarySearchTreesSolution's, the same
// methods AllElementsInTwoBinarySearchTreesTests proves correct. CollectAllThenSort
// dumps every node from both trees into one list and sorts it - O((n+m) log(n+m)) -
// while InOrderTraversalMerge collects each tree's already-ascending sequence and
// two-pointer merges them in O(n+m). Both trees are built here with this repo's own
// BinarySearchTree<int>.Insert over a shuffled permutation, so insertion order stays
// randomized (no adversarial ascending-order degeneration) the same way
// FindModeInBinarySearchTreeBenchmarks' fixture is shuffled.
[MemoryDiagnoser]
public class AllElementsInTwoBinarySearchTreesBenchmarks
{
    private const int RandomSeedTree1 = 1305; // LeetCode problem number
    private const int RandomSeedTree2 = 1306;

    private BinaryTreeNode<int>? _root1;

    private BinaryTreeNode<int>? _root2;
    [Params(300, 5_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _root1 = BuildTree(seed: RandomSeedTree1);
        _root2 = BuildTree(seed: RandomSeedTree2);
    }

    private BinaryTreeNode<int>? BuildTree(int seed)
    {
        var values = Enumerable.Range(0, NodeCount).ToArray();
        var random = new Random(seed);

        for (var i = values.Length - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (values[i], values[j]) = (values[j], values[i]);
        }

        var tree = new BinarySearchTree<int>();
        foreach (var value in values)
        {
            tree.Insert(value);
        }

        return tree.Root;
    }

    [Benchmark(Baseline = true)]
    public int[] CollectAllThenSort() =>
        AllElementsInTwoBinarySearchTreesSolution.GetAllElementsByCollectThenSort(_root1, _root2);

    [Benchmark]
    public int[] InOrderTraversalMerge() =>
        AllElementsInTwoBinarySearchTreesSolution.GetAllElementsByInOrderMerge(_root1, _root2);
}
