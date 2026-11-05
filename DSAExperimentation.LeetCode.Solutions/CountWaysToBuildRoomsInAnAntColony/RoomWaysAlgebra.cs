using DSAExperimentation.Algorithms.Folding;
using DSAExperimentation.Algorithms.NumberTheory;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.Solutions.CountWaysToBuildRoomsInAnAntColony;

// The number of valid build orders for a tree is the number of its distinct
// topological orderings, which folds bottom-up as a multinomial coefficient: a
// subtree of size s built from children of size s1, s2, ... contributes
// (s-1)! / (s1! * s2! * ...) ways to interleave those children's own already-counted
// orderings among themselves.
//
// Factorial and modular inverse are recomputed from scratch per node (PowXn/SuperPow's
// own "no repo container fits a running scalar" precedent), which is O(subtree size)
// per node - see RoomWaysPrecomputedFactorialAlgebra for the O(1)-per-node variant
// this trades against. This algebra answers one LeetCode problem and nothing else,
// which is why it lives beside the solution rather than in Algorithms/.
internal readonly struct RoomWaysAlgebra : IFoldAlgebra<RootedTreeNode, (long Size, long Ways)>
{
    public (long Size, long Ways) Empty => (0, 1);

    public (long Size, long Ways) Combine(RootedTreeNode node, ReadOnlySpan<(long Size, long Ways)> children)
    {
        var size = 1L;
        var ways = 1L;

        for (var i = 0; i < children.Length; i++)
        {
            var (childSize, childWays) = children[i];
            ways = ways * childWays % LeetCodeModulus.Value;
            size += childSize;
        }

        ways = ways * Factorial(size - 1) % LeetCodeModulus.Value;

        for (var i = 0; i < children.Length; i++)
        {
            var childSize = children[i].Size;
            ways = ways * ModularInverse.ByFermat<LeetCodeModulus>(Factorial(childSize)) % LeetCodeModulus.Value;
        }

        return (size, ways);
    }

    private static long Factorial(long value)
    {
        var result = 1L;

        for (var i = 2; i <= value; i++)
        {
            result = result * i % LeetCodeModulus.Value;
        }

        return result;
    }
}
