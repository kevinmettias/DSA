namespace DSAExperimentation.DataStructures.Graph.Grids;

// A unit-weight, 4-directionally connected rectangular grid of WeightedGridNodes,
// with any subset of cells walled off. Blocked cells are absent from the result
// entirely rather than present-but-unreachable, so every edge in the built graph
// crosses open cells only.
internal static class WeightedGrid
{
    private const int UnitWeight = 1;

    private static readonly (int DeltaRow, int DeltaCol)[] Orthogonal = [(-1, 0), (1, 0), (0, -1), (0, 1)];

    private static readonly HashSet<(int Row, int Col)> NoWalls = [];

    public static Dictionary<(int Row, int Col), WeightedGridNode> Build(int rows, int cols) =>
        Build(rows, cols, NoWalls);

    public static Dictionary<(int Row, int Col), WeightedGridNode> Build(
        int rows, int cols, IReadOnlySet<(int Row, int Col)> walls)
    {
        var nodes = BuildNodes(rows, cols, walls);

        WireOrthogonalEdges(nodes);

        return nodes;
    }

    private static Dictionary<(int Row, int Col), WeightedGridNode> BuildNodes(
        int rows, int cols, IReadOnlySet<(int Row, int Col)> walls)
    {
        var nodes = new Dictionary<(int Row, int Col), WeightedGridNode>();

        for (var row = 0; row < rows; row++)
        {
            for (var col = 0; col < cols; col++)
            {
                if (!walls.Contains((row, col)))
                {
                    nodes[(row, col)] = new WeightedGridNode(row, col);
                }
            }
        }

        return nodes;
    }

    private static void WireOrthogonalEdges(Dictionary<(int Row, int Col), WeightedGridNode> nodes)
    {
        foreach (var ((row, col), node) in nodes)
        {
            foreach (var (deltaRow, deltaCol) in Orthogonal)
            {
                if (nodes.TryGetValue((row + deltaRow, col + deltaCol), out var neighbor))
                {
                    node.Edges.Add((UnitWeight, neighbor));
                }
            }
        }
    }
}
