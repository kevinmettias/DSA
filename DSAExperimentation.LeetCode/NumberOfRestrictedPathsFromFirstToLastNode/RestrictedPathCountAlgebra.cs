using DSAExperimentation.Algorithms.Folding;
using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.NumberOfRestrictedPathsFromFirstToLastNode;

// Sums the children's already-counted restricted paths under LeetCode's own modulus
// (Domain.Modular's, shared with every other counting problem in the catalogue).
// Dist == 0 uniquely identifies the last node (it's the Dijkstra source, and every
// edge weight is positive so no other node can share its distance), so that's the
// fold's base case instead of threading a separate "is this the target" flag through
// Combine. Answers LC 1786 alone, which is why it lives beside the solution rather
// than in Domain (RoomWaysAlgebra's own precedent).
internal readonly struct RestrictedPathCountAlgebra : IFoldAlgebra<RestrictedPathNode, long>
{
    public static long Empty => 0;

    public static long Combine(RestrictedPathNode node, IReadOnlyList<long> children)
    {
        if (node.Dist == 0)
        {
            return 1;
        }

        var total = 0L;

        foreach (var child in children)
        {
            total = (total + child) % ModularArithmetic.Modulo;
        }

        return total;
    }
}
