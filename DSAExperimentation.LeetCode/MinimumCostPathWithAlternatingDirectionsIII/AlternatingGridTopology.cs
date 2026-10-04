using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;
using DSAExperimentation.DataStructures.Graph.Grids;

namespace DSAExperimentation.LeetCode.MinimumCostPathWithAlternatingDirectionsIII;

// Every state has up to five outgoing actions: wait (pay the cell's own penalty,
// the parity still flips) and the four grid moves, always legal regardless of
// parity - only whether the move's direction matches the CURRENT action's
// required set (right/down for an odd action, left/up for an even one) decides
// whether the source cell's penalty is also charged. All five costs are
// non-negative, so ShortestPath.Dijkstra applies directly - the edges are never
// materialized as a stored adjacency structure, GetEdges generates one node's
// worth on demand exactly like GridShortestPath's own composition.
internal readonly struct AlternatingGridTopology
    : IEdgeTopology<AlternatingGridNode, ListEdges<AlternatingGridNode, long>, long>
{
    public static ListEdges<AlternatingGridNode, long> GetEdges(AlternatingGridNode node)
    {
        var penalty = node.Penalty;
        var bounds = GridSize.Of(penalty);
        var stayPenalty = penalty[node.Row][node.Col];
        var flipped = !node.NextActionIsOdd;

        var edges = new List<(long Weight, AlternatingGridNode Target)>
        {
            (stayPenalty, node with { NextActionIsOdd = flipped }),
        };

        foreach (var (deltaRow, deltaCol, matchesOddAction) in AlternatingGridMoveTable.Moves)
        {
            AddMoveEdge(edges, node, (deltaRow, deltaCol, matchesOddAction), bounds);
        }

        return new ListEdges<AlternatingGridNode, long>(edges);
    }

    // The edge one move contributes, unless the move leaves the penalty grid on any of
    // its four edges and so has no target cell to move to. Only whether the move's
    // direction matches the current action's required set decides whether the source
    // cell's penalty is charged again.
    private static void AddMoveEdge(
        List<(long Weight, AlternatingGridNode Target)> edges,
        AlternatingGridNode node,
        (int DeltaRow, int DeltaCol, bool MatchesOddAction) move,
        GridSize bounds)
    {
        var row = node.Row + move.DeltaRow;
        var col = node.Col + move.DeltaCol;

        if (!bounds.HasCell(row, col))
        {
            return;
        }

        var stayPenalty = node.Penalty[node.Row][node.Col];
        var target = node with { Row = row, Col = col, NextActionIsOdd = !node.NextActionIsOdd };
        var followsParity = move.MatchesOddAction == node.NextActionIsOdd;
        var weight = target.EntranceCost + (followsParity ? 0 : stayPenalty);

        edges.Add((weight, target));
    }
}
