using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Hamming;

namespace DSAExperimentation.Algorithms.ShortestPaths.Hamming;

// Reduce.Graph's BFS distance map, specialized to HammingNode once instead of at
// every call site - the generic argument list is eight types long and identical
// for every problem in this family.
internal static class HammingDistances
{
    public static Dictionary<HammingNode, int> From(HammingNode root) =>
        Reduce.Graph<
            HammingNode, HammingTopology, ListChildren<HammingNode>,
            NaturalChildOrder<HammingNode, ListChildren<HammingNode>>, ListChildren<HammingNode>,
            BreadthFirstReduceOrder<HammingNode>,
            DistanceMapReduceAlgebra<HammingNode>, Dictionary<HammingNode, int>>(root);
}
