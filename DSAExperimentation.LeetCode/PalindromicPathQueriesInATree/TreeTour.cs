using NodeStack = DSAExperimentation.DataStructures.Stack.Stack<int>;
using ParentPositionTree = DSAExperimentation.DataStructures.SegmentTree.SegmentTree<
    int, DSAExperimentation.DataStructures.ElementAlgebra.MinOperation<int>>;

namespace DSAExperimentation.LeetCode.PalindromicPathQueriesInATree;

// LC 3841's tree, rooted at node 0 and laid out in pre-order: the part of the input
// no command changes, built once so a benchmark charges it to [GlobalSetup]
// (ARCHITECTURE.md 17.4). It answers this problem's two structural questions and
// nothing else, so it lives beside the solution rather than in Domain/ (17.3).
//
// Two facts about a pre-order carry the composed strategy:
// - Every subtree is one contiguous run of positions, starting at its root, so
//   "every node below v" is a range: SubtreeOf(v).
// - For two distinct nodes at positions p < q, every node at a position in (p, q]
//   lies strictly below their lowest common ancestor, and the ancestor's child on
//   the way to the later node is among them. So the smallest parent position over
//   (p, q] is the ancestor's own position - one range minimum over this repo's
//   SegmentTree<int, MinOperation<int>>, O(log n).
//
// Algorithms/Ancestry's LowestCommonAncestor.Find is not reused for the second:
// it searches down from the root on every call, O(n) per query, and recurses as
// deep as the tree is tall - up to 5 * 10^4 frames on a chain LeetCode allows.
internal sealed class TreeTour
{
    private const int Root = 0;
    private const int NoParent = -1;

    private readonly int[] _nodeAt;
    private readonly int[] _positionOf;
    private readonly int[] _subtreeEndAt;
    private readonly ParentPositionTree _parentPositions;

    private TreeTour(int[] nodeAt, int[] positionOf, int[] subtreeEndAt, ParentPositionTree parentPositions)
    {
        _nodeAt = nodeAt;
        _positionOf = positionOf;
        _subtreeEndAt = subtreeEndAt;
        _parentPositions = parentPositions;
    }

    // edges[] is undirected; the walk itself roots the tree at node 0.
    public static TreeTour Build(int nodeCount, int[][] edges)
    {
        var walk = WalkPreOrder(nodeCount, edges);
        var positions = PositionsOf(walk.Order);
        var parentPositionAt = ParentPositions(walk.ParentOf, walk.Order, positions);
        var subtreeEnds = SubtreeEnds(parentPositionAt);

        return new TreeTour(walk.Order, positions, subtreeEnds, new ParentPositionTree(parentPositionAt));
    }

    // A stack pre-order: a popped node takes the next position and pushes its
    // children, which are all popped - with everything below them - before anything
    // pushed earlier, so each subtree's positions are contiguous. On a tree a node's
    // only neighbor that is not its child is its parent, so no visited set is
    // needed. Ends once the stack drains, every node having been pushed exactly once.
    private static (int[] ParentOf, int[] Order) WalkPreOrder(int nodeCount, int[][] edges)
    {
        var neighbors = NeighborLists(nodeCount, edges);
        var parentOf = new int[nodeCount];
        var order = new int[nodeCount];
        var nextPosition = 0;
        var pending = new NodeStack();
        parentOf[Root] = NoParent;
        pending.Push(Root);

        while (pending.TryPop(out var node))
        {
            order[nextPosition] = node;
            nextPosition++;
            PushChildren(neighbors[node], node, parentOf, pending);
        }

        return (parentOf, order);
    }

    // Each node's neighbors as a plain id list - LeetCodeAdjacency's layout, with the
    // far endpoint's id as the only thing a slot records.
    private static List<int>[] NeighborLists(int nodeCount, int[][] edges) =>
        LeetCodeAdjacency.ZeroBased<List<int>>(nodeCount, edges, _ => [], (slot, farId, _, _) => slot.Add(farId));

    private static void PushChildren(List<int> neighbors, int node, int[] parentOf, NodeStack pending)
    {
        foreach (var neighbor in neighbors)
        {
            if (neighbor == parentOf[node])
            {
                continue;
            }

            parentOf[neighbor] = node;
            pending.Push(neighbor);
        }
    }

    private static int[] PositionsOf(int[] order)
    {
        var positions = new int[order.Length];

        for (var position = 0; position < order.Length; position++)
        {
            positions[order[position]] = position;
        }

        return positions;
    }

    // At each position, the position of that node's parent. The root's slot keeps
    // its own position, 0: no ancestor query reads it, since a query range starts
    // one past the earlier of two positions.
    private static int[] ParentPositions(int[] parentOf, int[] order, int[] positions)
    {
        var parentPositionAt = new int[order.Length];

        for (var position = 1; position < order.Length; position++)
        {
            var parentNode = parentOf[order[position]];
            parentPositionAt[position] = positions[parentNode];
        }

        return parentPositionAt;
    }

    // Every subtree ends at least at its own root, and otherwise where its last
    // child's subtree ends. Children sit after their parent, so a backward pass
    // over positions finishes each child's end before handing it up to the parent.
    private static int[] SubtreeEnds(int[] parentPositionAt)
    {
        var subtreeEnds = Enumerable.Range(0, parentPositionAt.Length).ToArray();

        for (var position = parentPositionAt.Length - 1; position >= 1; position--)
        {
            var parentPosition = parentPositionAt[position];
            subtreeEnds[parentPosition] = Math.Max(subtreeEnds[parentPosition], subtreeEnds[position]);
        }

        return subtreeEnds;
    }

    public int PositionOf(int node) => _positionOf[node];

    // The positions of node and of everything below it, both ends inclusive.
    public (int First, int Last) SubtreeOf(int node)
    {
        var first = _positionOf[node];

        return (first, _subtreeEndAt[first]);
    }

    public int LowestCommonAncestor(int first, int second)
    {
        if (first == second)
        {
            return first;
        }

        var earlier = Math.Min(_positionOf[first], _positionOf[second]);
        var later = Math.Max(_positionOf[first], _positionOf[second]);
        var ancestorPosition = _parentPositions.Query(earlier + 1, later);

        return _nodeAt[ancestorPosition];
    }
}
