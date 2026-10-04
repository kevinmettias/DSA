using DSAExperimentation.DataStructures.Graph.Grids;

namespace DSAExperimentation.Algorithms.ShortestPaths.Grids;

// BreadthFirstDistances fixed to the grid's node, topology and children, read for one target - see
// DistanceMapReduceAlgebra for why this particular runtime-parameterized query doesn't need a bespoke
// traversal, unlike LowestCommonAncestor.
internal static class GridShortestPath
{
    public static int? Distance(GridNode start, GridNode target)
    {
        var distances = BreadthFirstDistances.From<GridNode, GridTopology, GridChildren>(start);

        return distances.TryGetValue(target, out var distance) ? distance : null;
    }
}
