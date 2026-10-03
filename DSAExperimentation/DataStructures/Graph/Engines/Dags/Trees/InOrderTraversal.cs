namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

// Hardwired directly to BinaryTreeNode<TValue>.Left/Right, deliberately NOT generic
// over ITreeTopology<TNode,TChildren> the way DepthFirstTraversal/TopDownTraversal/
// LevelGroupedBreadthFirstTraversal are - see BinaryTreeChildren's own doc comment
// for why a compacted IChildren view can't preserve the left/right positional
// identity in-order actually needs. Per ARCHITECTURE.md §5 step 7 / §13.5, an
// Operations type hardwired to exactly one concrete Representation with no
// substitutable interface co-locates with that Representation under
// DataStructures/, the same reasoning that keeps Heap.cs beside HeapArray.cs -
// hence this file lives here, not under Algorithms/Traversal/.
//
// No graph-tier counterpart (no WalkGraph, no visited-tracking): in-order has no
// defined meaning without unique ancestry, and ITreeTopology's promise - the only
// thing that would have justified a guarded tier - is exactly what this bypasses.
internal static class InOrderTraversal
{
    public static THooks Walk<TValue, THooks>(BinaryTreeNode<TValue>? root, THooks hooks)
        where THooks : struct, IInOrderHooks<TValue>
    {
        if (root is not null)
        {
            Visit(root, 0, ref hooks);
        }

        return hooks;
    }

    // The hook travels by reference, so every visit acts on the one value Walk hands back rather
    // than on a copy each frame would pass down and lose.
    private static void Visit<TValue, THooks>(BinaryTreeNode<TValue> node, int depth, ref THooks hooks)
        where THooks : struct, IInOrderHooks<TValue>
    {
        if (node.Left is not null)
        {
            Visit(node.Left, depth + 1, ref hooks);
        }

        hooks.Visit(node, depth);

        if (node.Right is not null)
        {
            Visit(node.Right, depth + 1, ref hooks);
        }
    }
}
