namespace DSAExperimentation.Tests.LeetCodeCoverage.FindEventualSafeStates.Fixtures;

// Predecessors, not the original graph's own out-edges: TopologicalSort.TrySort's
// in-degree bookkeeping needs edges pointing the direction Kahn's algorithm should
// peel from, and a node is only "safe" once every one of its own out-edges has
// already been confirmed safe - so the direction to close over here is "who points
// at me," mirroring CourseNode's own precedent of orienting edges toward what
// Kahn's in-degree count actually needs, not the problem statement's literal
// adjacency direction.
internal sealed class SafeStateNode(int id)
{
    public int Id { get; } = id;

    public List<SafeStateNode> Predecessors { get; } = [];

    public override string ToString() => Id.ToString();
}
