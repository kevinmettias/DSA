using DSAExperimentation.Algorithms.Folding;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Folding.Fixtures;

// Counts how many times Combine actually fires - used to prove DagFold memoizes a
// shared descendant instead of recomputing it once per incoming path. The count lives
// in a CombineCounter the algebra holds by reference: the fold threads the algebra by
// value, so a counter field on the struct itself would be counted on a copy.
internal readonly struct CountCombineCallsFoldAlgebra(CombineCounter counter) : IFoldAlgebra<TestNode, int>
{
    public int Empty => 0;

    public int Combine(TestNode node, IReadOnlyList<int> children)
    {
        counter.Calls++;
        return 1 + children.Sum();
    }
}

internal sealed class CombineCounter
{
    public int Calls { get; set; }
}
