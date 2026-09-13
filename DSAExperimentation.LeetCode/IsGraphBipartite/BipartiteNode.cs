namespace DSAExperimentation.LeetCode.IsGraphBipartite;

// One vertex of LC 785's undirected adjacency list. Answers LC 785 alone - a
// plain adjacency-list graph node with no fixed vertex set or modulus, so it
// lives beside the solution rather than in Domain/ (ARCHITECTURE.md #17.6),
// exactly like CourseSchedule's CourseNode and MaximumPartitionFactor's
// PartitionNode.
internal sealed class BipartiteNode(int id)
{
    public int Id { get; } = id;

    public List<BipartiteNode> Neighbors { get; } = [];

    public override string ToString() => Id.ToString();
}
