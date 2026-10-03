using DSAExperimentation.Algorithms.Folding;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.CountWaysToBuildRoomsInAnAntColony;

// The same multinomial fold as RoomWaysAlgebra, with the factorials hoisted: the
// caller builds one Domain.Modular FactorialTable, O(n) total with a single
// modular-inverse call, and hands it to the algebra, so every Combine is
// O(children) instead of O(subtree size). The table is a field of the algebra the
// fold is given, so each fold carries its own and the table travels whole - a
// Combine can never pair one table's factorials with another's inverse factorials.
internal readonly struct RoomWaysPrecomputedFactorialAlgebra(FactorialTable table)
    : IFoldAlgebra<RootedTreeNode, (long Size, long Ways)>
{
    public (long Size, long Ways) Empty => (0, 1);

    public (long Size, long Ways) Combine(RootedTreeNode node, IReadOnlyList<(long Size, long Ways)> children)
    {
        var size = 1L;
        var ways = 1L;

        for (var i = 0; i < children.Count; i++)
        {
            var (childSize, childWays) = children[i];
            ways = ways * childWays % ModularArithmetic.Modulo;
            size += childSize;
        }

        // size and childSize are long because this algebra's Size contract is a
        // long; a subtree can hold no more nodes than the tree it came from, so
        // both narrow safely to FactorialTable's int indices.
        ways = ways * table.Factorial((int)(size - 1)) % ModularArithmetic.Modulo;

        for (var i = 0; i < children.Count; i++)
        {
            var childSize = children[i].Size;
            ways = ways * table.InverseFactorial((int)childSize) % ModularArithmetic.Modulo;
        }

        return (size, ways);
    }
}
