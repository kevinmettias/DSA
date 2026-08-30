using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Construct Binary Tree from Preorder and Postorder Traversal (LC 889): a naive
// linear rescan of postorder to find each left-subtree's root index (O(n) per
// node, O(n^2) overall on a left-skewed input) vs. this repo's own
// HashMap<TValue,TIndex> giving that same lookup in O(1), the same
// ConstructBinaryTreeFromPreorderAndInorderTraversal/
// ConstructBinaryTreeFromInorderAndPostorderTraversal shape, keyed off postorder
// position instead of inorder position.
[MemoryDiagnoser]
public class ConstructBinaryTreeFromPreorderAndPostorderTraversalBenchmarks
{
    [Params(200, 2_000)]
    public int NodeCount;

    private int[] _preorder = null!;
    private int[] _postorder = null!;

    // A left-skewed full binary tree (every node's left child is the only child):
    // preorder walks root, root.Left, root.Left.Left, ... (descending values);
    // postorder walks the same chain bottom-up (ascending values). This is the
    // shape that makes LinearRescan's search distance - and so its O(n^2) blowup -
    // actually grow with n, the same "degenerate chain" framing
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
    public int LinearRescan() => Height(LinearRescanBuild(_preorder, _postorder));

    private static BinaryTreeNode<int>? LinearRescanBuild(int[] preorder, int[] postorder)
    {
        var pre = 0;

        BinaryTreeNode<int>? BuildRange(int postLow, int postHigh)
        {
            if (postLow > postHigh)
            {
                return null;
            }

            var node = new BinaryTreeNode<int>(preorder[pre++]);
            if (postLow == postHigh)
            {
                return node;
            }

            var leftRootValue = preorder[pre];
            var leftRootPostIndex = postLow;

            for (var i = postLow; i <= postHigh; i++)
            {
                if (postorder[i] == leftRootValue)
                {
                    leftRootPostIndex = i;
                    break;
                }
            }

            var leftSize = leftRootPostIndex - postLow + 1;
            node.Left = BuildRange(postLow, postLow + leftSize - 1);
            node.Right = BuildRange(postLow + leftSize, postHigh - 1);
            return node;
        }

        return BuildRange(0, postorder.Length - 1);
    }

    [Benchmark]
    public int HashMapIndexed() => Height(HashMapIndexedBuild(_preorder, _postorder));

    private static BinaryTreeNode<int>? HashMapIndexedBuild(int[] preorder, int[] postorder)
    {
        var postIndexOf = new HashMap<int, int>();
        for (var i = 0; i < postorder.Length; i++)
        {
            postIndexOf.Set(postorder[i], i);
        }

        var pre = 0;

        BinaryTreeNode<int>? BuildRange(int postLow, int postHigh)
        {
            if (postLow > postHigh)
            {
                return null;
            }

            var node = new BinaryTreeNode<int>(preorder[pre++]);
            if (postLow == postHigh)
            {
                return node;
            }

            postIndexOf.TryGetValue(preorder[pre], out var leftRootPostIndex);
            var leftSize = leftRootPostIndex - postLow + 1;

            node.Left = BuildRange(postLow, postLow + leftSize - 1);
            node.Right = BuildRange(postLow + leftSize, postHigh - 1);
            return node;
        }

        return BuildRange(0, postorder.Length - 1);
    }

    private static int Height(BinaryTreeNode<int>? node)
        => node is null ? 0 : 1 + Math.Max(Height(node.Left), Height(node.Right));
}
