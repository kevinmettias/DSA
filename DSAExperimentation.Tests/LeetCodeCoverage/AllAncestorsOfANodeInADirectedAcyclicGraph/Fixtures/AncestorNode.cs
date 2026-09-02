namespace DSAExperimentation.Tests.LeetCodeCoverage.AllAncestorsOfANodeInADirectedAcyclicGraph.Fixtures;

internal sealed class AncestorNode(int id)
{
    public int Id { get; } = id;

    public List<AncestorNode> Children { get; } = [];
}
