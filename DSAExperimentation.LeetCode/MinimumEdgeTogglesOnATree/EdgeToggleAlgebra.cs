using DSAExperimentation.Algorithms.Folding;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.MinimumEdgeTogglesOnATree;

// A post-order fold over ToggleTree: each node's result is (NeedsParentToggle,
// ToggledEdges) - whether this node's own color still disagrees with target after
// every toggle decided within its subtree, and every edge index toggled so far.
// A child that still needs a toggle forces exactly one toggle: the edge to *this*
// node, which is the only remaining edge that can reach it - so Combine adds that
// edge (looked up via ToggleTree.ParentEdgeIndex, prepared once before the fold)
// and flips this node's own requirement, since toggling an edge flips both ends.
//
// Combine only receives already-folded child results, not the child nodes
// themselves (IFoldAlgebra's contract), so it re-reads node.Children in the same
// order TreeFold folded them (RootedTreeTopology.GetChildren is exactly
// node.Children, walked under NaturalChildOrder - see RootedTreeTopology.cs) to
// zip each result back to the edge it came from - the same "table established
// once before folding begins" shape RoomWaysPrecomputedFactorialAlgebra uses,
// here for start/target/edge lookups instead of factorials. A witness meaningful
// only to this problem, so it lives here rather than in Domain/ (§17.3).
internal readonly struct EdgeToggleAlgebra
    : IFoldAlgebra<RootedTreeNode, (bool NeedsParentToggle, List<int> ToggledEdges)>
{
    private static string _start = "";
    private static string _target = "";
    private static int[] _parentEdgeIndex = [];

    public static void Prepare(string start, string target, int[] parentEdgeIndex)
    {
        _start = start;
        _target = target;
        _parentEdgeIndex = parentEdgeIndex;
    }

    public static (bool NeedsParentToggle, List<int> ToggledEdges) Empty => (false, []);

    public static (bool NeedsParentToggle, List<int> ToggledEdges) Combine(
        RootedTreeNode node, IReadOnlyList<(bool NeedsParentToggle, List<int> ToggledEdges)> children)
    {
        var needsToggle = _start[node.Id] != _target[node.Id];
        var toggled = new List<int>();

        for (var i = 0; i < children.Count; i++)
        {
            var (childNeedsToggle, childToggled) = children[i];
            toggled.AddRange(childToggled);

            if (childNeedsToggle)
            {
                toggled.Add(_parentEdgeIndex[node.Children[i].Id]);
                needsToggle = !needsToggle;
            }
        }

        return (needsToggle, toggled);
    }
}
