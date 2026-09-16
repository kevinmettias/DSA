using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Reducing;

public sealed class ZipReduceAlgebraTests
{
    private static readonly TestNode Node = new("A");

    // Both halves are a Dictionary<TestNode, int>, so a tuple would leave the caller
    // telling this algebra's side from that one by position alone; SeedPair names which
    // side is which. The algebra itself still threads its own pair - only this
    // harness's helper was unnamed.
    private static SeedPair Seed()
    {
        var seed = ZipReduceAlgebra<TestNode,
            Dictionary<TestNode, int>, Dictionary<TestNode, int>,
            DistanceMapReduceAlgebra<TestNode>, DistanceMapReduceAlgebra<TestNode>>.Seed;

        return new SeedPair(seed.A, seed.B);
    }

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
        var seed = Seed();
        var state = ZipReduceAlgebra<TestNode,
            Dictionary<TestNode, int>, Dictionary<TestNode, int>,
            DistanceMapReduceAlgebra<TestNode>, DistanceMapReduceAlgebra<TestNode>>.Enter((seed.A, seed.B), Node, 2);

        Assert.Equal(2, state.A[Node]);
        Assert.Equal(2, state.B[Node]);
    }

    [Fact]
    public void Exit_AppliesBothAlgebrasAndLeavesTheStateIntact()
    {
        var seed = Seed();
        var seedState = (seed.A, seed.B);
        var entered = ZipReduceAlgebra<TestNode,
            Dictionary<TestNode, int>, Dictionary<TestNode, int>,
            DistanceMapReduceAlgebra<TestNode>, DistanceMapReduceAlgebra<TestNode>>.Enter(seedState, Node, 1);

        var enteredState = (entered.A, entered.B);
        var exited = ZipReduceAlgebra<TestNode,
            Dictionary<TestNode, int>, Dictionary<TestNode, int>,
            DistanceMapReduceAlgebra<TestNode>, DistanceMapReduceAlgebra<TestNode>>.Exit(enteredState, Node, 1);

        Assert.Equal(1, exited.A[Node]);
        Assert.Equal(1, exited.B[Node]);
    }

    // The two seeds the pair comes back as. Private and nested: the pair is this
    // harness's own naming of the result, not a concept the solution exposes.
    private readonly record struct SeedPair(Dictionary<TestNode, int> A, Dictionary<TestNode, int> B);
}
