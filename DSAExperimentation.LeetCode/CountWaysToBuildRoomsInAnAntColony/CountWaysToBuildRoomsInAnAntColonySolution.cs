using DSAExperimentation.Algorithms.Folding.Dags.Trees;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.CountWaysToBuildRoomsInAnAntColony;

// LeetCode 1916. Count Ways to Build Rooms in an Ant Colony: prevRoom[] describes a
// tree rooted at room 0 (prevRoom[i] is the room that must be built directly before
// room i), and the answer is that tree's count of distinct topological orderings,
// modulo 1e9+7.
//
// The two composed strategies are the same TreeFold over DataStructures' parent-array
// tree - exactly TreeMetrics.Size's composition with a counting algebra instead of
// SizeAlgebra's plain sum - and differ only in which algebra they close over, so
// comparing them measures the cost of recomputing factorials per node against
// precomputing them once. The baseline is the plain post-order recursion that fold
// stands for, with the per-node algebra written inline, so comparing it with the
// per-node fold measures what the fold itself costs.
internal static class CountWaysToBuildRoomsInAnAntColonySolution
{
    public static int WaysToBuildOrderByPostOrderRecursion(int[] prevRoom)
    {
        var rooms = ParentArrayTree.Build(prevRoom);

        return WaysToBuildOrderByPostOrderRecursion(rooms[0]);
    }

    // The textbook answer: a post-order recursion that, at every room, multiplies its
    // children's own counts by the multinomial (size - 1)! / (s1! * s2! * ...), recomputing
    // each factorial from scratch and inverting it by Fermat's little theorem - O(subtree
    // size) per room, the same work RoomWaysAlgebra does inside TreeFold. It recurses once
    // per level, which LC's 1e5-room chain would overflow; the benchmark's chains stay far
    // below that.
    public static int WaysToBuildOrderByPostOrderRecursion(RootedTreeNode root)
    {
        var (_, ways) = RoomWays(root);

        return (int)ways;
    }

    private static (long Size, long Ways) RoomWays(RootedTreeNode room)
    {
        var subtree = (Size: 1L, Ways: 1L);

        foreach (var child in room.Children)
        {
            var (childSize, childWays) = RoomWays(child);
            subtree = AbsorbChild(subtree, childSize, childWays);
        }

        return CloseRoom(subtree);
    }

    // Folds one finished child into its room's running count: the child's rooms join the
    // subtree, its own orderings multiply in, and childSize! divides out of the multinomial
    // that interleaves the children.
    private static (long Size, long Ways) AbsorbChild(
        (long Size, long Ways) subtree, long childSize, long childWays)
    {
        var childFactorial = FactorialFromScratch(childSize);
        var ways = subtree.Ways * childWays % LeetCodeModulus.Value * ModInverse(childFactorial) % LeetCodeModulus.Value;

        return (subtree.Size + childSize, ways);
    }

    // Recomputed from scratch at every room that needs it - the per-node cost the
    // precomputed table exists to remove.
    private static long FactorialFromScratch(long value)
    {
        var result = 1L;

        for (var i = 2; i <= value; i++)
        {
            result = result * i % LeetCodeModulus.Value;
        }

        return result;
    }

    // Fermat's little theorem: for the prime LeetCodeModulus.Value, value^(LeetCodeModulus.Value - 2) is value's inverse.
    private static long ModInverse(long value) => ModPower(value, LeetCodeModulus.Value - 2);

    // value^exponent mod LeetCodeModulus.Value by repeated squaring, restated rather than called on
    // ModularArithmetic for the same reason as LeetCodeModulus.Value itself.
    private static long ModPower(long value, long exponent)
    {
        var result = 1L;
        value %= LeetCodeModulus.Value;

        while (exponent > 0)
        {
            if ((exponent & 1) == 1)
            {
                result = result * value % LeetCodeModulus.Value;
            }

            value = value * value % LeetCodeModulus.Value;
            exponent >>= 1;
        }

        return result;
    }

    // The multinomial's numerator, once every child is in: the room itself is built first,
    // and the other size - 1 rooms of its subtree interleave in (size - 1)! orders.
    private static (long Size, long Ways) CloseRoom((long Size, long Ways) subtree)
    {
        var interleavings = FactorialFromScratch(subtree.Size - 1);

        return (subtree.Size, subtree.Ways * interleavings % LeetCodeModulus.Value);
    }

    public static int WaysToBuildOrderByPerNodeFactorial(int[] prevRoom)
    {
        var rooms = ParentArrayTree.Build(prevRoom);

        return WaysToBuildOrderByPerNodeFactorial(rooms[0]);
    }

    public static int WaysToBuildOrderByPerNodeFactorial(RootedTreeNode root)
    {
        var (_, ways) = TreeFold.Fold<
            RootedTreeNode, RootedTreeTopology, ListChildren<RootedTreeNode>,
            RoomWaysAlgebra, (long Size, long Ways)>(root);

        return (int)ways;
    }

    public static int WaysToBuildOrderByPrecomputedFactorials(int[] prevRoom)
    {
        var rooms = ParentArrayTree.Build(prevRoom);

        return WaysToBuildOrderByPrecomputedFactorials(rooms[0], prevRoom.Length);
    }

    // The table must cover the whole tree, so the caller states the room count -
    // which a benchmark already knows and would otherwise have to recount.
    public static int WaysToBuildOrderByPrecomputedFactorials(RootedTreeNode root, int roomCount)
    {
        var (_, ways) = TreeFold.Fold<
            RootedTreeNode, RootedTreeTopology, ListChildren<RootedTreeNode>,
            RoomWaysPrecomputedFactorialAlgebra, (long Size, long Ways)>(
            root, new RoomWaysPrecomputedFactorialAlgebra(FactorialTable.Build(roomCount)));

        return (int)ways;
    }
}
