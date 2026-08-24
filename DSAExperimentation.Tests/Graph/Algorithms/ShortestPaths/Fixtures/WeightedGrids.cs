namespace DSAExperimentation.Tests.Graph.Algorithms.ShortestPaths.Fixtures;

internal static class WeightedGrids
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
        var nodes = BuildNodes();
        WireOrthogonalEdges(nodes);

        return nodes;
    }

    private const int GridSize = 3;
    private const int WallRow = 1;
    private const int WallCol = 1;

    private static Dictionary<(int, int), WeightedGridNode> BuildNodes()
    {
        var nodes = new Dictionary<(int, int), WeightedGridNode>();

        for (var row = 0; row < GridSize; row++)
        {
            for (var col = 0; col < GridSize; col++)
            {
                if (row == WallRow && col == WallCol)
                {
                    continue;
                }

                nodes[(row, col)] = new WeightedGridNode($"({row},{col})", row, col);
            }
        }

        return nodes;
    }

    private static void WireOrthogonalEdges(Dictionary<(int Row, int Col), WeightedGridNode> nodes)
    {
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
    }
}
