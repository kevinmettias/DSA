using DSAExperimentation.Graph;

namespace DSAExperimentation.Tests;

public sealed class WeightedNode(string name)
{
    public string Name { get; } = name;

    public List<(int Weight, WeightedNode Target)> Edges { get; } = [];

    public override string ToString() => Name;
}

public readonly struct WeightedTopology : IEdgeTopology<WeightedNode, ListEdges<WeightedNode, int>, int>
{
    public static ListEdges<WeightedNode, int> GetEdges(WeightedNode node) => new(node.Edges);
}

public readonly struct CountWeightedNodesReduceAlgebra : IReduceAlgebra<WeightedNode, int>
{
    public static int Seed => 0;

    public static int Enter(int state, WeightedNode node, int depth) => state + 1;
}

public static class WeightedGraphs
{
    // A -[1]-> B -[2]-> C -[1]-> D
    // A -[4]-> C
    // B -[5]-> D
    // Shortest A->D: A-B-C-D = 1+2+1 = 4 (not the direct-looking A-C-D = 4+1 = 5,
    // and not A-B-D = 1+5 = 6).
    public static WeightedNode SampleGraph(
        out WeightedNode a, out WeightedNode b, out WeightedNode c, out WeightedNode d)
    {
        a = new WeightedNode("A");
        b = new WeightedNode("B");
        c = new WeightedNode("C");
        d = new WeightedNode("D");

        a.Edges.Add((1, b));
        a.Edges.Add((4, c));
        b.Edges.Add((2, c));
        b.Edges.Add((5, d));
        c.Edges.Add((1, d));

        return a;
    }
}

// A weighted, coordinate-carrying node - distinct from WeightedNode because
// ShortestPath.AStar needs geometry to compute an admissible heuristic against,
// which WeightedNode's arbitrary edge weights don't provide.
public sealed class WeightedGridNode(string name, int row, int col)
{
    public string Name { get; } = name;
    public int Row { get; } = row;
    public int Col { get; } = col;

    public List<(int Weight, WeightedGridNode Target)> Edges { get; } = [];

    public override string ToString() => Name;
}

public readonly struct WeightedGridTopology : IEdgeTopology<WeightedGridNode, ListEdges<WeightedGridNode, int>, int>
{
    public static ListEdges<WeightedGridNode, int> GetEdges(WeightedGridNode node) => new(node.Edges);
}

// Straight-line (Manhattan) distance on a unit-weight 4-directional grid: never
// overestimates the true remaining distance, even once a wall forces a detour, since
// removing cells can only make the true shortest path longer than the unobstructed
// straight line - which is exactly what "admissible" requires.
public readonly struct ManhattanHeuristic : IPathHeuristic<WeightedGridNode, int>
{
    public static int Estimate(WeightedGridNode node, WeightedGridNode? target)
        => target is null ? 0 : Math.Abs(node.Row - target.Row) + Math.Abs(node.Col - target.Col);
}

public static class WeightedGrids
{
    // . . .
    // . # .
    // . . .
    // Same wall-detour shape as GridTests' GridWithCenterWall, rebuilt as
    // unit-weight IEdgeTopology instead of IGraphTopology: every edge crosses open
    // cells only, so the straight Manhattan distance from (1,0) to (1,2) (2)
    // undercounts the true shortest path (4) - proving AStar's heuristic only ever
    // *biases* the search order, it never shortcuts past the wall.
    public static Dictionary<(int Row, int Col), WeightedGridNode> WithCenterWall()
    {
        var nodes = new Dictionary<(int, int), WeightedGridNode>();

        for (var row = 0; row < 3; row++)
        {
            for (var col = 0; col < 3; col++)
            {
                if (row == 1 && col == 1)
                {
                    continue;
                }

                nodes[(row, col)] = new WeightedGridNode($"({row},{col})", row, col);
            }
        }

        (int DRow, int DCol)[] directions = [(-1, 0), (1, 0), (0, -1), (0, 1)];

        foreach (var ((row, col), node) in nodes)
        {
            foreach (var (dRow, dCol) in directions)
            {
                if (nodes.TryGetValue((row + dRow, col + dCol), out var neighbor))
                {
                    node.Edges.Add((1, neighbor));
                }
            }
        }

        return nodes;
    }
}
