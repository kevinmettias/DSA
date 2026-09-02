using DSAExperimentation.Algorithms.Metrics;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Metrics;

public sealed class SizeAlgebraTests
{
    private static readonly TestNode Node = new("A");

    [Fact]
    public void Empty_IsZeroSoAnAbsentSubtreeContributesNothing()
    {
        Assert.Equal(0, SizeAlgebra<TestNode>.Empty);
    }

    [Fact]
    public void Combine_Leaf_CountsOnlyItself()
    {
        Assert.Equal(1, SizeAlgebra<TestNode>.Combine(Node, []));
    }

    [Fact]
    public void Combine_AddsOneToTheSumOfItsChildrenSizes()
    {
        Assert.Equal(1 + 2 + 3, SizeAlgebra<TestNode>.Combine(Node, [2, 3]));
    }

    [Fact]
    public void Combine_ManyChildren_SumsThemAll()
    {
        Assert.Equal(1 + 1 + 1 + 1, SizeAlgebra<TestNode>.Combine(Node, [1, 1, 1]));
    }
}
