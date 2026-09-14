namespace DSAExperimentation.LeetCode.MinimumCostOfAPathWithSpecialRoads;

// LC 2662's own vertex: one of the finitely many points a route ever needs to turn
// at - the start, the target, and each special road's two endpoints - carrying the
// outgoing weighted edges of the graph the solution builds over them (a symmetric
// Manhattan-distance edge to every other point, plus each special road's own one-way
// discounted edge). The coordinates themselves stay in the solution's point-to-node
// map; a node only needs an identity, the same way LC 1584's PointNode carries just
// its input index.
//
// Kept local to this problem folder rather than in DataStructures/: Tests' own
// WeightedNode and Benchmarks' own WeightedGraphNode already cover the general
// "weighted graph node" fixture role for the still-unmigrated problems, so a third
// general copy here would recreate the exact defect this migration exists to remove
// - the same reasoning LeetCode/NetworkDelayTime's NetworkNode records.
internal sealed class SpecialRoadNode(int index)
{
    public int Index { get; } = index;

    public List<(int Weight, SpecialRoadNode Target)> Edges { get; } = [];

    public override string ToString() => Index.ToString();
}
