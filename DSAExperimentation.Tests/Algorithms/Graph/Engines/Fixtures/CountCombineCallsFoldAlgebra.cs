using DSAExperimentation.Algorithms.Graph.Engines.Folding;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Graph.Engines.Fixtures;

// Counts how many times Combine actually fires - used to prove DagFold memoizes a
// shared descendant instead of recomputing it once per incoming path.
internal readonly struct CountCombineCallsFoldAlgebra<TMarker> : IFoldAlgebra<TestNode, int>
    where TMarker : struct
{
    private static int _combineCalls;

    public static int CombineCalls => _combineCalls;

    public static int Empty => 0;

    public static int Combine(TestNode node, IReadOnlyList<int> children)
    {
        _combineCalls++;
        return 1 + children.Sum();
    }
}
