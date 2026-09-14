namespace DSAExperimentation.LeetCode.ModifyGraphEdgeWeights;

// One node of LC 2699's undirected graph. Edges holds both directions of every
// road, and holds them by value in a List so a weight can be reassigned in place
// by index - which is the whole point of this problem and the reason it does not
// reuse a shared immutable weighted node. Answers LC 2699 alone; nothing generic
// enough for a mutable weighted graph exists in DataStructures yet, so this stays
// a problem-local witness rather than another copy of the harness-tier
// WeightedNode / WeightedGraphNode pair the pre-migration test and benchmark each
// reached for (NetworkNode and CityNode make the same call).
internal sealed class AssignableEdgeNode(int id)
{
    public int Id { get; } = id;

    public List<(int Weight, AssignableEdgeNode Target)> Edges { get; } = [];

    public override string ToString() => Id.ToString();
}
