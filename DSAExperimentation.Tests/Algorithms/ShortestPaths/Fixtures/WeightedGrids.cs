using DSAExperimentation.DataStructures.Graph.Grids;

namespace DSAExperimentation.Tests.Algorithms.ShortestPaths.Fixtures;

// Test scenarios over DataStructures.Graph.Grids' weighted grid - the grid itself is a data
// structure, only the particular wall layout under test lives here.
internal static class WeightedGrids
{
    private const int GridSize = 3;

    private static readonly HashSet<(int Row, int Col)> CenterWall = [(1, 1)];

    // . . .
    // . # .
    // . . .
    // Same wall-detour shape as GridTests' GridWithCenterWall, rebuilt as
    // unit-weight IEdgeTopology instead of IGraphTopology: every edge crosses open
    // cells only, so the straight Manhattan distance from (1,0) to (1,2) (2)
    // undercounts the true shortest path (4) - proving AStar's heuristic only ever
    // *biases* the search order, it never shortcuts past the wall.
    public static Dictionary<(int Row, int Col), WeightedGridNode> WithCenterWall() =>
        WeightedGrid.Build(GridSize, GridSize, CenterWall);
}
