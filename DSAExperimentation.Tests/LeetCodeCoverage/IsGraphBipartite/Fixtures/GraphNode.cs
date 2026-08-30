namespace DSAExperimentation.Tests.LeetCodeCoverage.IsGraphBipartite.Fixtures;

internal sealed class GraphNode(int id)
{
    public int Id { get; } = id;

    public List<GraphNode> Neighbors { get; } = [];

    public override string ToString() => Id.ToString();
}
