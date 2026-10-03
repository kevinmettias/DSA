using DSAExperimentation.LeetCode.ConstructBinaryTreeFromPreorderAndPostorderTraversal;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// ConstructBinaryTreeFromPreorderAndPostorderTraversalSolution's, the same methods
// ...Tests proves correct. The comparison is how each node's left-subtree root is
// located in postorder - a linear rescan of the live range against this repo's own
// HashMap<TValue,TIndex> built once up front. Each arm returns the rebuilt root as
// object?, since a public [Benchmark] method cannot name the internal
// BinaryTreeNode<int> (CS0050).
public class ConstructBinaryTreeFromPreorderAndPostorderTraversalBenchmarks
{
    private int[] _preorder = [];

    private int[] _postorder = [];
    // LC 889's traversals hold at most 30 nodes, valued 1..n.
    [Params(10, 30)]
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
    public object? LinearRescan() =>
        ConstructBinaryTreeFromPreorderAndPostorderTraversalSolution.BuildByPostorderScan(_preorder, _postorder);

    [Benchmark]
    public object? HashMapIndexed() =>
        ConstructBinaryTreeFromPreorderAndPostorderTraversalSolution.BuildByPostorderIndexMap(_preorder, _postorder);
}
