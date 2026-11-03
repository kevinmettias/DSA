using DSAExperimentation.Algorithms.Folding;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Folding.Fixtures;

internal readonly struct CountNodesFoldAlgebra : IFoldAlgebra<TestNode, int>
{
    public int Empty => 0;

    public int Combine(TestNode node, ReadOnlySpan<int> children)
    {
        var count = 1;

        foreach (var childCount in children)
        {
            count += childCount;
        }

        return count;
    }
}
