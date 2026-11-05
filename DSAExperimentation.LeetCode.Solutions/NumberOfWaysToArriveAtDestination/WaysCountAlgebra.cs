using DSAExperimentation.Algorithms.Folding;

namespace DSAExperimentation.LeetCode.Solutions.NumberOfWaysToArriveAtDestination;

// Sums the children's already-counted shortest paths under LeetCode's own modulus
// (LeetCodeModulus, shared with every other counting problem in the catalogue).
// Dist == 0 uniquely identifies the destination (it's the Dijkstra source, and
// every road takes positive time so no other intersection can share its distance),
// so that's the fold's base case instead of threading a separate "is this the
// target" flag through Combine. Answers LC 1976 alone, which is why it lives beside
// the solution rather than in Algorithms/ (RoomWaysAlgebra's own precedent).
internal readonly struct WaysCountAlgebra : IFoldAlgebra<WaysNode, long>
{
    public long Empty => 0;

    public long Combine(WaysNode node, ReadOnlySpan<long> children)
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
