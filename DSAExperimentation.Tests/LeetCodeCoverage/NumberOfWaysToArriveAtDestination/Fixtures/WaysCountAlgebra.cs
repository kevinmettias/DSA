using DSAExperimentation.Algorithms.Folding;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfWaysToArriveAtDestination.Fixtures;

// LeetCode's own modulo. Dist == 0 uniquely identifies the destination (it's the
// Dijkstra source, and every edge weight is positive so no other node can share
// its distance), so that's the fold's base case - the same shape
// RestrictedPathCountAlgebra already uses for its own Dijkstra-source node.
internal readonly struct WaysCountAlgebra : IFoldAlgebra<WaysNode, long>
{
    private const long Modulo = 1_000_000_007;

    public static long Empty => 0;

    public static long Combine(WaysNode node, IReadOnlyList<long> children)
    {
        if (node.Dist == 0)
        {
            return 1;
        }

        var total = 0L;

        foreach (var child in children)
        {
            total = (total + child) % Modulo;
        }

        return total;
    }
}
