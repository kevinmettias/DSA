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
    [Params(200, 2_000)]
    public int NodeCount;

    private int[] _preorder = null!;
    private int[] _postorder = null!;

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
    public int LinearRescan() =>
        Height(ConstructBinaryTreeFromPreorderAndPostorderTraversalSolution.BuildByPostorderScan(
            _preorder, _postorder));

    [Benchmark]
    public int HashMapIndexed() =>
        Height(ConstructBinaryTreeFromPreorderAndPostorderTraversalSolution.BuildByPostorderIndexMap(
            _preorder, _postorder));

    private static int Height(BinaryTreeNode<int>? node) =>
        node is null ? 0 : 1 + Math.Max(Height(node.Left), Height(node.Right));
}
