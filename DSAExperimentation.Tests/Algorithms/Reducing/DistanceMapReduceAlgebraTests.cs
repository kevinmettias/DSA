using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Reducing;

public sealed class DistanceMapReduceAlgebraTests
{
    [Fact]
    public void Seed_StartsAsAnEmptyMap() => Assert.Empty(DistanceMapReduceAlgebra<TestNode>.Seed);

    [Fact]
    public void Seed_IsAFreshMapPerCallSoTwoWalksCannotShareState()
    {
        var first = DistanceMapReduceAlgebra<TestNode>.Seed;
        first[new TestNode("A")] = 0;

        Assert.Empty(DistanceMapReduceAlgebra<TestNode>.Seed);
    }

    [Fact]
    public void Enter_RecordsTheNodesDepth()
    {
        var node = new TestNode("A");

        var state = DistanceMapReduceAlgebra<TestNode>.Enter(DistanceMapReduceAlgebra<TestNode>.Seed, node, 3);

        Assert.Equal(3, state[node]);
    }

    [Fact]
    public void Enter_AccumulatesAcrossNodes()
    {
        var a = new TestNode("A");
        var b = new TestNode("B");
        var state = DistanceMapReduceAlgebra<TestNode>.Seed;

        state = DistanceMapReduceAlgebra<TestNode>.Enter(state, a, 0);
        state = DistanceMapReduceAlgebra<TestNode>.Enter(state, b, 1);

        Assert.Equal(new Dictionary<TestNode, int> { [a] = 0, [b] = 1 }, state);
    }

    [Fact]
    public void Enter_SameNodeAgain_OverwritesWithTheLaterDepth()
    {
        // The guard normally prevents a revisit; this pins what happens if one occurs.
        var node = new TestNode("A");
        var state = DistanceMapReduceAlgebra<TestNode>.Seed;

        state = DistanceMapReduceAlgebra<TestNode>.Enter(state, node, 1);
        state = DistanceMapReduceAlgebra<TestNode>.Enter(state, node, 5);

        Assert.Equal(5, state[node]);
    }

    [Fact]
    public void Enter_ThreadsTheSameMapRatherThanCopyingIt()
    {
        var state = DistanceMapReduceAlgebra<TestNode>.Seed;

        var returned = DistanceMapReduceAlgebra<TestNode>.Enter(state, new TestNode("A"), 0);

        Assert.Same(state, returned);
    }
}
