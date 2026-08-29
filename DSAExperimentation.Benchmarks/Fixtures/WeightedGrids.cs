namespace DSAExperimentation.Benchmarks.Fixtures;

internal static class WeightedGrids
{
    public static (WeightedGridNode[,] Nodes, WeightedGridNode Source, WeightedGridNode FarCorner) OpenGrid(int size)
    {
        var nodes = new WeightedGridNode[size, size];

        for (var row = 0; row < size; row++)
        {
            for (var col = 0; col < size; col++)
            {
                nodes[row, col] = new WeightedGridNode(row, col);
            }
        }

        WireOrthogonalEdges(nodes, size);

        return (nodes, nodes[0, 0], nodes[size - 1, size - 1]);
    }

    private static void WireOrthogonalEdges(WeightedGridNode[,] nodes, int size)
    {
        (int DRow, int DCol)[] directions = [(-1, 0), (1, 0), (0, -1), (0, 1)];

        for (var row = 0; row < size; row++)
        {
            for (var col = 0; col < size; col++)
            {
                foreach (var (dRow, dCol) in directions)
                {
                    var (r, c) = (row + dRow, col + dCol);

                    if (r >= 0 && r < size && c >= 0 && c < size)
                    {
                        nodes[row, col].Edges.Add((1, nodes[r, c]));
                    }
                }
            }
        }
    }
}
