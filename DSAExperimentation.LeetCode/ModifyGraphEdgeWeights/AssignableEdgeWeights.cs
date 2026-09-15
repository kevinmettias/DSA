namespace DSAExperimentation.LeetCode.ModifyGraphEdgeWeights;

// The two fixed values LC 2699's edge weights are read and written against, named
// once for both ends of the problem: -1 is LeetCode's own marker for "you choose
// this edge's weight", and 1 is the smallest positive weight the problem allows -
// which every -1 edge starts at, so the first search over a freshly built graph is
// the shortest distance any legal assignment can produce. Separate from
// AssignableEdgeGraph because the graph that stores them and the strategy that
// reassigns them have to agree on both.
internal static class AssignableEdgeWeights
{
    // LC 2699 writes -1 for "you choose this edge's weight".
    public const int Unassigned = -1;

    // Weights must be positive integers, so 1 is the floor every -1 edge starts at.
    public const int FloorWeight = 1;
}
