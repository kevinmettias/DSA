namespace DSAExperimentation.Tests.LeetCodeCoverage.CountVisitedNodesInADirectedGraph.Fixtures;

// Each node has exactly one outgoing edge - LeetCode's own "edges[i]" functional-graph
// shape (same as LongestCycleInAGraphTests' own fixture of this name), so Successors
// always holds exactly 1 entry here (LC 2876 disallows both self-loops and a missing
// edge, unlike LC 2360's -1 case).
internal sealed class FunctionalGraphNode(int id)
{
    public int Id { get; } = id;

    public List<FunctionalGraphNode> Successors { get; } = [];

    public override string ToString() => Id.ToString();
}
