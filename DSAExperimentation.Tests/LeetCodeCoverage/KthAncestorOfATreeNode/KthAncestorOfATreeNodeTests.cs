using DSAExperimentation.Algorithms.Traversal.TopDown;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.LeetCodeCoverage.KthAncestorOfATreeNode.Fixtures;

namespace DSAExperimentation.Tests.LeetCodeCoverage.KthAncestorOfATreeNode;

// LeetCode 1483. Kth Ancestor of a Tree Node: every Graph-domain topology contract
// (ITreeTopology included) is child-ward only - GetChildren, never GetParent - so
// answering "kth ancestor" needs the ancestor chain precomputed, not walked live off
// the topology. TopDownTraversal.Walk with a custom ITopDownHooks (the same
// inherited-attribute primitive Algorithms.Paths.AllRootToLeafPaths uses to thread
// "the path so far" down a root-to-node walk) does exactly that: Descend appends the
// parent onto its own ancestor chain to hand each child, so by the time Visit fires
// at any node its full root-to-parent chain is already sitting in state, ready to
// store. getKthAncestor then becomes an O(1) index into that stored chain instead of
// an O(k) walk.
public sealed partial class KthAncestorOfATreeNodeTests
{
    [Fact]
    public void GetKthAncestor_LeetCodeExample_ReturnsExpectedAncestorsAndMinusOneWhenTooFar()
    {
        // node:   0
        //        / \
        //       1   2
        //      / \ / \
        //     3  4 5  6
        int[] parent = [-1, 0, 0, 1, 1, 2, 2];
        var ancestorsById = BuildAncestorTable(parent);

        var thirdNodeFirstAncestor = GetKthAncestor(ancestorsById, node: 3, k: 1);
        var fifthNodeSecondAncestor = GetKthAncestor(ancestorsById, node: 5, k: 2);
        var sixthNodeThirdAncestor = GetKthAncestor(ancestorsById, node: 6, k: 3);

        Assert.Equal(1, thirdNodeFirstAncestor);
        Assert.Equal(0, fifthNodeSecondAncestor);
        Assert.Equal(-1, sixthNodeThirdAncestor);
    }

    [Fact]
    public void GetKthAncestor_RootNode_HasNoAncestors()
    {
        int[] parent = [-1, 0, 0];
        var ancestorsById = BuildAncestorTable(parent);

        var rootAncestor = GetKthAncestor(ancestorsById, node: 0, k: 1);

        Assert.Equal(-1, rootAncestor);
    }

    private static int GetKthAncestor(TreeAncestorNode[][] ancestorsById, int node, int k)
    {
        var ancestors = ancestorsById[node];
        return k <= ancestors.Length ? ancestors[^k].Id : -1;
    }

    // Builds the child-ward tree TopDownTraversal needs from the LeetCode-supplied
    // parent array (parent[i] = i's parent, root's parent is -1), then walks it once
    // to fill ancestorsById[node.Id] with node's full root-to-parent chain.
    private static TreeAncestorNode[][] BuildAncestorTable(int[] parent)
    {
        var nodes = new TreeAncestorNode[parent.Length];
        for (var i = 0; i < parent.Length; i++)
        {
            nodes[i] = new TreeAncestorNode(i);
        }

        for (var i = 1; i < parent.Length; i++)
        {
            nodes[parent[i]].Children.Add(nodes[i]);
        }

        var ancestorsById = new TreeAncestorNode[parent.Length][];

        TopDownTraversal.Walk<
            TreeAncestorNode, TreeAncestorTopology, ListChildren<TreeAncestorNode>,
            NaturalChildOrder<TreeAncestorNode, ListChildren<TreeAncestorNode>>, ListChildren<TreeAncestorNode>,
            CollectAncestorsHooks, (TreeAncestorNode[] Ancestors, TreeAncestorNode[][] AncestorsById)>(
            nodes[0], ([], ancestorsById));

        return ancestorsById;
    }

    private readonly struct CollectAncestorsHooks
        : ITopDownHooks<TreeAncestorNode, (TreeAncestorNode[] Ancestors, TreeAncestorNode[][] AncestorsById)>
    {
        public static void Visit(
            TreeAncestorNode node,
            (TreeAncestorNode[] Ancestors, TreeAncestorNode[][] AncestorsById) state,
            int depth,
            NodePosition position)
            => state.AncestorsById[node.Id] = state.Ancestors;

        public static (TreeAncestorNode[] Ancestors, TreeAncestorNode[][] AncestorsById) Descend(
            TreeAncestorNode parentNode,
            (TreeAncestorNode[] Ancestors, TreeAncestorNode[][] AncestorsById) parentState,
            TreeAncestorNode child)
        {
            var ancestors = new TreeAncestorNode[parentState.Ancestors.Length + 1];
            Array.Copy(parentState.Ancestors, ancestors, parentState.Ancestors.Length);
            ancestors[^1] = parentNode;

            return (ancestors, parentState.AncestorsById);
        }
    }
}
