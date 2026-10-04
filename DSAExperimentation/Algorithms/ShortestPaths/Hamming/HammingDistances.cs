using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Hamming;

namespace DSAExperimentation.Algorithms.ShortestPaths.Hamming;

// BreadthFirstDistances fixed to HammingNode once instead of at every call site - the node, topology
// and children are the same three types for every problem in this family.
internal static class HammingDistances
{
    public static Dictionary<HammingNode, int> From(HammingNode root) =>
        BreadthFirstDistances.From<HammingNode, HammingTopology, ListChildren<HammingNode>>(root);
}
