namespace DSAExperimentation.LeetCode.FindEventualSafeStates;

// Predecessors, not the original graph's own out-edges: TopologicalSort.TrySort's
// in-degree bookkeeping needs edges pointing the direction Kahn's algorithm should
// peel from, and a node is only "safe" once every one of its own out-edges has
// already been confirmed safe - so the direction to close over here is "who points
// at me," mirroring CourseNode's own precedent of orienting edges toward what
// Kahn's in-degree count actually needs, not the problem statement's literal
// adjacency direction.
//
// Answers LC 802 alone - a plain adjacency-list graph node with no fixed vertex
// set or modulus - so it lives beside the solution rather than in Domain/
// (ARCHITECTURE.md #17.3/#17.6).
internal sealed class SafeStateNode(int id)
{
    public int Id { get; } = id;

    public List<SafeStateNode> Predecessors { get; } = [];

    public override string ToString() => Id.ToString();
}
