using DSAExperimentation.Algorithms.Metrics;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Metrics;

public sealed partial class HeightAlgebraTests
{
    private static readonly TestNode Node = new("A");

    [Fact]
    public void Empty_IsZero() => Assert.Equal(0, default(HeightAlgebra<TestNode>).Empty);

    [Fact]
    public void Combine_Leaf_IsOne()
    {
        var combined = default(HeightAlgebra<TestNode>).Combine(Node, []);

        Assert.Equal(1, combined);
    }

    [Fact]
    public void Combine_TakesTheDeepestChildNotTheSum()
    {
        var combined = default(HeightAlgebra<TestNode>).Combine(Node, [2, 4, 3]);

        Assert.Equal(1 + 4, combined);
    }

    [Fact]
    public void Combine_IsUnaffectedByChildOrder()
    {
        var leftToRight = default(HeightAlgebra<TestNode>).Combine(Node, [2, 4, 3]);
        var rightToLeft = default(HeightAlgebra<TestNode>).Combine(Node, [4, 3, 2]);

        Assert.Equal(leftToRight, rightToLeft);
    }
}
