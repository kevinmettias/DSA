using DSAExperimentation.Algorithms.Traversal.TopDown;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.KthAncestorOfATreeNode;

// LeetCode 1483. Kth Ancestor of a Tree Node: given a rooted tree as a parent
// array, answer repeated "what is node's kth ancestor" queries, reporting -1 when
// the chain runs out before k steps.
//
// Every Graph-domain topology contract here (ITreeTopology included) is child-ward
// only - GetChildren, never GetParent - so the composed strategy cannot walk
// ancestors live off the topology and precomputes them instead.
internal static class KthAncestorOfATreeNodeSolution
{
    // Marks the root in LeetCode's parent-array encoding, and the answer once a
    // walk steps off the top of the tree.
    private const int NoParent = LeetCodeAnswer.None;

    // The textbook baseline this composition has to justify itself against: step
    // up the raw parent array k times, O(k) per query and no precompute at all.
    // Deliberately written without this repo's primitives.
    public static int GetKthAncestorByParentWalk(int[] parent, int node, int k)
    {
        var current = node;

        for (var step = 0; step < k; step++)
        {
            if (current == NoParent)
            {
                return LeetCodeAnswer.None;
            }

            current = parent[current];
        }

        return current;
    }

    // The composed answer, in LeetCode's own shape: one query against a table this
    // call builds. A caller answering many queries builds the table once with
    // BuildAncestorChains and uses the overload below.
    public static int GetKthAncestorByAncestorChains(int[] parent, int node, int k) =>
        GetKthAncestorByAncestorChains(BuildAncestorChains(parent), node, k);

    // The hoisted overload (ARCHITECTURE.md §17.4): the chains are already built,
    // so all that is charged here is the O(1) index the precompute bought.
    public static int GetKthAncestorByAncestorChains(AncestorChains chains, int node, int k)
    {
        var ancestors = chains.Of(node);

        return k <= ancestors.Length ? ancestors[^k] : LeetCodeAnswer.None;
    }

    // Materializes the child-ward tree TopDownTraversal needs from LeetCode's
    // parent array, then walks it once to fill in every node's root-to-parent
    // chain. TopDownTraversal.Walk is the inherited-attribute primitive
    // Algorithms.Paths.AllRootToLeafPaths uses to thread "the path so far" down a
    // root-to-node walk: Descend appends the parent onto its own chain to hand
    // each child, so by the time Visit fires at a node its full chain is already
    // in state, ready to store.
    public static AncestorChains BuildAncestorChains(int[] parent)
    {
        var nodes = ParentArrayTree.Build(parent);
        var ancestorsById = new int[parent.Length][];

        TopDownTraversal.Walk<
            RootedTreeNode, RootedTreeTopology, ListChildren<RootedTreeNode>,
            NaturalChildOrder<RootedTreeNode, ListChildren<RootedTreeNode>>, ListChildren<RootedTreeNode>,
            CollectAncestorIdsHooks, (int[] Ancestors, int[][] AncestorsById)>(
            nodes[0], ([], ancestorsById));

        return new AncestorChains(ancestorsById);
    }

    private readonly struct CollectAncestorIdsHooks
        : ITopDownHooks<RootedTreeNode, (int[] Ancestors, int[][] AncestorsById)>
    {
        public static void Visit(
            RootedTreeNode node,
            (int[] Ancestors, int[][] AncestorsById) state,
            int depth,
            NodePosition position)
            => state.AncestorsById[node.Id] = state.Ancestors;

        public static (int[] Ancestors, int[][] AncestorsById) Descend(
            RootedTreeNode parent,
            (int[] Ancestors, int[][] AncestorsById) parentState,
            RootedTreeNode child)
        {
            var ancestors = new int[parentState.Ancestors.Length + 1];
            Array.Copy(parentState.Ancestors, ancestors, parentState.Ancestors.Length);
            ancestors[^1] = parent.Id;

            return (ancestors, parentState.AncestorsById);
        }
    }
}
