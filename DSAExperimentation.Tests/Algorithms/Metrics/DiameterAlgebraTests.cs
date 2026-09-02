using DSAExperimentation.Algorithms.Metrics;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Metrics;

public sealed class DiameterAlgebraTests
{
    private static readonly TestNode Node = new("A");

    private static HeightDiameterState State(int height, int diameter) => new(height, diameter);

    [Fact]
    public void Empty_IsZeroHeightAndZeroDiameter()
    {
        Assert.Equal(State(0, 0), DiameterAlgebra<TestNode>.Empty);
    }

    [Fact]
    public void Combine_Leaf_IsHeightOneAndDiameterZero()
    {
        Assert.Equal(State(1, 0), DiameterAlgebra<TestNode>.Combine(Node, []));
    }

    [Fact]
    public void Combine_HeightIsOneMoreThanTheTallestChild()
    {
        Assert.Equal(4, DiameterAlgebra<TestNode>.Combine(Node, [State(3, 0), State(1, 0)]).Height);
    }

    [Fact]
    public void Combine_TwoChildren_DiameterJoinsTheTwoTallestBranches()
    {
        // A path down one branch and up the other passes through this node.
        Assert.Equal(3 + 2, DiameterAlgebra<TestNode>.Combine(Node, [State(3, 0), State(2, 0)]).Diameter);
    }

    [Fact]
    public void Combine_OneChild_DiameterIsThatBranchesHeightAlone()
    {
        Assert.Equal(3, DiameterAlgebra<TestNode>.Combine(Node, [State(3, 0)]).Diameter);
    }

    [Fact]
    public void Combine_KeepsAChildsDiameterWhenItBeatsThePathThroughThisNode()
    {
        // A wide subtree hanging off a shallow branch: 9 must survive.
        Assert.Equal(9, DiameterAlgebra<TestNode>.Combine(Node, [State(2, 9), State(1, 0)]).Diameter);
    }

    [Fact]
    public void Combine_ThreeChildren_UsesOnlyTheTwoTallest()
    {
        Assert.Equal(5 + 4, DiameterAlgebra<TestNode>.Combine(
            Node, [State(5, 0), State(4, 0), State(3, 0)]).Diameter);
    }

    [Fact]
    public void Combine_IsUnaffectedByChildOrder()
    {
        var ascending = DiameterAlgebra<TestNode>.Combine(Node, [State(1, 0), State(4, 0), State(3, 0)]);
        var descending = DiameterAlgebra<TestNode>.Combine(Node, [State(4, 0), State(3, 0), State(1, 0)]);

        Assert.Equal(ascending, descending);
    }
}
