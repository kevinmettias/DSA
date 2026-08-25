using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Algorithms.Graph.Engines.Reducing;

namespace DSAExperimentation.Graph.Algorithms.Grids;

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
