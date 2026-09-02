namespace DSAExperimentation.Benchmarks.Fixtures;

// Mirrors DSAExperimentation.Tests' LongestCycleInAGraph fixture: each node has at
// most one outgoing edge, the functional-graph shape LeetCode 2360's `edges[i]` array
// itself describes.
internal sealed class FunctionalGraphNode(int id)
{
    public int Id { get; } = id;

    public List<FunctionalGraphNode> Successors { get; } = [];
}
