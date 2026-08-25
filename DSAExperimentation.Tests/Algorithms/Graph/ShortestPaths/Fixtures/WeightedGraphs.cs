namespace DSAExperimentation.Tests.Algorithms.Graph.ShortestPaths.Fixtures;

internal static class WeightedGraphs
{
    // A -[1]-> B -[2]-> C -[1]-> D
    // A -[4]-> C
    // B -[5]-> D
    // Shortest A->D: A-B-C-D = 1+2+1 = 4 (not the direct-looking A-C-D = 4+1 = 5,
    // and not A-B-D = 1+5 = 6).
    public static SampleGraphNodes SampleGraph()
    {
        var a = new WeightedNode("A");
        var b = new WeightedNode("B");
        var c = new WeightedNode("C");
        var d = new WeightedNode("D");

        a.Edges.Add((1, b));
        a.Edges.Add((4, c));
        b.Edges.Add((2, c));
        b.Edges.Add((5, d));
        c.Edges.Add((1, d));

        return new SampleGraphNodes(a, b, c, d);
    }
}
