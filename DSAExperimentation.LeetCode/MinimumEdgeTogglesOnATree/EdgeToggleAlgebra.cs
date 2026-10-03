using DSAExperimentation.Algorithms.Folding;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.MinimumEdgeTogglesOnATree;

// A post-order fold over ToggleTree: each node's result is (NeedsParentToggle,
// ToggledEdges) - whether this node's own color still disagrees with target after
// every toggle decided within its subtree, and every edge index toggled so far.
// A child that still needs a toggle forces exactly one toggle: the edge to *this*
// node, which is the only remaining edge that can reach it - so Combine adds that
// edge (looked up via ToggleTree.ParentEdgeIndex, which the algebra holds)
// and flips this node's own requirement, since toggling an edge flips both ends.
//
// Combine only receives already-folded child results, not the child nodes
// themselves (IFoldAlgebra's contract), so it re-reads node.Children in the same
// order TreeFold folded them (RootedTreeTopology.GetChildren is exactly
// node.Children, walked under NaturalChildOrder - see RootedTreeTopology.cs) to
// zip each result back to the edge it came from - the start/target/edge lookups
// travel as the algebra's own field, the way RoomWaysPrecomputedFactorialAlgebra
// carries its factorial table. A witness meaningful
// only to this problem, so it lives here rather than in Domain/ (§17.3).
internal readonly struct EdgeToggleAlgebra(string start, string target, int[] parentEdgeIndex)
    : IFoldAlgebra<RootedTreeNode, (bool NeedsParentToggle, List<int> ToggledEdges)>
{
    // The three node-indexed lookups, held by the algebra each fold is handed, so a fold
    // reads only its own start/target/edge table.
    private readonly ToggleInputs _inputs = new(start, target, parentEdgeIndex);

    private readonly record struct ToggleInputs(string Start, string Target, int[] ParentEdgeIndex);

    public (bool NeedsParentToggle, List<int> ToggledEdges) Empty => (false, []);

    public (bool NeedsParentToggle, List<int> ToggledEdges) Combine(
        RootedTreeNode node, IReadOnlyList<(bool NeedsParentToggle, List<int> ToggledEdges)> children)
    {
        var inputs = _inputs;
        var needsToggle = inputs.Start[node.Id] != inputs.Target[node.Id];
        var toggled = new List<int>();

        for (var i = 0; i < children.Count; i++)
        {
            var (childNeedsToggle, childToggled) = children[i];
            toggled.AddRange(childToggled);

            if (childNeedsToggle)
            {
                toggled.Add(inputs.ParentEdgeIndex[node.Children[i].Id]);
                needsToggle = !needsToggle;
            }
        }

        return (needsToggle, toggled);
    }
}
