using DSAExperimentation.Algorithms.Traversal.BreadthFirst;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.LeetCode.PopulatingNextRightPointersInEachNodeII;

namespace DSAExperimentation.LeetCode.PopulatingNextRightPointersInEachNode;

// LeetCode 116. Populating Next Right Pointers in Each Node: given a perfect binary
// tree, connect each node to its next right neighbor at the same depth (null for
// the rightmost node of each level). BinaryTreeNode<int> has no Next field of its
// own, so "populate" is represented as a node -> next-node map built from a
// level-by-level BFS - the two strategies differ only in how a level's boundary is
// discovered, the same split BinaryTreeLevelOrderTraversalSolution uses for LC 102.
//
// Only the manual-queue baseline is this class's own; the level-grouped arm is LC
// 117's, called through. #117 asks the same question of an arbitrary binary tree,
// so its class is the one implementation of that walk - the perfectness #116
// guarantees is not what makes the walk work (BinaryTreeChildren compacts null
// Left/Right slots away, so a missing child just yields a shorter level), which is
// why the same walk answers both problems unchanged.
internal static class PopulatingNextRightPointersInEachNodeSolution
{
    // Textbook baseline: BCL Queue + Dictionary, snapshotting Count at the top of
    // each iteration to know how many nodes belong to the level being drained.
    public static Dictionary<BinaryTreeNode<int>, BinaryTreeNode<int>?> ConnectByManualQueueBfs(
        BinaryTreeNode<int> root)
    {
        var next = new Dictionary<BinaryTreeNode<int>, BinaryTreeNode<int>?>();
        var queue = new Queue<BinaryTreeNode<int>>();
        queue.Enqueue(root);

        while (queue.Count > 0)
        {
            DrainLevel(queue, next);
        }

        return next;
    }

    // One level of the walk: exactly the nodes already queued are drained, each linked
    // to its left-hand neighbor on the way, with that level's children queued up behind
    // them; the level then closes with a null right neighbor on its last node.
    private static void DrainLevel(Queue<BinaryTreeNode<int>> queue, Dictionary<BinaryTreeNode<int>, BinaryTreeNode<int>?> next)
    {
        var levelSize = queue.Count;
        BinaryTreeNode<int>? previous = null;

        for (var i = 0; i < levelSize; i++)
        {
            var node = queue.Dequeue();
            previous = VisitNode(node, queue, next, previous);
        }

        next[previous!] = null;
    }

    // One node of a level: linked to the left-hand neighbor the level has seen so far,
    // then handing its own children to the back of the queue - and returned, so the
    // caller can carry it as the next node's left-hand neighbor.
    private static BinaryTreeNode<int> VisitNode(
        BinaryTreeNode<int> node,
        Queue<BinaryTreeNode<int>> queue,
        Dictionary<BinaryTreeNode<int>, BinaryTreeNode<int>?> next,
        BinaryTreeNode<int>? previous)
    {
        if (previous is not null)
        {
            next[previous] = node;
        }

        if (node.Left is not null)
        {
            queue.Enqueue(node.Left);
        }

        if (node.Right is not null)
        {
            queue.Enqueue(node.Right);
        }

        return node;
    }

    // This repo's own level-grouped BFS: LevelGroupedBreadthFirstTraversal buffers a
    // whole depth before firing, so the hook only has to link each buffered level's
    // nodes to their right neighbor and null the last one. LC 117's class holds the
    // one implementation of that pairing over an arbitrary tree; this arm calls it,
    // which is why this class needs no level hook of its own.
    public static HashMap<BinaryTreeNode<int>, BinaryTreeNode<int>?> ConnectByLevelGroupedTraversal(
        BinaryTreeNode<int> root) =>
        PopulatingNextRightPointersInEachNodeIISolution.ConnectByLevelGroupedTraversal(root);
}
