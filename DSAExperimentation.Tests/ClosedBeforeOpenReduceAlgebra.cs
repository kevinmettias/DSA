using DSAExperimentation.Graph;

namespace DSAExperimentation.Tests;

// Records, for every node, how many OTHER nodes had already fully finished (Exit'd)
// by the moment this one started (Enter'd). That's a global sequential fact about
// the whole walk, not a function of this node's own subtree, so a bottom-up
// IFoldAlgebra structurally cannot see it - it needs a single accumulator threaded
// across both Enter and Exit, which is exactly what IReduceAlgebra provides.
public readonly struct ClosedBeforeOpenReduceAlgebra<TMarker> : IReduceAlgebra<TestNode, int>
    where TMarker : struct
{
    private static readonly List<(string Name, int ClosedBefore)> log = [];

    public static IReadOnlyList<(string Name, int ClosedBefore)> Log => log;

    public static int Seed => 0;

    public static int Enter(int closedSoFar, TestNode node, int depth)
    {
        log.Add((node.Name, closedSoFar));
        return closedSoFar;
    }

    public static int Exit(int closedSoFar, TestNode node, int depth) => closedSoFar + 1;
}
