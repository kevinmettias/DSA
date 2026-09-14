namespace DSAExperimentation.LeetCode.MinimumWeightedSubgraphWithTheRequiredPaths;

// One node per graph vertex; Edges holds the outgoing directed edges of whichever
// orientation of the input graph this node belongs to, weighted by long since a
// path's total cost can exceed int range (n and each weight are up to 10^5).
// Answers LC 2203 alone - a fully generic weighted node belongs in DataStructures,
// not here, but nothing that generic exists in DSAExperimentation yet
// (EdgeGraphNode's own doc comment makes the same call for LC 3123's identical
// shape), so this stays a problem-local witness.
internal sealed class RequiredPathsNode(int id)
{
    public int Id { get; } = id;

    public List<(long Weight, RequiredPathsNode Target)> Edges { get; } = [];

    public override string ToString() => Id.ToString();
}
