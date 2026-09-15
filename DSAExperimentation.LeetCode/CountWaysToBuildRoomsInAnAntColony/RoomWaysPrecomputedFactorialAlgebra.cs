using DSAExperimentation.Algorithms.Folding;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.CountWaysToBuildRoomsInAnAntColony;

// The same multinomial fold as RoomWaysAlgebra, with the factorials hoisted:
// Prepare(n) fills a factorial/inverse-factorial table once, O(n) total with a
// single modular-inverse call, so every Combine is O(children) instead of
// O(subtree size). The table is established by the caller before folding begins
// and never re-touched during the walk - the same "external state stashed before
// the fold starts" shape a static-abstract algebra has to use, since it can carry
// no instance state of its own. It is held in an AsyncLocal rather than in plain
// static fields so the table belongs to the flow that called Prepare: two folds on
// different threads get their own, instead of overwriting each other's mid-walk.
internal readonly struct RoomWaysPrecomputedFactorialAlgebra : IFoldAlgebra<RootedTreeNode, (long Size, long Ways)>
{
    // Both halves of the table travel together, so a Combine can never pair one
    // Prepare call's factorials with another's inverse factorials.
    private static readonly AsyncLocal<FactorialTable> Table = new();

    private readonly record struct FactorialTable(long[] Factorial, long[] InverseFactorial);

    public static (long Size, long Ways) Empty => (0, 1);

    public static void Prepare(int maxSize)
    {
        var factorial = new long[maxSize + 1];
        var inverseFactorial = new long[maxSize + 1];
        factorial[0] = 1;

        for (var i = 1; i <= maxSize; i++)
        {
            factorial[i] = factorial[i - 1] * i % ModularArithmetic.Modulo;
        }

        inverseFactorial[maxSize] = ModularArithmetic.Inverse(factorial[maxSize]);

        for (var i = maxSize - 1; i >= 0; i--)
        {
            inverseFactorial[i] = inverseFactorial[i + 1] * (i + 1) % ModularArithmetic.Modulo;
        }

        Table.Value = new FactorialTable(factorial, inverseFactorial);
    }

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

        ways = ways * table.Factorial[size - 1] % ModularArithmetic.Modulo;

        foreach (var (childSize, _) in children)
        {
            ways = ways * table.InverseFactorial[childSize] % ModularArithmetic.Modulo;
        }

        return (size, ways);
    }
}
