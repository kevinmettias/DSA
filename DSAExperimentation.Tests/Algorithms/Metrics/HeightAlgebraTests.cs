using DSAExperimentation.Algorithms.Metrics;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Metrics;

public sealed class HeightAlgebraTests
{
    private static readonly TestNode Node = new("A");

    [Fact]
    public void Empty_IsZero()
    {
        Assert.Equal(0, HeightAlgebra<TestNode>.Empty);
    }

    [Fact]
    public void Combine_Leaf_IsOne()
    {
        Assert.Equal(1, HeightAlgebra<TestNode>.Combine(Node, []));
    }

    [Fact]
    public void Combine_TakesTheDeepestChildNotTheSum()
    {
        Assert.Equal(1 + 4, HeightAlgebra<TestNode>.Combine(Node, [2, 4, 3]));
    }

    [Fact]
    public void Combine_IsUnaffectedByChildOrder()
    {
        Assert.Equal(
            HeightAlgebra<TestNode>.Combine(Node, [2, 4, 3]),
            HeightAlgebra<TestNode>.Combine(Node, [4, 3, 2]));
    }
}
