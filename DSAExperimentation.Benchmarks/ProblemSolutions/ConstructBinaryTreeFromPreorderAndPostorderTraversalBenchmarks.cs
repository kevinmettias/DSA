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
    public int LinearRescan()
    {
        var tree = LinearRescanBuild(_preorder, _postorder);
        return Height(tree);
    }

    private static BinaryTreeNode<int>? LinearRescanBuild(int[] preorder, int[] postorder)
    {
        var arrays = new TraversalArrays(preorder, postorder);
        var pre = 0;
        return LinearRescanBuildRange(arrays, ref pre, 0, postorder.Length - 1);
    }

    private static BinaryTreeNode<int>? LinearRescanBuildRange(
        TraversalArrays arrays, ref int pre, int postLow, int postHigh)
    {
        if (postLow > postHigh)
        {
            return null;
        }

        var node = new BinaryTreeNode<int>(arrays.Preorder[pre++]);
        if (postLow == postHigh)
        {
            return node;
        }

        var leftSize = FindLeftSubtreeSizeByScan(arrays, pre, postLow, postHigh);
        node.Left = LinearRescanBuildRange(arrays, ref pre, postLow, postLow + leftSize - 1);
        node.Right = LinearRescanBuildRange(arrays, ref pre, postLow + leftSize, postHigh - 1);
        return node;
    }

    private static int FindLeftSubtreeSizeByScan(TraversalArrays arrays, int pre, int postLow, int postHigh)
    {
        var leftRootValue = arrays.Preorder[pre];
        var leftRootPostIndex = postLow;

        for (var i = postLow; i <= postHigh; i++)
        {
            if (arrays.Postorder[i] == leftRootValue)
            {
                leftRootPostIndex = i;
                break;
            }
        }

        return leftRootPostIndex - postLow + 1;
    }

    [Benchmark]
    public int HashMapIndexed()
    {
        var tree = HashMapIndexedBuild(_preorder, _postorder);
        return Height(tree);
    }

    private static BinaryTreeNode<int>? HashMapIndexedBuild(int[] preorder, int[] postorder)
    {
        var postIndexOf = IndexPostorderPositions(postorder);
        var indexed = new IndexedTraversal(preorder, postIndexOf);
        var pre = 0;
        return HashMapIndexedBuildRange(indexed, ref pre, 0, postorder.Length - 1);
    }

    private static HashMap<int, int> IndexPostorderPositions(int[] postorder)
    {
        var postIndexOf = new HashMap<int, int>();
        for (var i = 0; i < postorder.Length; i++)
        {
            postIndexOf.Set(postorder[i], i);
        }

        return postIndexOf;
    }

    private static BinaryTreeNode<int>? HashMapIndexedBuildRange(
        IndexedTraversal indexed, ref int pre, int postLow, int postHigh)
    {
        if (postLow > postHigh)
        {
            return null;
        }

        var node = new BinaryTreeNode<int>(indexed.Preorder[pre++]);
        if (postLow == postHigh)
        {
            return node;
        }

        indexed.PostIndexOf.TryGetValue(indexed.Preorder[pre], out var leftRootPostIndex);
        var leftSize = leftRootPostIndex - postLow + 1;

        node.Left = HashMapIndexedBuildRange(indexed, ref pre, postLow, postLow + leftSize - 1);
        node.Right = HashMapIndexedBuildRange(indexed, ref pre, postLow + leftSize, postHigh - 1);
        return node;
    }

    private static int Height(BinaryTreeNode<int>? node)
        => node is null ? 0 : 1 + Math.Max(Height(node.Left), Height(node.Right));

    private readonly record struct TraversalArrays(int[] Preorder, int[] Postorder);

    private readonly record struct IndexedTraversal(int[] Preorder, HashMap<int, int> PostIndexOf);
}
