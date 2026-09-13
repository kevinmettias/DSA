namespace DSAExperimentation.LeetCode.ReachableNodesInSubdividedGraph;

// One node per ORIGINAL graph vertex - never per subdivision node, which is the
// whole point of LC 882's composed strategy: the subdivision chain along an edge
// is represented by that edge's weight (cnt + 1 unit moves), not by cnt extra
// objects. Answers LC 882 alone - a fully generic weighted node belongs in
// DataStructures, but nothing that generic exists in DSAExperimentation yet
// (EdgeGraphNode's own doc comment makes the same call for LC 3123's identical
// shape), so this stays a problem-local witness.
internal sealed class SubdividedGraphNode(int id)
{
    public int Id { get; } = id;

    public List<(int Weight, SubdividedGraphNode Target)> Edges { get; } = [];

    public override string ToString() => Id.ToString();
}
