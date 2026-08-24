using DSAExperimentation.Graph;

namespace DSAExperimentation.Tests;

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
