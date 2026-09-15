using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

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
    private static readonly (int DeltaRow, int DeltaCol, bool MatchesOddAction)[] Moves =
    [
        (1, 0, true), (0, 1, true),
        (-1, 0, false), (0, -1, false),
    ];

    public static ListEdges<AlternatingGridNode, long> GetEdges(AlternatingGridNode node)
    {
        var penalty = node.Penalty;
        var rows = penalty.Length;
        var cols = penalty[0].Length;
        var stayPenalty = penalty[node.Row][node.Col];
        var flipped = !node.NextActionIsOdd;

        var edges = new List<(long Weight, AlternatingGridNode Target)>
        {
            (stayPenalty, node with { NextActionIsOdd = flipped }),
        };

        foreach (var (deltaRow, deltaCol, matchesOddAction) in Moves)
        {
            AddMoveEdge(edges, node, (deltaRow, deltaCol, matchesOddAction), (rows, cols));
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
        (int Rows, int Cols) bounds)
    {
        var row = node.Row + move.DeltaRow;
        var col = node.Col + move.DeltaCol;

        if (IsOutsideGrid(row, col, bounds.Rows, bounds.Cols))
        {
            return;
        }

        var stayPenalty = node.Penalty[node.Row][node.Col];
        var target = node with { Row = row, Col = col, NextActionIsOdd = !node.NextActionIsOdd };
        var followsParity = move.MatchesOddAction == node.NextActionIsOdd;
        var weight = target.EntranceCost + (followsParity ? 0 : stayPenalty);

        edges.Add((weight, target));
    }

    // A move that leaves the penalty grid on any of its four edges has no target
    // cell to move to.
    private static bool IsOutsideGrid(int row, int col, int rows, int cols)
        => row < 0 || row >= rows || col < 0 || col >= cols;
}
