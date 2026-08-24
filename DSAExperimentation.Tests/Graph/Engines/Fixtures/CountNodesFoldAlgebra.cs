using DSAExperimentation.Graph.Engines.Folding;
using DSAExperimentation.Tests.Graph.Fixtures;

namespace DSAExperimentation.Tests.Graph.Engines.Fixtures;

internal readonly struct CountNodesFoldAlgebra : IFoldAlgebra<TestNode, int>
{
    public static int Empty => 0;

    public static int Combine(TestNode node, IReadOnlyList<int> children)
        => 1 + children.Sum();
}
