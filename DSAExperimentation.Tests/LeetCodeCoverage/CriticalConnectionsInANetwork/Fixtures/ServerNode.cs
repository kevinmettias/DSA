namespace DSAExperimentation.Tests.LeetCodeCoverage.CriticalConnectionsInANetwork.Fixtures;

internal sealed class ServerNode(int id)
{
    public int Id { get; } = id;

    public List<ServerNode> Neighbors { get; } = [];

    public override string ToString() => Id.ToString();
}
