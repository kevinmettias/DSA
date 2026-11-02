using NodeStack = DSAExperimentation.DataStructures.Stack.Stack<
    DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.RootedTreeNode>;

namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

// A rooted tree laid out in pre-order - the layout LeetCode write-ups call an Euler tour,
// though a strict Euler tour lists a node once per edge it crosses and this lists each node
// once. A node takes the next position when the walk first reaches it, so two facts hold that
// the tree's own child lists cannot answer directly:
// - Every subtree is one contiguous run of positions starting at its root: "every node below
//   v" is the range SubtreeOf(v), and a whole-subtree update is a range update.
// - A node's parent sits at an earlier position than the node.
//
// Over RootedTreeNode, not over any ITreeTopology: every answer is looked up by a node's dense
// Id, which is what lets the layout be plain arrays read in O(1), and RootedTreeNode is the
// tree-tier node that carries one. The walk still reads children through RootedTreeTopology,
// the node's ITreeTopology witness, and leans on that witness's unique-ancestry promise: every
// node is reached exactly once, so no visited set is kept. Not over AdjacencyNode either: an
// undirected adjacency list promises no tree, so whoever can vouch for one roots it first -
// EdgeListTree's breadth-first pass to a parent array, then ParentArrayTree.Build - and hands
// this the root.
//
// What stays distinct from its neighbours. ParentArrayTree materializes a tree and
// RootedTreeNode answers "what is below v" one level at a time through its child list; this
// answers it as one range, at the price of an O(n) build after which nothing here changes.
// DepthFirstTraversal also visits in pre-order, but it observes a walk through hooks rather
// than keeping a layout, and it recurses - and LeetCode's trees run to chains of 10^5 nodes,
// deep enough to overflow the call stack. This walk keeps its own stack instead.
//
// Siblings are laid out last child first: a node's last child takes the position right after
// it. Any sibling order keeps subtrees contiguous, and no caller here depends on which one;
// this is the order the three hand-written walks this replaced used. Measured on LC 3515's
// workload, where a node's earlier children carry the larger subtrees, first-child-first left
// more siblings waiting on the stack - one more doubling of it, 0.28 KB at 200 nodes and
// 0.52 KB at 2,000.
/// <summary>A rooted tree laid out in pre-order, so each subtree is a contiguous run of positions.</summary>
public sealed class PreOrderTour
{
    private const string UnreachedNodesMessage =
        "Every id below nodeCount must be reachable from the root, each exactly once.";

    private readonly int[] _nodeAt;
    private readonly int[] _positionOf;
    private readonly int[] _parentOf;
    private readonly int[] _subtreeEndAt;

    /// <summary>How many nodes the tour lays out, which is also how many positions it has.</summary>
    public int NodeCount => _nodeAt.Length;

    private PreOrderTour(int[] nodeAt, int[] positionOf, int[] parentOf, int[] subtreeEndAt)
    {
        _nodeAt = nodeAt;
        _positionOf = positionOf;
        _parentOf = parentOf;
        _subtreeEndAt = subtreeEndAt;
    }

    // nodeCount is how many nodes hang from root, their ids 0..nodeCount-1 - RootedTreeNode's
    // dense ids, all in one tree. O(n), and iterative at any depth.
    /// <summary>Lays a tree out in pre-order in O(n), without recursing, so any depth is safe.</summary>
    /// <param name="root">The tree's root, which takes position 0.</param>
    /// <param name="nodeCount">How many nodes hang from the root, itself included, with ids 0 to nodeCount - 1.</param>
    /// <returns>The tour, with siblings laid out last child first.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when fewer than <paramref name="nodeCount"/> nodes hang from the root.
    /// </exception>
    public static PreOrderTour Build(RootedTreeNode root, int nodeCount)
    {
        var (nodeAt, parentOf) = WalkPreOrder(root, nodeCount);
        var positionOf = PositionsOf(nodeAt);
        var subtreeEndAt = SubtreeEnds(nodeAt, parentOf, positionOf);

        return new PreOrderTour(nodeAt, positionOf, parentOf, subtreeEndAt);
    }

    // A stack pre-order: a popped node takes the next position and pushes its children, which
    // all come off - with everything below them - before anything pushed earlier, so each
    // subtree's positions are contiguous. Ends once the stack drains: unique ancestry means
    // every node is pushed exactly once. Too large a nodeCount would leave positions no node
    // took, so that is refused rather than laid out as node 0.
    private static (int[] NodeAt, int[] ParentOf) WalkPreOrder(RootedTreeNode root, int nodeCount)
    {
        var nodeAt = new int[nodeCount];
        var parentOf = new int[nodeCount];
        var pending = new NodeStack();
        var nextPosition = 0;
        parentOf[root.Id] = ParentArrayTree.NoParent;
        pending.Push(root);

        while (pending.TryPop(out var node))
        {
            nodeAt[nextPosition] = node.Id;
            nextPosition++;
            PushChildren(node, parentOf, pending);
        }

        if (nextPosition != nodeCount)
        {
            throw new ArgumentException(UnreachedNodesMessage, nameof(nodeCount));
        }

        return (nodeAt, parentOf);
    }

    // In the topology's own order, so the last child is the next one popped.
    private static void PushChildren(RootedTreeNode node, int[] parentOf, NodeStack pending)
    {
        var children = RootedTreeTopology.GetChildren(node);

        for (var index = 0; index < children.Count; index++)
        {
            var child = children.Get(index);
            parentOf[child.Id] = node.Id;
            pending.Push(child);
        }
    }

    private static int[] PositionsOf(int[] nodeAt)
    {
        var positionOf = new int[nodeAt.Length];

        for (var position = 0; position < nodeAt.Length; position++)
        {
            positionOf[nodeAt[position]] = position;
        }

        return positionOf;
    }

    // A subtree ends where its last child's subtree ends, or at its own root when it has no
    // children. Walking positions backward reaches every node after everything below it, so
    // its end is final by then and is handed up to its parent, which sits earlier. The root,
    // at position 0, hands nothing up: its end is whatever its children handed it, or 0.
    private static int[] SubtreeEnds(int[] nodeAt, int[] parentOf, int[] positionOf)
    {
        var subtreeEndAt = new int[nodeAt.Length];

        for (var position = nodeAt.Length - 1; position > 0; position--)
        {
            var end = Math.Max(subtreeEndAt[position], position);
            var parentPosition = positionOf[parentOf[nodeAt[position]]];
            subtreeEndAt[position] = end;
            subtreeEndAt[parentPosition] = Math.Max(subtreeEndAt[parentPosition], end);
        }

        return subtreeEndAt;
    }

    /// <summary>Finds where a node sits in the tour.</summary>
    /// <param name="node">The node's id.</param>
    /// <returns>The node's 0-based position, 0 for the root and always later than its parent's.</returns>
    public int PositionOf(int node) => _positionOf[node];

    /// <summary>Finds which node sits at a position, undoing <see cref="PositionOf"/>.</summary>
    /// <param name="position">A 0-based position below <see cref="NodeCount"/>.</param>
    /// <returns>The id of the node at that position.</returns>
    public int NodeAt(int position) => _nodeAt[position];

    // The id of node's parent; negative for the root, which has none.
    /// <summary>Finds a node's parent.</summary>
    /// <param name="node">The node's id.</param>
    /// <returns>The id of the node's parent, or a negative value for the root, which has none.</returns>
    public int ParentOf(int node) => _parentOf[node];

    // The positions of node and of everything below it, both ends inclusive.
    /// <summary>Finds the run of positions that a node's subtree occupies.</summary>
    /// <param name="node">The id of the subtree's root.</param>
    /// <returns>The node's own position and the last its subtree reaches, both inclusive; equal for a leaf.</returns>
    public (int First, int Last) SubtreeOf(int node)
    {
        var first = _positionOf[node];

        return (first, _subtreeEndAt[first]);
    }
}
