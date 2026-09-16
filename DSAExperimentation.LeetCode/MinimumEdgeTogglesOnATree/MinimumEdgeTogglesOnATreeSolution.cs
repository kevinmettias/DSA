using DSAExperimentation.Algorithms.Folding.Dags.Trees;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.MinimumEdgeTogglesOnATree;

// LeetCode 3812. Minimum Edge Toggles on a Tree: edges[] is an undirected tree:
// toggling edge i flips the color of both its endpoints. Find the fewest edges to
// toggle to turn start into target, and report their indices in increasing order,
// or [-1] if no sequence of toggles can.
//
// Root the tree at 0. A leaf can only be fixed by toggling the single edge to its
// parent, and that toggle also flips the parent - so bottom-up, every node's
// "does my color still disagree with target" requirement is forced: if it does,
// the edge to its own parent must be toggled (there is no other edge left that
// can reach it), which resolves this node and flips its parent's requirement in
// turn. If the root still disagrees after every subtree is resolved, no sequence
// of toggles closes the gap. Both strategies are that same bottom-up walk; they
// differ only in which primitives the tree and the walk are built from.
internal static class MinimumEdgeTogglesOnATreeSolution
{
    // The textbook answer: a BCL adjacency list and a recursive DFS that returns
    // whether the current node still needs its parent edge toggled - deliberately
    // written without this repo's tree/fold primitives, the arm the composed fold
    // below has to justify itself against.
    public static int[] MinTogglesByBruteForceDfs(int n, int[][] edges, string start, string target)
    {
        var adjacency = BuildAdjacency(n, edges);

        var toggled = new List<int>();
        var rootNeedsToggle = Dfs((0, -1), adjacency, (start, target), toggled);

        if (rootNeedsToggle)
        {
            return [LeetCodeAnswer.None];
        }

        toggled.Sort();
        return [.. toggled];
    }

    // The BCL adjacency list the textbook arm walks: n empty neighbour lists, then each
    // undirected edge appended to both of its endpoints, each side carrying the index it
    // came from. LeetCodeAdjacency states that layout once for every problem taking an
    // (n, edges) pair; only the stored per-neighbour pair is this arm's own.
    private static List<(int To, int EdgeIndex)>[] BuildAdjacency(int nodeCount, int[][] edges) =>
        LeetCodeAdjacency.ZeroBased<List<(int To, int EdgeIndex)>>(
            nodeCount, edges, _ => [], (list, farId, _, edgeIndex) => list.Add((farId, edgeIndex)));

    // An undirected walk step is the node and the neighbour it came from (the parent
    // is how this DFS excludes that neighbour), and the two colour strings always
    // travel as the one pair of states being reconciled.
    private static bool Dfs(
        (int Node, int Parent) step, List<(int To, int EdgeIndex)>[] adjacency,
        (string Start, string Target) colors, List<int> toggled)
    {
        var (node, parent) = step;
        var (start, target) = colors;
        var needsToggle = start[node] != target[node];

        foreach (var (next, edgeIndex) in adjacency[node])
        {
            if (next == parent)
            {
                continue;
            }

            if (Dfs((next, node), adjacency, colors, toggled))
            {
                toggled.Add(edgeIndex);
                needsToggle = !needsToggle;
            }
        }

        return needsToggle;
    }

    // Composed: ToggleTree turns edges[] into DataStructures' RootedTreeNode (plus
    // the per-node parent-edge index a plain parent array doesn't carry), and the
    // same bottom-up requirement propagation above is exactly TreeFold's
    // catamorphism over it with EdgeToggleAlgebra.
    public static int[] MinTogglesByTreeFold(int n, int[][] edges, string start, string target)
    {
        var tree = ToggleTree.Build(n, edges);

        return MinTogglesByTreeFold(tree, start, target);
    }

    public static int[] MinTogglesByTreeFold(ToggleTree tree, string start, string target)
    {
        EdgeToggleAlgebra.Prepare(start, target, tree.ParentEdgeIndex);

        var (rootNeedsToggle, toggled) = TreeFold.Fold<
            RootedTreeNode, RootedTreeTopology, ListChildren<RootedTreeNode>,
            NaturalChildOrder<RootedTreeNode, ListChildren<RootedTreeNode>>, ListChildren<RootedTreeNode>,
            EdgeToggleAlgebra, (bool NeedsParentToggle, List<int> ToggledEdges)>(tree.Root);

        if (rootNeedsToggle)
        {
            return [LeetCodeAnswer.None];
        }

        toggled.Sort();
        return [.. toggled];
    }
}
