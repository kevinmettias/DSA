using DSAExperimentation.Algorithms.Folding;

namespace DSAExperimentation.LeetCode.NumberOfRestrictedPathsFromFirstToLastNode;

// Sums the children's already-counted restricted paths under LeetCode's own modulus
// (LeetCodeModulus, shared with every other counting problem in the catalogue).
// Dist == 0 uniquely identifies the last node (it's the Dijkstra source, and every
// edge weight is positive so no other node can share its distance), so that's the
// fold's base case instead of threading a separate "is this the target" flag through
// Combine. Answers LC 1786 alone, which is why it lives beside the solution rather
// than at the tier's root (RoomWaysAlgebra's own precedent).
internal readonly struct RestrictedPathCountAlgebra : IFoldAlgebra<RestrictedPathNode, long>
{
    public long Empty => 0;

    public long Combine(RestrictedPathNode node, ReadOnlySpan<long> children)
    {
        if (node.Dist == 0)
        {
            return 1;
        }

        var total = 0L;

        for (var i = 0; i < children.Length; i++)
        {
            var child = children[i];
            total = (total + child) % LeetCodeModulus.Value;
        }

        return total;
    }
}
