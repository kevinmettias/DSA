namespace DSAExperimentation.LeetCode.MinCostToConnectAllPoints;

// LC 1584's own vertex in the complete graph over the given points: one node per
// input index, carrying the undirected Manhattan-distance edges to every other
// point as a symmetric adjacency list (MinimumSpanningTree.Kruskal's own doc
// comment explains why discovering each edge twice costs a little and changes
// nothing).
//
// Kept local to this problem folder rather than in DataStructures/: Tests' own
// WeightedNode and Benchmarks' own WeightedGraphNode already cover the general
// "weighted graph node" fixture role for the still-unmigrated problems, so a
// third general copy here would recreate the exact defect this migration exists
// to remove - the same reasoning LeetCode/NetworkDelayTime's NetworkNode records.
internal sealed class PointNode(int index)
{
    public int Index { get; } = index;

    public List<(int Weight, PointNode Target)> Edges { get; } = [];

    public override string ToString() => Index.ToString();
}
