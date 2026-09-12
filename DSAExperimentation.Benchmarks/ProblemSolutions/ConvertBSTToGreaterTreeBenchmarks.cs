using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.ConvertBSTToGreaterTree;

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
    public int ManualReverseInOrder() => ConvertBSTToGreaterTreeSolution.ConvertByReverseInOrder(Clone(_root))!.Value;

    [Benchmark]
    public int InOrderTraversalHooks() => ConvertBSTToGreaterTreeSolution.ConvertByInOrderHooks(Clone(_root))!.Value;

    private static BinaryTreeNode<int> Clone(BinaryTreeNode<int> node) => new(node.Value)
    {
        Left = node.Left is null ? null : Clone(node.Left),
        Right = node.Right is null ? null : Clone(node.Right),
    };
}
