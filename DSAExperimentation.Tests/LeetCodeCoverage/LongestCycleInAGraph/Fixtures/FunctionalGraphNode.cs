namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestCycleInAGraph.Fixtures;

// Each node has at most one outgoing edge - LeetCode's own "edges[i]" functional-graph
// shape - so Successors holds 0 or 1 entries, never more.
internal sealed class FunctionalGraphNode(int id)
{
    public int Id { get; } = id;

    public List<FunctionalGraphNode> Successors { get; } = [];

    public override string ToString() => Id.ToString();
}
