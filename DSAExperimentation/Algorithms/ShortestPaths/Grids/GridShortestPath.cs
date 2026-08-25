using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Grids;
using DSAExperimentation.Algorithms.Reducing;

namespace DSAExperimentation.Algorithms.ShortestPaths.Grids;

// Reuses Reduce.Graph directly, unlike LowestCommonAncestor - see
// DistanceMapReduceAlgebra for why this particular runtime-parameterized query
// doesn't need a bespoke traversal.
internal static class GridShortestPath
{
    public static int? Distance(GridNode start, GridNode target)
    {
        var distances = Reduce.Graph<
            GridNode, GridTopology, GridChildren,
            NaturalChildOrder<GridNode, GridChildren>, GridChildren,
            BreadthFirstReduceOrder<GridNode>,
            DistanceMapReduceAlgebra<GridNode>, Dictionary<GridNode, int>>(start);

        return distances.TryGetValue(target, out var distance) ? distance : null;
    }
}
