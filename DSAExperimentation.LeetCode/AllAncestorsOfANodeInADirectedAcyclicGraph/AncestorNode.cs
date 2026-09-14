namespace DSAExperimentation.LeetCode.AllAncestorsOfANodeInADirectedAcyclicGraph;

// Edges point ancestor -> descendant, which is the direction both strategies walk:
// Kahn's algorithm wants in-degree to count unresolved ancestors, and the naive
// forward walk wants to reach everything a start node is an ancestor of. Answers
// LC 2192 alone - a plain adjacency-list graph node over dense ids, with no fixed
// vertex set or modulus, so it lives beside the solution rather than in Domain/
// (ARCHITECTURE.md #17.6). Kept as its own copy rather than shared with
// LeetCode/CourseScheduleII's structurally identical CourseNode, because each
// problem folder is self-contained (ARCHITECTURE.md #17.2).
internal sealed class AncestorNode(int id)
{
    public int Id { get; } = id;

    public List<AncestorNode> Children { get; } = [];

    public override string ToString() => Id.ToString();
}
