using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Reducing;

public sealed class ZipReduceAlgebraTests
{
    private static readonly TestNode Node = new("A");

    private static (Dictionary<TestNode, int> A, Dictionary<TestNode, int> B) Seed() =>
        ZipReduceAlgebra<TestNode,
            Dictionary<TestNode, int>, Dictionary<TestNode, int>,
            DistanceMapReduceAlgebra<TestNode>, DistanceMapReduceAlgebra<TestNode>>.Seed;

    [Fact]
    public void Seed_PairsBothAlgebrasOwnSeeds()
    {
        var (a, b) = Seed();

        Assert.Empty(a);
        Assert.Empty(b);
    }

    [Fact]
    public void Seed_GivesTheTwoSidesSeparateState()
    {
        var (a, b) = Seed();

        Assert.NotSame(a, b);
    }

    [Fact]
    public void Enter_AppliesBothAlgebrasToTheSameNode()
    {
        var state = ZipReduceAlgebra<TestNode,
            Dictionary<TestNode, int>, Dictionary<TestNode, int>,
            DistanceMapReduceAlgebra<TestNode>, DistanceMapReduceAlgebra<TestNode>>.Enter(Seed(), Node, 2);

        Assert.Equal(2, state.A[Node]);
        Assert.Equal(2, state.B[Node]);
    }

    [Fact]
    public void Exit_AppliesBothAlgebrasAndLeavesTheStateIntact()
    {
        var entered = ZipReduceAlgebra<TestNode,
            Dictionary<TestNode, int>, Dictionary<TestNode, int>,
            DistanceMapReduceAlgebra<TestNode>, DistanceMapReduceAlgebra<TestNode>>.Enter(Seed(), Node, 1);

        var exited = ZipReduceAlgebra<TestNode,
            Dictionary<TestNode, int>, Dictionary<TestNode, int>,
            DistanceMapReduceAlgebra<TestNode>, DistanceMapReduceAlgebra<TestNode>>.Exit(entered, Node, 1);

        Assert.Equal(1, exited.A[Node]);
        Assert.Equal(1, exited.B[Node]);
    }
}
