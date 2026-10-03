using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.SearchInABinarySearchTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only. Both arms are SearchInABinarySearchTreeSolution's, the same
// methods SearchInABinarySearchTreeSolutionTests proves correct. Both build their own
// input from the same shuffled insertion order so tree height stays close to
// O(log n) instead of the degenerate O(n) ascending-insertion case, the same
// convention DeleteNodeInABSTBenchmarks already uses. The values are 1..NodeCount, as
// LC 700's start at 1, and NodeCount stops at its 5,000 nodes; the target is the highest.
public class SearchInABinarySearchTreeBenchmarks
{
    private BinaryTreeNode<int> _root = null!;

    private int _target;
    [Params(500, 5_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var values = SeededSequences.ShuffledOneTo(NodeCount, seed: 1);

        var tree = new BinarySearchTree<int>();

        foreach (var value in values)
        {
            tree.Insert(value);
        }

        // presumption: allow -- values always has NodeCount >= 1 entries above, so
        // at least one Insert ran and Root is never null here.
        _root = tree.Root!;
        _target = NodeCount;
    }

    // Each arm returns the found node, the root of LeetCode's answer subtree, as
    // object because BinaryTreeNode<int> is internal and a [Benchmark] method
    // must be public.
    [Benchmark(Baseline = true)]
    public object? LinearScan() =>
        SearchInABinarySearchTreeSolution.SearchBstByLinearScan(_root, _target);

    [Benchmark]
    public object? BinarySearchTreeDescent() =>
        SearchInABinarySearchTreeSolution.SearchBstByBstDescent(_root, _target);
}
