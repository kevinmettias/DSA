using DSAExperimentation.Algorithms.Folding.Dags.Trees;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.CountWaysToBuildRoomsInAnAntColony;

// LeetCode 1916. Count Ways to Build Rooms in an Ant Colony: prevRoom[] describes a
// tree rooted at room 0 (prevRoom[i] is the room that must be built directly before
// room i), and the answer is that tree's count of distinct topological orderings,
// modulo 1e9+7.
//
// Both strategies are the same TreeFold over DataStructures' parent-array tree
// - exactly TreeMetrics.Size's composition with a counting algebra instead of
// SizeAlgebra's plain sum - and differ only in which algebra they close over, so
// what the benchmark measures is the cost of recomputing factorials per node
// against precomputing them once.
internal static class CountWaysToBuildRoomsInAnAntColonySolution
{
    public static int WaysToBuildOrderByPerNodeFactorial(int[] prevRoom)
    {
        var rooms = ParentArrayTree.Build(prevRoom);

        return WaysToBuildOrderByPerNodeFactorial(rooms[0]);
    }

    public static int WaysToBuildOrderByPerNodeFactorial(RootedTreeNode root)
    {
        var (_, ways) = TreeFold.Fold<
            RootedTreeNode, RootedTreeTopology, ListChildren<RootedTreeNode>,
            NaturalChildOrder<RootedTreeNode, ListChildren<RootedTreeNode>>, ListChildren<RootedTreeNode>,
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
        RoomWaysPrecomputedFactorialAlgebra.Prepare(roomCount);

        var (_, ways) = TreeFold.Fold<
            RootedTreeNode, RootedTreeTopology, ListChildren<RootedTreeNode>,
            NaturalChildOrder<RootedTreeNode, ListChildren<RootedTreeNode>>, ListChildren<RootedTreeNode>,
            RoomWaysPrecomputedFactorialAlgebra, (long Size, long Ways)>(root);

        return (int)ways;
    }
}
