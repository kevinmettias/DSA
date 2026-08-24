using DSAExperimentation.Graph;

namespace DSAExperimentation.Tests;

public readonly struct CountNodesFoldAlgebra : IFoldAlgebra<TestNode, int>
{
    public static int Empty => 0;

    public static int Combine(TestNode node, IReadOnlyList<int> children)
        => 1 + children.Sum();
}

public readonly struct CountNodesReduceAlgebra : IReduceAlgebra<TestNode, int>
{
    public static int Seed => 0;

    public static int Enter(int state, TestNode node, int depth) => state + 1;
}

// Concatenates node names in visit order - deliberately non-commutative, so it can
// demonstrate that reduce (unlike fold) actually depends on traversal order.
public readonly struct PathReduceAlgebra : IReduceAlgebra<TestNode, string>
{
    public static string Seed => "";

    public static string Enter(string state, TestNode node, int depth) => state + node.Name;
}

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

// Counts how many times Combine actually fires - used to prove DagFold memoizes a
// shared descendant instead of recomputing it once per incoming path.
public readonly struct CountCombineCallsFoldAlgebra<TMarker> : IFoldAlgebra<TestNode, int>
    where TMarker : struct
{
    private static int combineCalls;

    public static int CombineCalls => combineCalls;

    public static int Empty => 0;

    public static int Combine(TestNode node, IReadOnlyList<int> children)
    {
        combineCalls++;
        return 1 + children.Sum();
    }
}

public readonly struct WordCountFoldAlgebra : IFoldAlgebra<TrieNode, int>
{
    public static int Empty => 0;

    public static int Combine(TrieNode node, IReadOnlyList<int> children)
        => (node.IsWord ? 1 : 0) + children.Sum();
}
