namespace DSAExperimentation.LeetCode.CriticalConnectionsInANetwork;

// One server of LC 1192's undirected network. Answers LC 1192 alone - a plain
// adjacency-list graph node with no fixed vertex set or modulus, so it lives
// beside the solution rather than in Domain/ (ARCHITECTURE.md #17.6), exactly
// like IsGraphBipartite's BipartiteNode and CourseSchedule's CourseNode.
internal sealed class ServerNode(int id)
{
    public int Id { get; } = id;

    public List<ServerNode> Neighbors { get; } = [];

    public override string ToString() => Id.ToString();
}
