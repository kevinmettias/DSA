using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Hamming;

namespace DSAExperimentation.Algorithms.ShortestPaths.Hamming;

// BreadthFirstDistances fixed to HammingNode once instead of at every call site - the node, topology
// and children are the same three types for every problem in this family.
/// <summary>Breadth-first distances from one node of a Hamming graph to every node it can reach.</summary>
public static class HammingDistances
{
    /// <summary>Counts the fewest one-character steps from the root to every node it can reach.</summary>
    /// <param name="root">The node every distance is measured from.</param>
    /// <returns>
    /// Each node reachable from <paramref name="root"/>, the root itself at 0, mapped to its step count;
    /// nodes in other components are absent.
    /// </returns>
    public static Dictionary<HammingNode, int> From(HammingNode root) =>
        BreadthFirstDistances.From<HammingNode, HammingTopology, ListChildren<HammingNode>>(root);

    // One target's mutation count, from a search that stops once it reaches the target.
    /// <summary>Counts the fewest single-letter mutations from a root to one target.</summary>
    /// <param name="root">The word the mutations start from.</param>
    /// <param name="target">The word whose distance is wanted.</param>
    /// <returns>The fewest mutations from root to target, or null when no chain of words joins them.</returns>
    public static int? To(HammingNode root, HammingNode target) =>
        BreadthFirstDistances.To<HammingNode, HammingTopology, ListChildren<HammingNode>>(root, target);
}
