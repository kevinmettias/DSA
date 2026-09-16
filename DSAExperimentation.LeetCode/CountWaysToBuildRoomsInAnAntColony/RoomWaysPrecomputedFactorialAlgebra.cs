using DSAExperimentation.Algorithms.Folding;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.CountWaysToBuildRoomsInAnAntColony;

// The same multinomial fold as RoomWaysAlgebra, with the factorials hoisted:
// Prepare(n) builds one Domain.Modular FactorialTable, O(n) total with a single
// modular-inverse call, so every Combine is O(children) instead of O(subtree
// size). The table is established by the caller before folding begins and never
// re-touched during the walk - the same "external state stashed before the fold
// starts" shape a static-abstract algebra has to use, since it can carry no
// instance state of its own. It is held in an AsyncLocal rather than in plain
// static fields so the table belongs to the flow that called Prepare: two folds on
// different threads get their own, instead of overwriting each other's mid-walk.
internal readonly struct RoomWaysPrecomputedFactorialAlgebra : IFoldAlgebra<RootedTreeNode, (long Size, long Ways)>
{
    // The table travels whole, so a Combine can never pair one Prepare call's
    // factorials with another's inverse factorials.
    private static readonly AsyncLocal<FactorialTable> Table = new();

    public static (long Size, long Ways) Empty => (0, 1);

    public static void Prepare(int maxSize) => Table.Value = FactorialTable.Build(maxSize);

    public static (long Size, long Ways) Combine(RootedTreeNode node, IReadOnlyList<(long Size, long Ways)> children)
    {
        var table = Table.Value;
        var size = 1L;
        var ways = 1L;

        foreach (var (childSize, childWays) in children)
        {
            ways = ways * childWays % ModularArithmetic.Modulo;
            size += childSize;
        }

        // size and childSize are long because this algebra's Size contract is a
        // long; a subtree can hold no more nodes than the tree it came from, so
        // both narrow safely to FactorialTable's int indices.
        ways = ways * table.Factorial((int)(size - 1)) % ModularArithmetic.Modulo;

        foreach (var (childSize, _) in children)
        {
            ways = ways * table.InverseFactorial((int)childSize) % ModularArithmetic.Modulo;
        }

        return (size, ways);
    }
}
