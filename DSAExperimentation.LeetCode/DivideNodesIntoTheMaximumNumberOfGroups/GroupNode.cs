namespace DSAExperimentation.LeetCode.DivideNodesIntoTheMaximumNumberOfGroups;

// One node of LC 2493's undirected graph, with Neighbors holding every node it
// shares an edge with. Answers LC 2493 alone - a plain adjacency-list graph node
// with no fixed vertex set or modulus, so it lives beside the solution rather
// than in Domain/ (ARCHITECTURE.md #17.6), exactly like PossibleBipartition's
// PersonNode.
internal sealed class GroupNode(int id)
{
    public int Id { get; } = id;

    public List<GroupNode> Neighbors { get; } = [];

    public override string ToString() => Id.ToString();
}
