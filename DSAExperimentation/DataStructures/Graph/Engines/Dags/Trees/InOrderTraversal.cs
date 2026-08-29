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
    public static void Walk<TValue, THooks>(BinaryTreeNode<TValue>? root)
        where THooks : struct, IInOrderHooks<TValue>
    {
        if (root is null)
        {
            return;
        }

        Visit<TValue, THooks>(root, 0);
    }

    private static void Visit<TValue, THooks>(BinaryTreeNode<TValue> node, int depth)
        where THooks : struct, IInOrderHooks<TValue>
    {
        if (node.Left is not null)
        {
            Visit<TValue, THooks>(node.Left, depth + 1);
        }

        THooks.Visit(node, depth);

        if (node.Right is not null)
        {
            Visit<TValue, THooks>(node.Right, depth + 1);
        }
    }
}
