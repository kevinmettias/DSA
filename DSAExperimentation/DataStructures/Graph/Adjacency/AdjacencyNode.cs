namespace DSAExperimentation.DataStructures.Graph.Adjacency;

// A node of a general directed graph over dense integer ids, its out-edges held as the list of
// nodes they lead to - the general-graph sibling of RootedTreeNode, which is this same shape plus a
// tree's one-parent promise. Nothing here promises acyclicity or unique ancestry, so the only
// topology offered over it is the graph tier (AdjacencyTopology).
//
// What an edge means - "enables", "is disliked by", "must be printed before" - is the caller's
// reading of the list, not a property of the node, so a builder names it where it wires the edges.
internal sealed class AdjacencyNode(int id)
{
    public int Id { get; } = id;

    public List<AdjacencyNode> Neighbors { get; } = [];

    public override string ToString() => Id.ToString();
}
