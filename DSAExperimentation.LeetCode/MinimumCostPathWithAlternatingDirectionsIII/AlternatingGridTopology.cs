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
            var row = node.Row + deltaRow;
            var col = node.Col + deltaCol;

            if (row < 0 || row >= rows || col < 0 || col >= cols)
            {
                continue;
            }

            var target = node with { Row = row, Col = col, NextActionIsOdd = flipped };
            var followsParity = matchesOddAction == node.NextActionIsOdd;
            var weight = target.EntranceCost + (followsParity ? 0 : stayPenalty);

            edges.Add((weight, target));
        }

        return new ListEdges<AlternatingGridNode, long>(edges);
    }
}
