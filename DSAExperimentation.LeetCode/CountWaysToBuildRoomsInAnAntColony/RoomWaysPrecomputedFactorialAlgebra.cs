using DSAExperimentation.Algorithms.Folding;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.CountWaysToBuildRoomsInAnAntColony;

// The same multinomial fold as RoomWaysAlgebra, with the factorials hoisted:
// Prepare(n) fills a factorial/inverse-factorial table once, O(n) total with a
// single modular-inverse call, so every Combine is O(children) instead of
// O(subtree size). The table is static, established once by the caller before
// folding begins and never re-touched during the walk - the same "external state
// stashed before the fold starts" shape a static-abstract algebra has to use,
// since it can carry no instance state of its own.
internal readonly struct RoomWaysPrecomputedFactorialAlgebra : IFoldAlgebra<RootedTreeNode, (long Size, long Ways)>
{
    private static long[] _factorial = [];
    private static long[] _inverseFactorial = [];

    public static (long Size, long Ways) Empty => (0, 1);

    public static void Prepare(int maxSize)
    {
        _factorial = new long[maxSize + 1];
        _inverseFactorial = new long[maxSize + 1];
        _factorial[0] = 1;

        for (var i = 1; i <= maxSize; i++)
        {
            _factorial[i] = _factorial[i - 1] * i % ModularArithmetic.Modulo;
        }

        _inverseFactorial[maxSize] = ModularArithmetic.Inverse(_factorial[maxSize]);

        for (var i = maxSize - 1; i >= 0; i--)
        {
            _inverseFactorial[i] = _inverseFactorial[i + 1] * (i + 1) % ModularArithmetic.Modulo;
        }
    }

    public static (long Size, long Ways) Combine(RootedTreeNode node, IReadOnlyList<(long Size, long Ways)> children)
    {
        var size = 1L;
        var ways = 1L;

        foreach (var (childSize, childWays) in children)
        {
            ways = ways * childWays % ModularArithmetic.Modulo;
            size += childSize;
        }

        ways = ways * _factorial[size - 1] % ModularArithmetic.Modulo;

        foreach (var (childSize, _) in children)
        {
            ways = ways * _inverseFactorial[childSize] % ModularArithmetic.Modulo;
        }

        return (size, ways);
    }
}
