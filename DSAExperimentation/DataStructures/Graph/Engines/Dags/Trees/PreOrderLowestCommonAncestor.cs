using ParentPositionTree = DSAExperimentation.DataStructures.SegmentTree.SegmentTree<
    int, DSAExperimentation.DataStructures.ElementAlgebra.MinOperation<int>>;

namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

// Lowest common ancestors read off a PreOrderTour: O(n) to prepare, then O(log n) per query,
// with no recursion deeper than the segment tree's O(log n).
//
// It sits beside LowestCommonAncestor.Find instead of replacing it, because the two are
// different algorithms with different cost models, and each is the right one somewhere:
// - Find answers one query with no preparation, searching down from the root: O(n) per call,
//   recursing as deep as the tree is tall, over any ITreeTopology - binary trees and tries
//   included - and handing back the node itself.
// - This pays O(n) once so that every later query costs O(log n), over RootedTreeNode's dense
//   ids, because the tour it reads is laid out by id. For a single query that preparation is
//   pure overhead; for LeetCode's 5 * 10^4 queries on a 5 * 10^4-node chain, Find would be
//   quadratic and 5 * 10^4 frames deep.
// So Find and its callers are left as they are. Find lives in Algorithms/Ancestry because it is
// generic over a capability interface; this is hardwired to one concrete type, PreOrderTour, so
// it lives beside that type instead (ARCHITECTURE 13.5).
//
// The technique: for two distinct nodes at positions p < q, every node at a position in
// (p, q] lies strictly below their lowest common ancestor, and the ancestor's child on the way
// to the later node is among them. So the smallest parent position over (p, q] is the
// ancestor's own position - one range minimum over this repo's SegmentTree<int,
// MinOperation<int>>. When the earlier node is itself the ancestor, its child on the way to the
// later node is in the range and nothing in it has a smaller parent position, so the same
// minimum names it.
internal sealed class PreOrderLowestCommonAncestor
{
    private readonly PreOrderTour _tour;
    private readonly ParentPositionTree _parentPositions;

    public PreOrderLowestCommonAncestor(PreOrderTour tour)
    {
        _tour = tour;
        _parentPositions = new ParentPositionTree(ParentPositions(tour));
    }

    // Node ids in, a node id out, in either argument order.
    public int Find(int first, int second)
    {
        if (first == second)
        {
            return first;
        }

        var firstPosition = _tour.PositionOf(first);
        var secondPosition = _tour.PositionOf(second);
        var earlier = Math.Min(firstPosition, secondPosition);
        var later = Math.Max(firstPosition, secondPosition);
        var ancestorPosition = _parentPositions.Query(earlier + 1, later);

        return _tour.NodeAt(ancestorPosition);
    }

    // At each position, the position of that node's parent. The root's slot keeps 0, its own
    // position: no query reads it, since a query's range starts one past the earlier of two
    // positions.
    private static int[] ParentPositions(PreOrderTour tour)
    {
        var parentPositionAt = new int[tour.NodeCount];

        for (var position = 1; position < tour.NodeCount; position++)
        {
            var parent = tour.ParentOf(tour.NodeAt(position));
            parentPositionAt[position] = tour.PositionOf(parent);
        }

        return parentPositionAt;
    }
}
