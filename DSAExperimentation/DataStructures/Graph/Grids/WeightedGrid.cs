using DSAExperimentation.Domain.Mathematics.Lattices;

namespace DSAExperimentation.DataStructures.Graph.Grids;

// A unit-weight, 4-directionally connected rectangular grid of WeightedGridNodes,
// with any subset of cells walled off. Blocked cells are absent from the result
// entirely rather than present-but-unreachable, so every edge in the built graph
// crosses open cells only.
/// <summary>
/// Builds unit-weight, four-way connected grid graphs of <see cref="WeightedGridNode"/>, leaving walled cells out.
/// </summary>
public static class WeightedGrid
{
    private const int UnitWeight = 1;

    private static readonly HashSet<(int Row, int Col)> NoWalls = [];

    /// <summary>Builds a grid graph with every cell open.</summary>
    /// <param name="rows">How many rows the grid has.</param>
    /// <param name="cols">How many columns the grid has.</param>
    /// <returns>Each cell's node keyed by (Row, Col), with a weight-1 edge to each orthogonal neighbour.</returns>
    public static Dictionary<(int Row, int Col), WeightedGridNode> Build(int rows, int cols) =>
        Build(rows, cols, NoWalls);

    /// <summary>Builds a grid graph with some cells walled off and left out entirely.</summary>
    /// <param name="rows">How many rows the grid has.</param>
    /// <param name="cols">How many columns the grid has.</param>
    /// <param name="walls">The cells to leave out of the graph; a wall off the grid changes nothing.</param>
    /// <returns>
    /// Each open cell's node keyed by (Row, Col), with a weight-1 edge to each open orthogonal neighbour.
    /// </returns>
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
            foreach (var (deltaRow, deltaCol) in GridDirections.Orthogonal)
            {
                if (nodes.TryGetValue((row + deltaRow, col + deltaCol), out var neighbor))
                {
                    node.Edges.Add((UnitWeight, neighbor));
                }
            }
        }
    }
}
