using DSAExperimentation.Algorithms.Folding;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.CountWaysToBuildRoomsInAnAntColony;

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
// which is why it lives beside the solution rather than in Domain.
internal readonly struct RoomWaysAlgebra : IFoldAlgebra<RootedTreeNode, (long Size, long Ways)>
{
    public static (long Size, long Ways) Empty => (0, 1);

    public static (long Size, long Ways) Combine(RootedTreeNode node, IReadOnlyList<(long Size, long Ways)> children)
    {
        var size = 1L;
        var ways = 1L;

        foreach (var (childSize, childWays) in children)
        {
            ways = ways * childWays % ModularArithmetic.Modulo;
            size += childSize;
        }

        ways = ways * Factorial(size - 1) % ModularArithmetic.Modulo;

        foreach (var (childSize, _) in children)
        {
            ways = ways * ModularArithmetic.Inverse(Factorial(childSize)) % ModularArithmetic.Modulo;
        }

        return (size, ways);
    }

    private static long Factorial(long value)
    {
        var result = 1L;

        for (var i = 2; i <= value; i++)
        {
            result = result * i % ModularArithmetic.Modulo;
        }

        return result;
    }
}
