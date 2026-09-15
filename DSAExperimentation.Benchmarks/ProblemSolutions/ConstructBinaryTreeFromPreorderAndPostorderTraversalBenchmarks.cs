using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.ConstructBinaryTreeFromPreorderAndPostorderTraversal;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// ConstructBinaryTreeFromPreorderAndPostorderTraversalSolution's, the same methods
// ...Tests proves correct. The comparison is how each node's left-subtree root is
// located in postorder - a linear rescan of the live range against this repo's own
// HashMap<TValue,TIndex> built once up front.
[MemoryDiagnoser]
public class ConstructBinaryTreeFromPreorderAndPostorderTraversalBenchmarks
{
    private int[] _preorder = [];

    private int[] _postorder = [];
    [Params(200, 2_000)]
    public int NodeCount { get; set; }

    // A left-skewed chain (every node's left child is its only child): preorder
    // walks root, root.Left, root.Left.Left, ... (descending values); postorder
    // walks the same chain bottom-up (ascending values). This is the shape that
    // makes the rescan's search distance - and so its O(n^2) blowup - actually grow
    // with n, the same "degenerate chain" framing
    // DiameterOfBinaryTreeBenchmarks/MaximumBinaryTreeBenchmarks already use.
    [GlobalSetup]
    public void Setup()
    {
        _preorder = new int[NodeCount];
        _postorder = new int[NodeCount];

        for (var i = 0; i < NodeCount; i++)
        {
            _preorder[i] = NodeCount - i;
            _postorder[i] = i + 1;
        }
    }

    [Benchmark(Baseline = true)]
    public int LinearRescan()
    {
        var root = ConstructBinaryTreeFromPreorderAndPostorderTraversalSolution.BuildByPostorderScan(
            _preorder, _postorder);

        return Height(root);
    }

    [Benchmark]
    public int HashMapIndexed()
    {
        var root = ConstructBinaryTreeFromPreorderAndPostorderTraversalSolution.BuildByPostorderIndexMap(
            _preorder, _postorder);

        return Height(root);
    }

    private static int Height(BinaryTreeNode<int>? node) =>
        node is null ? 0 : NodeHeight(node);

    // The node's own level on top of its taller subtree.
    private static int NodeHeight(BinaryTreeNode<int> node) =>
        1 + Math.Max(Height(node.Left), Height(node.Right));
}
