namespace DSAExperimentation.LeetCode.LargestColorValueInADirectedGraph;

// Color is a 0-25 index into the lowercase alphabet, matching the problem's own
// colors string. Edges point predecessor -> successor, the same direction Kahn's
// algorithm needs for in-degree bookkeeping (CourseNode's own precedent).
//
// Answers LC 1857 alone - a plain adjacency-list graph node with no fixed vertex
// set or modulus - so it lives beside the solution rather than in Domain/ or
// DataStructures/ (ARCHITECTURE.md #17.3/#17.6). It previously existed twice: once
// as a Tests fixture and once again as a private copy inside
// LargestColorValueInADirectedGraphBenchmarks; this is the single surviving
// declaration.
internal sealed class ColorGraphNode(int id, int color)
{
    public int Id { get; } = id;

    public int Color { get; } = color;

    public List<ColorGraphNode> Successors { get; } = [];

    public override string ToString() => Id.ToString();
}
