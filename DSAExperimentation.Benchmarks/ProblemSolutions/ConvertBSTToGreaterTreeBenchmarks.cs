using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Convert BST to Greater Tree (LC 538): a hand-rolled recursive reverse in-order walk
// (right, visit+accumulate, left) - the textbook single-pass solution - vs. this
// repo's own ascending-only InOrderTraversal/IInOrderHooks, composed into the same
// result via two ascending passes (collect values, then reassign a precomputed
// suffix sum) since InOrderTraversal is hardwired left-then-right with no reverse
// option (see its own doc comment). Not an asymptotic win for the composed version -
// both are O(n) time - the comparison instead surfaces the real cost of that missing
// traversal direction: a second array allocation and a second full walk, the same
// "genuinely-distinct walk mechanics" shape KthSmallestElementInABSTBenchmarks
// already uses. Each [Benchmark] clones the shared tree first (the RotateImage
// convention for a mutate-in-place problem) so repeated BenchmarkDotNet invocations
// each start from the same untransformed values instead of re-transforming an
// already-transformed tree.
[MemoryDiagnoser]
public class ConvertBSTToGreaterTreeBenchmarks
{
    [Params(500, 20_000)]
    public int NodeCount;

    private BinaryTreeNode<int> _root = null!;

    [GlobalSetup]
    public void Setup() => _root = BinaryTrees.Balanced(NodeCount);

    [Benchmark(Baseline = true)]
    public int ManualReverseInOrder()
    {
        var root = Clone(_root);
        _runningSum = 0;
        Visit(root);
        return root.Value;
    }

    [Benchmark]
    public int InOrderTraversalHooks()
    {
        var root = Clone(_root);

        State.Values.Value = [];
        InOrderTraversal.Walk<int, CollectHooks>(root);
        var ascending = State.Values.Value!;

        var suffixSums = new int[ascending.Count];
        var runningSum = 0;

        for (var i = ascending.Count - 1; i >= 0; i--)
        {
            runningSum += ascending[i];
            suffixSums[i] = runningSum;
        }

        State.Index.Value = 0;
        State.GreaterSums.Value = suffixSums;
        InOrderTraversal.Walk<int, AssignHooks>(root);

        return root.Value;
    }

    private int _runningSum;

    private void Visit(BinaryTreeNode<int>? node)
    {
        if (node is null)
        {
            return;
        }

        Visit(node.Right);
        _runningSum += node.Value;
        node.Value = _runningSum;
        Visit(node.Left);
    }

    private readonly struct CollectHooks : IInOrderHooks<int>
    {
        public static void Visit(BinaryTreeNode<int> node, int depth) => State.Values.Value!.Add(node.Value);
    }

    private readonly struct AssignHooks : IInOrderHooks<int>
    {
        public static void Visit(BinaryTreeNode<int> node, int depth)
        {
            node.Value = State.GreaterSums.Value![State.Index.Value];
            State.Index.Value++;
        }
    }

    private static class State
    {
        public static readonly AsyncLocal<List<int>?> Values = new();
        public static readonly AsyncLocal<int> Index = new();
        public static readonly AsyncLocal<int[]?> GreaterSums = new();
    }

    private static BinaryTreeNode<int> Clone(BinaryTreeNode<int> node) => new(node.Value)
    {
        Left = node.Left is null ? null : Clone(node.Left),
        Right = node.Right is null ? null : Clone(node.Right),
    };
}
