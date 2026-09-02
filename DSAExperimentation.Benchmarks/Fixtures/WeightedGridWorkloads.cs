using DSAExperimentation.DataStructures.Graph.Grids;

namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing over DataStructures.Graph.Grids' weighted grid - the grid itself is a
// data structure, only the choice of how large to make it lives here.
internal static class WeightedGridWorkloads
{
    // A square grid with no walls at all, so a Manhattan/Chebyshev heuristic is
    // exact rather than merely admissible and A* explores close to the
    // straight-line corridor instead of Dijkstra's full expanding diamond.
    public static (WeightedGridNode Source, WeightedGridNode FarCorner) OpenGrid(int size)
    {
        var nodes = WeightedGrid.Build(size, size);

        return (nodes[(0, 0)], nodes[(size - 1, size - 1)]);
    }
}
