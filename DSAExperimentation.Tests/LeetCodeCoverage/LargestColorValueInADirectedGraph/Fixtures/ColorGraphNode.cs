namespace DSAExperimentation.Tests.LeetCodeCoverage.LargestColorValueInADirectedGraph.Fixtures;

// Color is a 0-25 index into the lowercase alphabet, matching the problem's
// own colors string. Edges point predecessor -> successor, the same
// direction Kahn's algorithm needs for in-degree bookkeeping (CourseNode's
// own precedent).
internal sealed class ColorGraphNode(int id, int color)
{
    public int Id { get; } = id;

    public int Color { get; } = color;

    public List<ColorGraphNode> Successors { get; } = [];

    public override string ToString() => Id.ToString();
}
