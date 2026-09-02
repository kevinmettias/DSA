namespace DSAExperimentation.LeetCode.FindEdgesInShortestPaths;

// One node per graph vertex; Edges holds both directions of every undirected
// edge from the input, weighted by long since a path's total cost can exceed
// int range (n and w are each up to 5*10^4/1e5). Answers LC 3123 alone - a
// fully generic weighted node belongs in DataStructures, not here, but
// nothing that generic exists in DSAExperimentation yet (BranchNode's own doc
// comment makes the same call for LC 2959's identical shape), so this stays a
// problem-local witness.
internal sealed class EdgeGraphNode(int id)
{
    public int Id { get; } = id;

    public List<(long Weight, EdgeGraphNode Target)> Edges { get; } = [];

    public override string ToString() => Id.ToString();
}
