using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.LinkedListInBinaryTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LinkedListInBinaryTreeSolution's, the same methods
// LinkedListInBinaryTreeTests proves correct. Each is handed the prepared pattern
// its hoisted overload takes - a flattened int[] for the array-slice walk, the
// SinglyLinkedListNode chain for the node walk - so building the needle is charged
// to [GlobalSetup] rather than to the search being measured. The tree is a skewed
// chain whose first half matches the needle before it deliberately fails, forcing
// real recursion depth (and, for the array variant, real slicing) instead of
// failing out on the first comparison.
[MemoryDiagnoser]
public class LinkedListInBinaryTreeBenchmarks
{
    private BinaryTreeNode<int> _root = null!;

    private int[] _headValues = [];
    private SinglyLinkedListNode<int> _head = null!;
    [Params(200, 2_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _root = BinaryTrees.Skewed(NodeCount);
        _headValues = LinkedListInBinaryTreeWorkloads.BuildNeedleValues(NodeCount);
        _head = LinkedListInBinaryTreeWorkloads.BuildNeedle(_headValues);
    }

    [Benchmark(Baseline = true)]
    public bool IsSubPathByArraySliceWalk() => LinkedListInBinaryTreeSolution.IsSubPathByArraySliceWalk(_headValues, _root);

    [Benchmark]
    public bool IsSubPathByLinkedNodeWalk() => LinkedListInBinaryTreeSolution.IsSubPathByLinkedNodeWalk(_head, _root);
}
