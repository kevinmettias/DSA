using DSAExperimentation.Algorithms.Metrics;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Metrics;

public sealed partial class SizeAlgebraTests
{
    private static readonly TestNode Node = new("A");

    [Fact]
    public void Empty_IsZeroSoAnAbsentSubtreeContributesNothing() => Assert.Equal(0, SizeAlgebra<TestNode>.Empty);

    [Fact]
    public void Combine_Leaf_CountsOnlyItself()
    {
        var combined = SizeAlgebra<TestNode>.Combine(Node, []);

        Assert.Equal(1, combined);
    }

    [Fact]
    public void Combine_AddsOneToTheSumOfItsChildrenSizes()
    {
        var combined = SizeAlgebra<TestNode>.Combine(Node, [2, 3]);

        Assert.Equal(1 + 2 + 3, combined);
    }

    [Fact]
    public void Combine_ManyChildren_SumsThemAll()
    {
        var combined = SizeAlgebra<TestNode>.Combine(Node, [1, 1, 1]);

        Assert.Equal(1 + 1 + 1 + 1, combined);
    }
}
